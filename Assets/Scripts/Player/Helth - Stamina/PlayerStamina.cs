using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("References")]
    public StaminaBarScript staminaBar;
    public PlayerMove playerMove;
    public DashScript dashScript;

    [Header("Variables")]
    public int maxStamina = 100;
    public int currentStamina;
    [Space]
    public int staminaRegen = 1;
    public int staminaLossRun = 2;
    public int staminaLossJump = 5;
    public int staminaLossDash = 15;
    [Space]
    public bool canRunStam;
    public bool canJumpStam;
    public bool canDashStam;
    [Space]
    public bool isUsingStam;
    public bool didUseStamina;
    public bool canRegenStamina = true;
    public bool tset;
    [Space]
    float timeBetweenStaminaLoss = 0.2f;
    float lastStaminaLoss;
    public float timeBetweenChill = 0.7f;
    public float lastChill;

    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    // Update is called once per frame
    void Update()
    {
        ControllStamina();
        CanUseAbilityTest();
        StaminaChillTime();
    }

    // Controll the Time between stamina loss
    void ControllStamina()
    {
        // Clamp
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Timer
        if (Time.time - lastStaminaLoss < timeBetweenStaminaLoss)
        {
            return;
        }
        lastStaminaLoss = Time.time;

        // Loose Stamina
        if (playerMove.moveState == PlayerMove.MovmentStates.sprinting && canRunStam && playerMove.hasMovmant)
        {
            LooseStamina(staminaLossRun);
            didUseStamina = true;
            isUsingStam = true;
        }
        else
            isUsingStam = false;

        if (canRegenStamina)
        {
            GainStamina(staminaRegen);
        }

    }

    // Not instaint regeneration
    void StaminaChillTime()
    {
        if (didUseStamina)
        {
            canRegenStamina = false;

            if (Time.time - lastChill < timeBetweenChill)
            {
                return;
            }
            lastChill = Time.time;
            
            if (!isUsingStam)
            {
                didUseStamina = false;
                canRegenStamina = true;
            }
        }

    }

    // Cannot Jump, Run, Dash if stamina to low
    public void CanUseAbilityTest()
    {
        // for Running
        if (currentStamina <= staminaLossRun)
            canRunStam = false;
        else
            canRunStam = true;

        // for Jumping
        if (currentStamina < staminaLossJump)
            canJumpStam = false;
        else
            canJumpStam = true;

        // for Dahsing
        if (currentStamina < staminaLossDash)
            canDashStam = false;
        else
            canDashStam = true;
    }

    // Jumps to loose Stamina
    public void JumpToLooseStamina()
    {
        LooseStamina(staminaLossJump);
    }

    // Dashes to loose Stamina
    public void DashesToLooseStamina()
    {
        LooseStamina(staminaLossDash);
    }

    // loose stamina
    void LooseStamina(int staminaloss)
    {
        currentStamina -= staminaloss;

        staminaBar.SetStamina(currentStamina);
    }

    // regenerate stamina
    void GainStamina(int staminaGain) 
    {
        currentStamina += staminaGain;

        staminaBar.SetStamina(currentStamina);
    }
}
