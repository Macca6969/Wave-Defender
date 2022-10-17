using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pistol : NetworkBehaviour
{
    [Header("Scripts")]
    public WeaponManager weaponManager;
    public AudioScript audioScript;

    [Header("Pistol Settings")]
    public float pistolFireRate = 0.4f;
    public int pistolBulletSpeed = 100;
    public int pistolDamage = 10;
    public int pistolMagSize = 7;
    public int pistolCurrentAmmo = 45;
    public int pistolCurrentClip = 7;
    public int pistolMaxAmmo = 50;
    public float pistolReloadSpeed = 0.8f;
    public bool isReloadingPistol = false;
    [SerializeField] private Transform pistolBulletSpawnPoint;
    [SerializeField] private GameObject pistolMuzzleFlash;
    [SerializeField] private TrailRenderer pistolBulletTrail;

 public void FiringPistol(bool playerShooting)
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


            CmdFiringPistol(playerName, targetHit, quatID, _hit.collider.gameObject);
        }
        if (!playerShooting)
        {
            Debug.Log("The player has stopped shooting.");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFiringPistol(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        RpcPistolFire(playerName, targetHit, quatID, objectHit);
    }

    [ClientRpc]
    public void RpcPistolFire(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
        if (transform.name == playerName)
        {
            RaycastHit _hit;
            Vector3 direction = weaponManager.shootingCam.transform.forward;
            Physics.Raycast(weaponManager.shootingCam.transform.position, direction, out _hit);

            if (!weaponManager.isFiring && pistolCurrentClip >= 1 && !weaponManager.player.isDead)
            {
                StartCoroutine(FirePistol());
            }
            IEnumerator FirePistol()
            {
                weaponManager.isFiring = true;
                //animator.Play("FirePistol");
                audioScript.pistolShoot.Play();

                if (targetHit != "No Target")
                {
                    TrailRenderer trail = Instantiate(pistolBulletTrail, pistolBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnPistolBulletTrail(trail, _hit.point, _hit.normal, true));
                }
                if (targetHit == "No Target")
                {
                    Debug.Log("We have shot nothing.");
                    TrailRenderer trail = Instantiate(pistolBulletTrail, pistolBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnPistolBulletTrail(trail, weaponManager.shootingCam.transform.forward * 100f, Vector3.zero, false));
                }

                //ATTACK ENEMY
                if (_hit.transform.tag == "Enemy")
                {
                    if (objectHit.TryGetComponent<Enemy>(out Enemy enemyComponent))
                    {
                        enemyComponent.CmdTakeDamage(pistolDamage, playerName);
                    }
                    
                  
                }

                //muzzleFlash.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                //muzzleFlash.SetActive(false);
                //animator.Play("IdlePistol");
                pistolCurrentClip = pistolCurrentClip - 1;
                weaponManager.manageUI.UpdateAmmoUI();
                yield return new WaitForSeconds(pistolFireRate);
                weaponManager.isFiring = false;

                FiringPistol(weaponManager.playerController.playerShooting);
            }

            if (!weaponManager.isFiring && pistolCurrentClip >= 0 && !weaponManager.player.isDead)
            {
                audioScript.pistolEmpty.Play();
            }
        }
    }
    //spawn the trail
    private IEnumerator SpawnPistolBulletTrail(TrailRenderer Trail, Vector3 _hitPoint, Vector3 _hitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, _hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, _hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= pistolBulletSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public void PistolReload()
        {
            string _player = gameObject.name;
            CmdPlayerReloadingPistol(_player);
        }

        [Command(requiresAuthority = false)]
        public void CmdPlayerReloadingPistol(string _player)
        {
              RpcPlayerReloadingPistol(_player);
        }

        [ClientRpc]
        public void RpcPlayerReloadingPistol(string _player)
         {
            if(gameObject.name == _player)
            
               if (!isReloadingPistol)
               {
                StartCoroutine(Reload());
               }
                IEnumerator Reload()
               {
                isReloadingPistol = true;
                //animator.Play("Reload");
                audioScript.pistolReload.Play();
                yield return new WaitForSeconds (pistolReloadSpeed);
                audioScript.pistolReload.Stop();
                //animator.Play("IdlePistol");
                int reloadAmount = pistolMagSize - pistolCurrentClip;
                reloadAmount = (pistolCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : pistolCurrentAmmo;
                pistolCurrentClip += reloadAmount;
                pistolCurrentAmmo -= reloadAmount;
                weaponManager.manageUI.UpdateAmmoUI();
                isReloadingPistol = false;
               }
            }

            public void AddAmmoPistol(int ammoAmount)
            {
               pistolCurrentAmmo += ammoAmount;
               if(pistolCurrentAmmo > pistolMaxAmmo)
               {
                  pistolCurrentAmmo = pistolMagSize;
               }
            }

           

}
