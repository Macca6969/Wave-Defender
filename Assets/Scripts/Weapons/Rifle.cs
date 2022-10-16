using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Rifle : NetworkBehaviour
{
    [Header("Scripts")]
    public WeaponManager weaponManager;
    public AudioScript audioScript;

    [Header("Rifle Settings")]
    public float rifleFireRate = 0.05f;
    public int rifleBulletSpeed = 200;
    public int rifleDamage = 3;
    public int rifleMagSize = 90;
    public int rifleCurrentAmmo = 720;
    public int rifleCurrentClip = 90;
    public int rifleMaxAmmo = 720;
    public float rifleReloadSpeed = 1.2f;
    public bool isReloadingRifle = false;
    [SerializeField] private Transform rifleBulletSpawnPoint;
    [SerializeField] private GameObject rifleMuzzleFlash;
    [SerializeField] private TrailRenderer rifleBulletTrail;

 public void FiringRifle(bool playerShooting)
    {
        if (playerShooting)
        {
            string targetHit;
            string playerName = gameObject.name;
            Quaternion quatID = Quaternion.identity;
            RaycastHit _hit;

            if (Physics.Raycast(weaponManager.shootingCam.transform.position, weaponManager.shootingCam.transform.forward, out _hit))
            {
                targetHit = _hit.collider.gameObject.name;
            }
            else
            {
                targetHit = "No Target";
            }

            if (targetHit == "Player")
            {
                targetHit = _hit.collider.gameObject.name;
                GameManager.GetPlayer(targetHit);
            }


            CmdFiringRifle(playerName, targetHit, quatID, _hit.collider.gameObject);
        }
        if (!playerShooting)
        {
            Debug.Log("The player has stopped shooting.");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFiringRifle(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        RpcRifleFire(playerName, targetHit, quatID, objectHit);
    }

    [ClientRpc]
    public void RpcRifleFire(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
        if (transform.name == playerName)
        {
            RaycastHit _hit;
            Vector3 direction = weaponManager.shootingCam.transform.forward;
            Physics.Raycast(weaponManager.shootingCam.transform.position, direction, out _hit);

            if (!weaponManager.isFiring && rifleCurrentClip >= 1 && !weaponManager.player.isDead)
            {
                StartCoroutine(Firerifle());
            }
            IEnumerator Firerifle()
            {
                weaponManager.isFiring = true;
                //animator.Play("Firerifle");
                audioScript.rifleShoot.Play();

                if (targetHit != "No Target")
                {
                    TrailRenderer trail = Instantiate(rifleBulletTrail, rifleBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnRifleBulletTrail(trail, _hit.point, _hit.normal, true));
                }
                if (targetHit == "No Target")
                {
                    Debug.Log("We have shot nothing.");
                    TrailRenderer trail = Instantiate(rifleBulletTrail, rifleBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnRifleBulletTrail(trail, weaponManager.shootingCam.transform.forward * 100f, Vector3.zero, false));
                }

                //ATTACK ENEMY
                if (_hit.transform.tag == "Enemy")
                {
                    if (objectHit.TryGetComponent<Enemy>(out Enemy enemyComponent))
                    {
                        enemyComponent.TakeDamage(rifleDamage, playerName);
                    }
                    
                  
                }

                //muzzleFlash.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                //muzzleFlash.SetActive(false);
                //animator.Play("Idlerifle");
                rifleCurrentClip = rifleCurrentClip - 1;
                weaponManager.manageUI.UpdateAmmoUI();
                yield return new WaitForSeconds(rifleFireRate);
                weaponManager.isFiring = false;

                FiringRifle(weaponManager.playerController.playerShooting);
            }

            if (!weaponManager.isFiring && rifleCurrentClip >= 0 && !weaponManager.player.isDead)
            {
                audioScript.rifleEmpty.Play();
            }
        }
    }
    //spawn the trail
    private IEnumerator SpawnRifleBulletTrail(TrailRenderer Trail, Vector3 _hitPoint, Vector3 _hitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, _hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, _hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= rifleBulletSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public void RifleReload()
        {
            string _player = gameObject.name;
            CmdPlayerReloadingRifle(_player);
        }

        [Command(requiresAuthority = false)]
        public void CmdPlayerReloadingRifle(string _player)
        {
              RpcPlayerReloadingRifle(_player);
        }

        [ClientRpc]
        public void RpcPlayerReloadingRifle(string _player)
         {
            if(gameObject.name == _player)
            
               if (!isReloadingRifle)
               {
                StartCoroutine(Reload());
               }
                IEnumerator Reload()
               {
                isReloadingRifle = true;
                //animator.Play("Reload");
                audioScript.rifleReload.Play();
                yield return new WaitForSeconds (rifleReloadSpeed);
                audioScript.rifleReload.Stop();
                //animator.Play("Idlerifle");
                int reloadAmount = rifleMagSize - rifleCurrentClip;
                reloadAmount = (rifleCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : rifleCurrentAmmo;
                rifleCurrentClip += reloadAmount;
                rifleCurrentAmmo -= reloadAmount;
                weaponManager.manageUI.UpdateAmmoUI();
                isReloadingRifle = false;
               }
            }

            public void AddAmmoRifle(int ammoAmount)
            {
               rifleCurrentAmmo += ammoAmount;
               if(rifleCurrentAmmo > rifleMaxAmmo)
               {
                  rifleCurrentAmmo = rifleMagSize;
               }
               
            }

           

}
