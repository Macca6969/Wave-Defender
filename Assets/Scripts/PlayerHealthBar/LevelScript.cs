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
    [SerializeField] public AudioScript audioScript;

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
        frontXpBar.fillAmount = player.playerCurrentXp / player.playerRequiredXp;
        backXpBar.fillAmount = player.playerCurrentXp / player.playerRequiredXp;
        player.playerRequiredXp = CalculateplayerRequiredXp();
        levelText.text = player.playerCurrentLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();
        if (Input.GetKeyDown(KeyCode.M) && isLocalPlayer)
        {
            GainExperienceScalable(20, player.playerCurrentLevel);
        }


        if (player.playerCurrentXp > player.playerRequiredXp && isLocalPlayer)
        {
            LevelUp();
        }

    }

    [ClientRpc]
    public void RpcPlayerGainExperience(int enemyXp, string playerName)
    {
        player.playerCurrentXp += enemyXp;
        Debug.Log(gameObject.name + " has gained " + enemyXp + " xp.");

        if (gameObject.name == playerName)
        {
            int bonusXp = (enemyXp / 10);
            player.playerCurrentXp += bonusXp;
            Debug.Log(gameObject.name + " has recieved the bonus xp.");

        }

    }

    public void UpdateXpUI()
    {
        float xpFraction = player.playerCurrentXp / player.playerRequiredXp;
        float FXP = frontXpBar.fillAmount;

        if (FXP < xpFraction)
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
        xpText.text = player.playerCurrentXp + "/" + player.playerRequiredXp;
    }

    public void GainExperienceFlatRate(float xpGained)
    {
        player.playerCurrentXp += xpGained;
        lerpTimer = 0f;
    }

    public void LevelUp()
    {
        CmdLevelUp();
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        player.playerCurrentXp = Mathf.RoundToInt(player.playerCurrentXp - player.playerRequiredXp);
        //alter attributes here
        GetComponent<PlayerHealth>().IncreaseHealth(player.playerCurrentLevel);
        player.playerRequiredXp = CalculateplayerRequiredXp();
        levelText.text = player.playerCurrentLevel.ToString();
        audioScript.playerLevelUp.Play();
    }

    [Command(requiresAuthority = false)]
    public void CmdLevelUp()
    {
        player.playerCurrentLevel++;

    }

    private int CalculateplayerRequiredXp()
    {
        int solveForplayerRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= player.playerCurrentLevel; levelCycle++)
        {
            solveForplayerRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForplayerRequiredXp / 4;
    }

    public void GainExperienceScalable(float xpGained, int passedLevel)
    {
        if (passedLevel < player.playerCurrentLevel)
        {
            float multiplier = 1 + (player.playerCurrentLevel - passedLevel) * 0.1f;
            player.playerCurrentXp += xpGained * multiplier;
        }
        else
        {
            player.playerCurrentXp += xpGained;
        }
        lerpTimer = 0f;
        delayTimer = 0f;
    }
}
