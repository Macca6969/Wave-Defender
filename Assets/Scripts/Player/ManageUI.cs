using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;


public class ManageUI : NetworkBehaviour
{

    [Header("UI Components")]
    [SerializeField] Image healthBarImage;
    [SerializeField] private GameObject healthTextUI;
    public GameObject ammoTextUI;
    private float lerpTimer;

    [Header("Objects")]
    public GameObject playerUI;

    [Header("Scripts")]
    public WeaponManager weaponManager;
    public Player player;
    public PlayerController playerController;
    public PlayerHealth playerHealth;



    public void SetupPlayerUI()
    {

        StartCoroutine(SetupUI());

        IEnumerator SetupUI()
        {

            yield return new WaitForSeconds(0.0f);

            playerUI.gameObject.SetActive(true);
            UpdateAmmoUI();
            UpdateHealthUI();

        }
    }

    public void UpdateHealthUI()
    {
        float fillF = playerHealth.frontHealthBar.fillAmount;
         float fillB = playerHealth.backHealthBar.fillAmount;
         
         //sets our health to a range 0-1 to adjust health bar fill amount
         float tempCurrentHealth = (float)player.playerCurrentHealth;
         float tempMaxHealth = (float)player.playerMaxHealth * 1.0f;
         float hFraction = tempCurrentHealth / tempMaxHealth;
         if (fillB > hFraction)
         {
            playerHealth.frontHealthBar.fillAmount = hFraction;
            playerHealth.backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / playerHealth.chipSpeed;
            percentComplete = percentComplete * percentComplete;
            playerHealth.backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
         }


         //heal
         if (fillF < hFraction)
         {
            playerHealth.backHealthBar.color = Color.green;
            playerHealth.backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / playerHealth.chipSpeed;
            percentComplete = percentComplete * percentComplete;
            playerHealth.frontHealthBar.fillAmount = Mathf.Lerp(fillF, playerHealth.backHealthBar.fillAmount, percentComplete);
         }
         playerHealth.healthText.text = Mathf.Round(player.playerCurrentHealth) + "/" + Mathf.Round(player.playerMaxHealth);
    }

    public void UpdateAmmoUI()
    {
        if (playerController.selectedWeapon == 0)
        {
            ammoTextUI.GetComponent<TMP_Text>().text = weaponManager.pistol.pistolCurrentClip + "/" + weaponManager.pistol.pistolCurrentAmmo;
        }
        if (playerController.selectedWeapon == 1)
        {
            ammoTextUI.GetComponent<TMP_Text>().text = weaponManager.rifle.rifleCurrentClip + "/" + weaponManager.rifle.rifleCurrentAmmo;
        }
        if (playerController.selectedWeapon == 2)
        {
            ammoTextUI.GetComponent<TMP_Text>().text = weaponManager.heavy.heavyCurrentClip + "/" + weaponManager.heavy.heavyCurrentAmmo;
        }
    }

}
