using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{



    [Header("Scripts.")]
    public Player player;
    public PlayerController playerController;
    public AudioScript audioScript;
    public ManageUI manageUI;
    public Pistol pistol;


    [Header("Objects")]
    public Camera cam;
    public Camera shootingCam;

    

    [Header("Rifle Settings")]
    public float rifleFireRate = 0.05f;
    public int rifleBulletSpeed = 200;
    public int rifleDamage = 7;
    public int rifleMagSize = 21;
    public int rifleCurrentAmmo = 130;
    public int rifleCurrentClip = 21;
    public int rifleMaxAmmo = 50;
    public float rifleReloadSpeed = 1.6f;
    public bool isReloadingRifle = false;
    public bool needReloadRifle = false;
    [SerializeField] private Transform rifleBulletSpawnPoint;
    [SerializeField] private TrailRenderer rifleBulletTrail;
    [SerializeField] private GameObject rifleMuzzleFlash;

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


    public bool isFiring;





    private void Awake()
    {

    }


    public void PlayerCheckWeapon(int selectedWeapon, bool playerShooting)
    {
        if (selectedWeapon == 0)
        {
            pistol.FiringPistol(playerShooting);
        }

        if (selectedWeapon == 1)
        {
            FiringRifle(playerShooting);
        }

        if (selectedWeapon == 2)
        {
            FiringHeavy(playerShooting);
        }

    }


    #region Rifle

    public void FiringRifle(bool playerShooting)
    {
        if (playerShooting)
        {
            string targetHit;
            string playerName = gameObject.name;
            Quaternion quatID = Quaternion.identity;
            RaycastHit _hit;

            if (Physics.Raycast(shootingCam.transform.position, shootingCam.transform.forward, out _hit))
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


            CmdFiringRifle(playerName, targetHit, quatID);
        }
        if (!playerShooting)
        {
            Debug.Log("The player has stopped shooting.");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFiringRifle(string playerName, string targetHit, Quaternion quatID)
    {
        RpcRifleFire(playerName, targetHit, quatID);
    }

    [ClientRpc]
    public void RpcRifleFire(string playerName, string targetHit, Quaternion quatID)
    {
        //checks if this is the player that shot
        // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
        if (transform.name == playerName)
        {
            RaycastHit _hit;
            Vector3 direction = shootingCam.transform.forward;
            Physics.Raycast(shootingCam.transform.position, direction, out _hit);

            if (!isFiring && rifleCurrentClip >= 1 && !player.isDead)
            {
                StartCoroutine(FireRifle());
                Debug.Log(playerName + " has shot " + targetHit + " with the rifle");
            }
            IEnumerator FireRifle()
            {
                isFiring = true;
                //animator.Play("FirePistol");
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
                    StartCoroutine(SpawnRifleBulletTrail(trail, shootingCam.transform.forward * 100f, Vector3.zero, false));
                }

                //muzzleFlash.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                //muzzleFlash.SetActive(false);
                //animator.Play("IdlePistol");
                rifleCurrentClip = rifleCurrentClip - 1;
                manageUI.UpdateAmmoUI();
                yield return new WaitForSeconds(rifleFireRate);
                isFiring = false;

                FiringRifle(playerController.playerShooting);
            }

            if (!isFiring && rifleCurrentClip >= 0 && !player.isDead)
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
                //animator.Play("IdlePistol");
                int reloadAmount = rifleMagSize - rifleCurrentClip;
                reloadAmount = (rifleCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : rifleCurrentAmmo;
                rifleCurrentClip += reloadAmount;
                rifleCurrentAmmo -= reloadAmount;
                manageUI.UpdateAmmoUI();
                needReloadRifle = false;
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

    #endregion

    #region Heavy

    public void FiringHeavy(bool playerShooting)
    {
        if (playerShooting)
        {
            string targetHit;
            string playerName = gameObject.name;
            Quaternion quatID = Quaternion.identity;
            RaycastHit _hit;

            if (Physics.Raycast(shootingCam.transform.position, shootingCam.transform.forward, out _hit))
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


            CmdFiringHeavy(playerName, targetHit, quatID);
        }
        if (!playerShooting)
        {
            Debug.Log("The player has stopped shooting.");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdFiringHeavy(string playerName, string targetHit, Quaternion quatID)
    {
        Debug.Log(playerName + " has shot " + targetHit + " with the pistol");
        RpcHeavyFire(playerName, targetHit, quatID);
    }

    [ClientRpc]
    public void RpcHeavyFire(string playerName, string targetHit, Quaternion quatID)
    {
        //checks if this is the player that shot
        // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
        if (transform.name == playerName)
        {
            RaycastHit _hit;
            Vector3 direction = shootingCam.transform.forward;
            Physics.Raycast(shootingCam.transform.position, direction, out _hit);

            if (!isFiring && heavyCurrentClip >= 1 && !player.isDead)
            {
                StartCoroutine(FirePistol());
            }
            IEnumerator FirePistol()
            {
                isFiring = true;
                //animator.Play("FirePistol");
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
                    StartCoroutine(SpawnHeavyBulletTrail(trail, shootingCam.transform.forward * 100f, Vector3.zero, false));
                }

                //muzzleFlash.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                //muzzleFlash.SetActive(false);
                //animator.Play("IdlePistol");
                heavyCurrentClip = heavyCurrentClip - 1;
                manageUI.UpdateAmmoUI();
                yield return new WaitForSeconds(heavyFireRate);
                isFiring = false;
                playerController.playerShooting = false;

                FiringHeavy(playerController.playerShooting);
            }

            if (!isFiring && heavyCurrentClip >= 0 && !player.isDead)
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
              RpcPlayerReloadingHeavy(_player);
        }

        [ClientRpc]
        public void RpcPlayerReloadingHeavy(string _player)
         {
            if(gameObject.name == _player)
            
               if (!isReloadingRifle)
               {
                StartCoroutine(Reload());
               }
                IEnumerator Reload()
               {
                isReloadingHeavy = true;
                //animator.Play("Reload");
                audioScript.heavyReload.Play();
                yield return new WaitForSeconds (heavyReloadSpeed);
                audioScript.heavyReload.Stop();
                //animator.Play("IdlePistol");
                int reloadAmount = heavyMagSize - heavyCurrentClip;
                reloadAmount = (heavyCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : heavyCurrentAmmo;
                heavyCurrentClip += reloadAmount;
                heavyCurrentAmmo -= reloadAmount;
                manageUI.UpdateAmmoUI();
                needReloadHeavy = false;
                isReloadingHeavy = false;
               }
            }

            public void AddAmmoHeavy(int ammoAmount)
            {
               heavyCurrentAmmo += ammoAmount;
               if(heavyCurrentAmmo > heavyMaxAmmo)
               {
                  heavyCurrentAmmo = heavyMagSize;
               }
            }

    #endregion


}







