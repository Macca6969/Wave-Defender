using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{

   #region Variables


  [Header("Objects")]
  [SerializeField] public GameObject playerUI;
  [SerializeField] private GameObject rightHand;
  [SerializeField] private Camera cam;
  [SerializeField] private CharacterController characterController;
  [SerializeField] GameObject[] disableGameObjectsOnDeath;
  [SerializeField] private MeshRenderer meshRen;
  [SerializeField] private GameObject meshRenWeapon;

  [Header("Scripts")]
  [SerializeField] private Pistol pistol;
  [SerializeField] private AudioScript audioScript;
  [SerializeField] private PlayerController playerController;
  [SerializeField] private SetupUI setupUI;



  [Header("UI")]
  [SerializeField] Image healthBarImage;
  [SerializeField] private GameObject healthTextUI;
  [SerializeField] private GameObject levelTextUI;

  [Header("Other")]
  [SerializeField] private bool [] wasEnabled;
  [SerializeField] private Behaviour[] disableOnDeath;
  
  [SerializeField] public int playerMaxHealth = 100;
  [SerializeField] public int playerLevel = 5;
  [SerializeField] private bool firstSetup = true;

  [Header("Sync Vars")]
  [SyncVar] public int playerCurrentHealth;
  [SyncVar] public int playerCurrentLevel;
  [SyncVar] private bool _isDead = false;
  public bool isDead 
  {
    get { return _isDead; }
    protected set { _isDead = value; }
  }



  #endregion

public void Update()
{
  levelTextUI.GetComponent<TMP_Text>().text = "Lvl - " + playerLevel;
}


  #region Player Setup
  public void SetupPlayer()
  {
      if(!isLocalPlayer)
      {
       string newPlayer = transform.name;
       Debug.Log(transform.name + " is running SetupPlayer.");
       CmdBroadCastNewPlayerSetup(newPlayer);
      }
  }

  [Command(requiresAuthority = false)]
  private void CmdBroadCastNewPlayerSetup(string newPlayer)
  {
      RpcSetupPlayerOnAllClients(newPlayer);
  }

[ClientRpc]
private void RpcSetupPlayerOnAllClients(string newPlayer)
{
  if (transform.name == newPlayer)
  {
  if (firstSetup)
  {
   wasEnabled = new bool[disableOnDeath.Length];
      for (int i = 0; i < wasEnabled.Length; i++)
      {
           wasEnabled[i] = disableOnDeath[i].enabled;
      }
      firstSetup = false;
  }
      Debug.Log("Setting defaults for " + transform.name);
      SetDefaults();
  }
  
}

public void SetDefaults ()
{
  Debug.Log("Setting Defaults for " + transform.name);
  isDead = false;

    Cursor.lockState = CursorLockMode.Locked;
    playerCurrentHealth = playerMaxHealth;
    healthTextUI.GetComponent<TMP_Text>().text = playerCurrentHealth + "/" + playerMaxHealth;
    levelTextUI.GetComponent<TMP_Text>().text = "Lvl - " + playerLevel;
    healthBarImage.fillAmount = (playerCurrentHealth + 0.0f) / (playerMaxHealth + 0.0f);
    pistol.ammoTextUI.GetComponent<TMP_Text>().text =  pistol.pistolCurrentClip + "/" + pistol.pistolCurrentAmmo;

   //Enable the collider
   Collider _col = GetComponent<Collider>();
   _col.enabled = true;

   // Components to enable
    characterController = GetComponent<CharacterController>();
    characterController.enabled = true;
    playerController.enabled = true;
    Debug.Log("Character controller enabled.");


    meshRen.enabled = true;
    meshRenWeapon.SetActive(true);

    

}

private void Start()
 {
    playerCurrentHealth = playerMaxHealth;
 }


#endregion

#region Player Damage, death, respawn
[ClientRpc]
public void RpcTakeDamage (int _amount)
{
  if (isDead)
      return;
    
    playerCurrentHealth -= _amount;
    setupUI.UpdateHealthUI();
    Debug.Log(transform.name + "now has " + playerCurrentHealth + "health.");
    audioScript.playerHitSound.Play();

    if (playerCurrentHealth <= 0)
    {
        Die();
    }
}


private void Die()
  {
    isDead = true;
    if (isDead == true) 
      {
        Debug.Log (transform.name + " is DEAD!");

        //Disable components
    Collider _col = GetComponent<Collider>();
    if (_col != false)
    {
        _col.enabled = false;
    }
    characterController = GetComponent<CharacterController>();
    if (characterController != false)
    {
       characterController.enabled = false;
       playerController.enabled = false;
       Debug.Log("Character controller disabled.");
    }
    meshRen.enabled = false;
    meshRenWeapon.SetActive(false);
    }
    
    
    string playerRespawning = transform.name;
    PlayerRespawn(playerRespawning);
    
  }

  [ClientRpc]
  private void PlayerRespawn(string playerRespawning)
  {
     if (transform.name == playerRespawning)
     {
       StartCoroutine(Respawn(playerRespawning));
  
       IEnumerator Respawn (string playerRespawning)
       {  
          yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);

          transform.position = NetworkManager.singleton.GetStartPosition().position;
          transform.rotation = Quaternion.Euler(0, 0, 0);

          yield return new WaitForSeconds(0.1f);

          SetDefaults();

         Debug.Log(transform.name + " respawned.");
       }   
   }
 }

#endregion

}












