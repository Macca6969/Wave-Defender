using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Heavy : NetworkBehaviour
{
    [Header("Scripts")]
    public WeaponManager weaponManager;
    public AudioScript audioScript;

    [Header("Heavy Settings")]
    public int heavyFireRate;
    public int heavyBulletSpeed = 30;
    public int heavyDamage = 35;
    public int heavyMagSize = 6;
    public int heavyCurrentAmmo = 21;
    public int heavyCurrentClip = 6;
    public int heavyMaxAmmo = 36;
    public float heavyReloadSpeed = 2.5f;
    public bool isReloadingHeavy = false;
    public bool needReloadHeavy = false;
    [SerializeField] private Transform heavyBulletSpawnPoint;
    [SerializeField] private TrailRenderer heavyBulletTrail;
    [SerializeField] private GameObject heavyMuzzleFlash;

    public void FiringHeavy(bool playerShooting)
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


            CmdFiringHeavy(playerName, targetHit, quatID, _hit.collider.gameObject);
        }
        if (!playerShooting)
        {
            Debug.Log("The player has stopped shooting.");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFiringHeavy(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        RpcHeavyFire(playerName, targetHit, quatID, objectHit);
    }

    [ClientRpc]
    public void RpcHeavyFire(string playerName, string targetHit, Quaternion quatID, GameObject objectHit)
    {
        // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
        if (transform.name == playerName)
        {
            RaycastHit _hit;
            Vector3 direction = weaponManager.shootingCam.transform.forward;
            Physics.Raycast(weaponManager.shootingCam.transform.position, direction, out _hit);

            if (!weaponManager.isFiring && heavyCurrentClip >= 1 && !weaponManager.player.isDead)
            {
                StartCoroutine(Fireheavy());
            }
            IEnumerator Fireheavy()
            {
                weaponManager.isFiring = true;
                //animator.Play("Fireheavy");
                audioScript.heavyShoot.Play();

                if (targetHit != "No Target")
                {
                    TrailRenderer trail = Instantiate(heavyBulletTrail, heavyBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnHeavyBulletTrail(trail, _hit.point, _hit.normal, true));
                }
                if (targetHit == "No Target")
                {
                    Debug.Log("We have shot nothing.");
                    TrailRenderer trail = Instantiate(heavyBulletTrail, heavyBulletSpawnPoint.position, quatID);
                    StartCoroutine(SpawnHeavyBulletTrail(trail, weaponManager.shootingCam.transform.forward * 100f, Vector3.zero, false));
                }

                //ATTACK ENEMY
                if (_hit.transform.tag == "Enemy")
                {
                    if (objectHit.TryGetComponent<Enemy>(out Enemy enemyComponent))
                    {
                        enemyComponent.CmdTakeDamage(heavyDamage, playerName);
                    }


                }

                //muzzleFlash.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                //muzzleFlash.SetActive(false);
                //animator.Play("Idleheavy");
                heavyCurrentClip = heavyCurrentClip - 1;
                weaponManager.manageUI.UpdateAmmoUI();
                yield return new WaitForSeconds(heavyFireRate);
                weaponManager.isFiring = false;

                FiringHeavy(weaponManager.playerController.playerShooting);
            }

            if (!weaponManager.isFiring && heavyCurrentClip >= 0 && !weaponManager.player.isDead)
            {
                audioScript.heavyEmpty.Play();
            }
        }
    }
    //spawn the trail
    private IEnumerator SpawnHeavyBulletTrail(TrailRenderer Trail, Vector3 _hitPoint, Vector3 _hitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, _hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, _hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= heavyBulletSpeed * Time.deltaTime;

            yield return null;
        }
    }

    public void HeavyReload()
    {
        string _player = gameObject.name;
        CmdPlayerReloadingHeavy(_player);
    }

    [Command(requiresAuthority = false)]
    public void CmdPlayerReloadingHeavy(string _player)
    {
        RpcPlayerReloadingheavy(_player);
    }

    [ClientRpc]
    public void RpcPlayerReloadingheavy(string _player)
    {
        if (gameObject.name == _player)

            if (!isReloadingHeavy)
            {
                StartCoroutine(Reload());
            }
        IEnumerator Reload()
        {
            isReloadingHeavy = true;
            //animator.Play("Reload");
            audioScript.heavyReload.Play();
            yield return new WaitForSeconds(heavyReloadSpeed);
            audioScript.heavyReload.Stop();
            //animator.Play("Idleheavy");
            int reloadAmount = heavyMagSize - heavyCurrentClip;
            reloadAmount = (heavyCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : heavyCurrentAmmo;
            heavyCurrentClip += reloadAmount;
            heavyCurrentAmmo -= reloadAmount;
            weaponManager.manageUI.UpdateAmmoUI();
            isReloadingHeavy = false;
        }
    }

    public void AddAmmoheavy(int ammoAmount)
    {
        heavyCurrentAmmo += ammoAmount;
        if (heavyCurrentAmmo > heavyMaxAmmo)
        {
            heavyCurrentAmmo = heavyMagSize;
        }

    }


}
