using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{


  [Header("Objects")]
  [SerializeField] public GameObject playerUI;
  [SerializeField] private GameObject rightHand;
  [SerializeField] private Camera cam;
  [SerializeField] private CharacterController characterController;
  [SerializeField] private MeshRenderer meshRen;

  [Header("UI")]
  [SerializeField] Image healthBarImage;
  [SerializeField] private GameObject healthTextUI;
  [SerializeField] private GameObject levelTextUI;

  [Header("Scripts")]
  //[SerializeField] private Pistol pistol;
  [SerializeField] private AudioScript audioScript;
  [SerializeField] private PlayerController playerController;
  [SerializeField] private ManageUI setupUI;
  [SerializeField] private MatchSettings matchSettings;

  [Header("Other")]
  [SerializeField] private bool [] wasEnabled;
  [SerializeField] private Behaviour[] disableOnDeath;
  
  [SerializeField] private bool firstSetup = true;

  [Header("Sync Vars")]
  [SyncVar] public int playerMaxHealth;
  [SyncVar] public int playerCurrentHealth;
  [SyncVar] public int playerCurrentLevel;
  [SyncVar] public float currentXp;
  [SyncVar] public float requiredXp;

  [SyncVar] private bool _isDead = false;
  public bool isDead 
  {
    get { return _isDead; }
    protected set { _isDead = value; }
  }

  [SyncVar] public int selectedWeapon = 0;


  #region Player Setup
  public void SetupPlayer()
  {
      if(!isLocalPlayer)
      {
       string newPlayer = transform.name;
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
    //levelTextUI.GetComponent<TMP_Text>().text = "Lvl - " + playerLevel;
    healthBarImage.fillAmount = (playerCurrentHealth + 0.0f) / (playerMaxHealth + 0.0f);
   // pistol.ammoTextUI.GetComponent<TMP_Text>().text =  pistol.pistolCurrentClip + "/" + pistol.pistolCurrentAmmo;

   //Enable the collider
   Collider _col = GetComponent<Collider>();
   _col.enabled = true;

   // Components to enable
    characterController = GetComponent<CharacterController>();
    if (characterController == false)
    characterController.enabled = true;
    if (playerController == false)
    {
    playerController.enabled = true;
    Debug.Log("Character controller enabled.");
    }


    meshRen.enabled = true;



    

}

private void Start()
 {
    playerCurrentHealth = playerMaxHealth;
    playerMaxHealth = matchSettings.setMaxHealth;
 }


#endregion

#region Player Damage, death, respawn
[ClientRpc]
public void RpcTakeDamage (int _amount)
{
  if (isDead)
      return;
    
    playerCurrentHealth -= _amount;
    //setupUI.UpdateHealthUI();
    Debug.Log(transform.name + "now has " + playerCurrentHealth + "health.");
    //audioScript.playerHitSound.Play();

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
          yield return new WaitForSeconds (matchSettings.respawnTime);

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
