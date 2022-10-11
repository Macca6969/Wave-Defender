using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{

    private float health;
    private float lerpTimer;
    public float maxHealth = 100;
    public float levelHealthMultiplier = 2;
    public float chipSpeed = 0.5f;
    public Image frontHealthBar;
    public Image backHealthBar;
    public TextMeshProUGUI healthText;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
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
         float hFraction = health / maxHealth;
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
         healthText.text = Mathf.Round(health) + "/" + Mathf.Round(maxHealth);
    }

    public void TakeDamage(float damage)
    {
          health -= damage;
          lerpTimer =0f;
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    public void IncreaseHealth(int level)
    {
             maxHealth += (health * 0.01f) * ((100 - level) * (levelHealthMultiplier / 10));
             health = maxHealth;
    }
}
