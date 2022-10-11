using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LevelScript : NetworkBehaviour
{

    private float lerpTimer;
    private float delayTimer;
    public Player player;

    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;


    [Header("Multipliers")]

    [Range(1f, 300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;



    // Start is called before the first frame update
    void Start()
    {
        frontXpBar.fillAmount = player.currentXp / player.requiredXp;
        backXpBar.fillAmount = player.currentXp / player.requiredXp;
        player.requiredXp = CalculateRequiredXp();
        levelText.text = player.playerCurrentLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();
        if (Input.GetKeyDown(KeyCode.M))
        {
            GainExperienceFlatRate(20);
        }

        if (player.currentXp > player.requiredXp)
        {
            LevelUp();
        }
    }

    public void UpdateXpUI()
    {
        float xpFraction = player.currentXp / player.requiredXp;
        float FXP = frontXpBar.fillAmount;

        if(FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXpBar.fillAmount = xpFraction;
            if (delayTimer > 3)
            {
                lerpTimer = Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
            }
        }
        xpText.text = player.currentXp + "/" + player.requiredXp;
    }

    public void GainExperienceFlatRate(float xpGained)
    {
        player.currentXp += xpGained;
        lerpTimer = 0f;
    }

    public void LevelUp()
    {
         CmdLevelUp();
         frontXpBar.fillAmount = 0f;
         backXpBar.fillAmount = 0f;
         player.currentXp = Mathf.RoundToInt(player.currentXp - player.requiredXp);
         //alter attributes here
         GetComponent<PlayerHealth>().IncreaseHealth(player.playerCurrentLevel);
         player.requiredXp = CalculateRequiredXp();
         levelText.text = player.playerCurrentLevel.ToString();
    }

     [Command(requiresAuthority = false)]
    public void CmdLevelUp()
    {
        player.playerCurrentLevel++;

    }

    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= player.playerCurrentLevel; levelCycle++)
        {
              solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }

    public void GainExperienceScalable(float xpGained, int passedLevel)
    {
        if (passedLevel < player.playerCurrentLevel)
        {
            float multiplier = 1 + (player.playerCurrentLevel - passedLevel) * 0.1f;
            player.currentXp += xpGained * multiplier; 
        }
        else
        {
            player.currentXp += xpGained;
        }
        lerpTimer = 0f;
        delayTimer = 0f;
    }
}
