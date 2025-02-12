using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamageScript : MonoBehaviour
{
    [Header("References")]
    public PlayerMove playerMove;
    public PlayerHealth playerHealth;

    [Header("Variables")]
    public float timeBetweenFallUpdate = 0.3f;
    [Space]
    float lastFallUpdate;
    int actuallFallDamage;
    public int startFallDamageAt = 10;
    bool wasInAir;
    [Space]
    public int test;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerMove.grounded && playerMove.rb.velocity.y < 0f)
            CalculatingFallTime();

        CalculateFallDamage();
    }

    // Falling and Calculating how long you fell
    void CalculatingFallTime()
    {
        wasInAir = true;

        actuallFallDamage -= (int)playerMove.rb.velocity.y / 20;
    }

    // Calculate Fall Damage
    void CalculateFallDamage()
    {
        if (wasInAir && playerMove.grounded)
        {
            TakeFallDamage();
        }
    }

    // Fall Damage
    void TakeFallDamage()
    {
        if (actuallFallDamage >= startFallDamageAt)
            playerHealth.TakeDamage(actuallFallDamage - startFallDamageAt);

        test = actuallFallDamage - startFallDamageAt;

        actuallFallDamage = 0;

        wasInAir = false;
    }
}
