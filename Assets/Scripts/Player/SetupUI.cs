using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;


public class SetupUI : NetworkBehaviour
{

[Header("UI Components")]
[SerializeField] Image healthBarImage;
[SerializeField] private GameObject healthTextUI;
public GameObject ammoTextUI;

[Header("Objects")]
public GameObject playerUI;

[Header("Scripts")]
public Pistol pistol;
public Player player;



public void SetupPlayerUI()
{

    StartCoroutine(SetupUI());

    IEnumerator SetupUI()
    {
    
    yield return new WaitForSeconds(0.0f);
    healthTextUI.GetComponent<TMP_Text>().text = player.playerCurrentHealth + "/" + player.playerMaxHealth;
    healthBarImage.fillAmount = (player.playerCurrentHealth + 0.0f) / (player.playerMaxHealth + 0.0f);
    pistol.ammoTextUI.GetComponent<TMP_Text>().text =  pistol.pistolCurrentClip + "/" + pistol.pistolCurrentAmmo;
    }
}

public void UpdateHealthUI()
{
    healthTextUI.GetComponent<TMP_Text>().text = player.playerCurrentHealth + "/" + player.playerMaxHealth;
    healthBarImage.fillAmount = (player.playerCurrentHealth + 0.0f) / (player.playerMaxHealth + 0.0f);
}

}
