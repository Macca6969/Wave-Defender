using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Pistol : NetworkBehaviour
{
#region Variables ~ 

    [Header("Pistol")]

    public int pistolDamage = 10;
    public int pistolMagSize = 7;
    public int pistolCurrentAmmo = 20;
    public int pistolCurrentClip = 7;
    public int pistolMaxAmmo = 50;
    public float pistolReloadSpeed = 0.8f;
    public string _ID;
    [SerializeField] private TrailRenderer bulletTrail;

    [Header("Objects")]
    public GameObject healthTextUI;
    public Image healthBarImage;
    private Animator Animator;
    public GameObject ammoTextUI;
    public Camera cam;
    public GameObject muzzleFlash;
    public GameObject pistolObj;
    public Transform bulletSpawnPoint;
    public Animator animator;

    [Header("Scripts")]
    public AudioScript audioScript;
    public Player player;

    [Header("Other")]
    
    public bool needReload;
    float toTarget;
    public bool isDead;
    static float distantFromTarget;
    public bool isFiring = false;
    public bool isReloading = false;

   #endregion


    private void Awake()
     {
        Animator = GetComponentInChildren<Animator>();
     }



    
    #region Player Shoots

      public void PistolFire()
      {
            RaycastHit _hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit))
              {
  
                  // Tracers
                  
                  Quaternion quatID = Quaternion.identity;

                  // If we hit a player  **change from tag to layer possibly?**
                  if(_hit.collider.tag == "Player")
                {
                  Debug.Log("We hit " + _hit.collider.gameObject.name);
                  string playerName = gameObject.name;
                  string playerHit = _hit.collider.gameObject.name;
                  string objectHit = _hit.collider.gameObject.name;
                  Player playerHit2 = GameManager.GetPlayer(playerHit);
                  CmdPlayerShoots(playerName, playerHit, objectHit, quatID);
                }
                  else
                  {
                     if(_hit.collider.tag != "Player")
                    {
                       Debug.Log("We hit " + _hit.collider.gameObject.name);
                       string playerName = gameObject.name;
                       string objectHit = _hit.collider.gameObject.name;
                       string playerHit = "none";
                       CmdPlayerShoots(playerName, playerHit, objectHit, quatID);
                     }
                  }
               }
      }

         [Command(requiresAuthority = false)]
         void CmdPlayerShoots(string playerName, string playerHit, string objectHit, Quaternion quatID)
         {
            if(playerHit != "none")
            {
            //RpcPistolFire(playerName);
            RpcPistolFire(playerName, quatID, playerHit);
            Player playerHit2 = GameManager.GetPlayer(playerHit);
            playerHit2.RpcTakeDamage(pistolDamage);
            }
            else 
            {
               Player playerShot = GameManager.GetPlayer(playerName);
               RpcPistolFire(playerName, quatID, playerHit);
            }
        }
          
         [ClientRpc]
         public void RpcPistolFire(string playerName, Quaternion quatID, string playerHit)
         {
         //checks if this is the player that shot
         // [ClientRpc(includeOwner = false)]  can be used to send to everyone except the owner. redo shooting method to client + clientRpc
          if (transform.name == playerName)
          {
            RaycastHit _hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit))

            if  (!isFiring && pistolCurrentClip >= 1 && !player.isDead) 
                {
                 StartCoroutine(FirePistol());
                }
                 IEnumerator FirePistol()
                  {
                  Debug.Log(transform.name + " is shooting the pistol");
                  isFiring = true;
                  animator.Play("FirePistol");
                  audioScript.pistolShot.Play();

                  TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
                  StartCoroutine(SpawnBulletTrail(trail, _hit));

                  muzzleFlash.SetActive(true);
                  yield return new WaitForSeconds(0.05f);
                  muzzleFlash.SetActive(false);
                  yield return new WaitForSeconds(0.15f);
                  animator.Play("IdlePistol");
                  pistolCurrentClip = pistolCurrentClip - 1;
                  ammoTextUI.GetComponent<TMP_Text>().text =  pistolCurrentClip + "/" + pistolCurrentAmmo;
                  isFiring = false;
                  }

            if(!isFiring && pistolCurrentClip >=0 && !player.isDead)
               {
               audioScript.pistolEmpty.Play();
               }
          }
                         
            
         }

         private IEnumerator SpawnBulletTrail(TrailRenderer Trail, RaycastHit _hit)
            {
               float time = 0;
               Vector3 startPosition = Trail.transform.position;

               while (time < 1)
               {
                  Trail.transform.position = Vector3.Lerp(startPosition, _hit.point, time);
                  time += Time.deltaTime / Trail.time;

                  yield return null;
               }
            }
         

         #endregion

    #region Player Reloading

         
        public void PistolReload()
        {
            string _player = gameObject.name;
            CmdPlayerReloading(_player);
        }

        [Command(requiresAuthority = false)]
        public void CmdPlayerReloading(string _player)
        {
              RpcPlayerReloading(_player);
        }

        [ClientRpc]
        public void RpcPlayerReloading(string _player)
         {
            if(gameObject.name == _player)
            
               if (!isReloading)
               {
                StartCoroutine(Reload());
               }
                IEnumerator Reload()
               {
                Debug.Log (_player + " is reloading");
                isReloading = true;
                animator.Play("Reload");
                audioScript.pistolReload.Play();
                yield return new WaitForSeconds (pistolReloadSpeed);
                audioScript.pistolReload.Stop();
                animator.Play("IdlePistol");
                int reloadAmount = pistolMagSize - pistolCurrentClip;
                reloadAmount = (pistolCurrentAmmo - reloadAmount) >= 0 ? reloadAmount : pistolCurrentAmmo;
                pistolCurrentClip += reloadAmount;
                pistolCurrentAmmo -= reloadAmount;
                ammoTextUI.GetComponent<TMP_Text>().text =  pistolCurrentClip + "/" + pistolCurrentAmmo;
                needReload = false;
                isReloading = false;
               }
            }

            public void AddAmmo(int ammoAmount)
            {
               pistolCurrentAmmo += ammoAmount;
               if(pistolCurrentAmmo > pistolMaxAmmo)
               {
                  pistolCurrentAmmo = pistolMagSize;
               }
            }

        #endregion


}
   

