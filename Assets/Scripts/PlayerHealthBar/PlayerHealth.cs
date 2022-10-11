using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{


    private float lerpTimer;

    public int levelHealthMultiplier = 2;
    public int chipSpeed = 1;
    public Image frontHealthBar;
    public Image backHealthBar;
    private int playerLevel;


    [Header("Scripts")]
    public Player player;

    public int healthIncrement;

    public TextMeshProUGUI healthText;
    // Start is called before the first frame update
    void Start()
    {
         healthIncrement = (player.playerMaxHealth / 8);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthUI();
        if(Input.GetKeyDown(KeyCode.K))
        {
            //damages a random amount between 5-10
            TakeDamage(Random.Range(5, 10));
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            //damages a random amount between 5-10
            RestoreHealth(Random.Range(5, 10));
        }
    }

    public void UpdateHealthUI()
    {
         float fillF = frontHealthBar.fillAmount;
         float fillB = backHealthBar.fillAmount;
         //sets our health to a range 0-1 to adjust health bar fill amount
         float tempCurrentHealth = (float)player.playerCurrentHealth;
         float tempMaxHealth = (float)player.playerMaxHealth * 1.0f;
         float hFraction = tempCurrentHealth / tempMaxHealth;
         if (fillB > hFraction)
         {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
         }


         //heal
         if (fillF < hFraction)
         {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
         }
         healthText.text = Mathf.Round(player.playerCurrentHealth) + "/" + Mathf.Round(player.playerMaxHealth);
    }

    public void TakeDamage(int damage)
    {
          player.playerCurrentHealth -= damage;
          lerpTimer =0f;
    }

    public void RestoreHealth(int healAmount)
    {
        if (player.playerCurrentHealth == player.playerMaxHealth)
        {
            return;
        }
        if (player.playerCurrentHealth < player.playerMaxHealth)
        player.playerCurrentHealth += healAmount;
        lerpTimer = 0f;
    }

    public void IncreaseHealth(int playerLevel)
    {

        CmdIncreaseHealth(playerLevel);


    }
    
    [Command(requiresAuthority = false)]
    public void CmdIncreaseHealth(int playerLevel)
    {  
        player.playerMaxHealth += healthIncrement;
        player.playerCurrentHealth = player.playerMaxHealth;
        Debug.Log("Setting players max health to - " + player.playerMaxHealth);
    }
}
