/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField] private WeatherController weatherController;
    [SerializeField] private Image staminaBar;
    private PlayerMovement playerMovement;
    private AnimationStateController animStateCntrlr;

    public float playerStamina = 100.0f;
    public float staminaRegenSec = 2.0f;
    public float staminaConsumptionSec = 2.0f;
    public float staminaRegenDelay = 2.0f;

    public bool resting = true;

    [SerializeField] private const float MAXSTAMINA = 100.0f;
    [SerializeField] private float staminaRegenTimer = 2.0f;
    private float currentStaminaRegenTimer;
    private float currentStaminaConsumptionSec;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = this.GetComponent<PlayerMovement>();
        animStateCntrlr = this.GetComponent<AnimationStateController>();
        currentStaminaRegenTimer = staminaRegenTimer;
        currentStaminaConsumptionSec = staminaConsumptionSec;
    }

    // Update is called once per frame
    void Update()
    {
        //check if player moving against wind or not
        Vector2 windDir = weatherController.wind.currentWindDir;
        float dotP = Vector3.Dot(playerMovement.movement, new Vector3(windDir.x, 0, windDir.y));
        
        if ((playerMovement.movement.magnitude > 0 && animStateCntrlr.IsCarryingSnow())|| animStateCntrlr.IsPlayerDigging())
        {
            resting = false;
        }
        else
        {
            resting = true;
        }

        if(dotP < 0)
        {
            currentStaminaConsumptionSec = 1.5f * staminaConsumptionSec;
        }
        else
        {
            currentStaminaConsumptionSec = staminaConsumptionSec;
        }

        //regenerate stamina after a brief delay of player not consuming stamina
        if (playerStamina <= MAXSTAMINA && resting)
        {
            currentStaminaRegenTimer -= Time.deltaTime;

            if (currentStaminaRegenTimer <= 0)
            {
                playerStamina += staminaRegenSec * Time.deltaTime;

            }
        }
        else if (!resting && playerStamina >= 0) //consume stamina
        {
            currentStaminaRegenTimer = staminaRegenTimer;
            playerStamina -= currentStaminaConsumptionSec * Time.deltaTime;
        }

        staminaBar.fillAmount = playerStamina / MAXSTAMINA;
    }
}
