using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    public HealthBarScript healthBarScript;

    [Header("Variables")]
    public int maxHealth = 100;
    public int currenthealth;
    public bool isDed;

    // Start is called before the first frame update
    void Start()
    {
        currenthealth = maxHealth;
        healthBarScript.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        currenthealth = Mathf.Clamp(currenthealth, 0, maxHealth);

        Die();
    }

    // Regenerate
    public void Regenerate(int healHealth)
    {
        currenthealth += healHealth;

        healthBarScript.SetHealth(currenthealth);
    }

    // Take damage
    public void TakeDamage(int damage)
    {
        currenthealth = currenthealth - damage;

        healthBarScript.SetHealth(currenthealth);
    }

    // YOU ARE DEAD (IN HEAVY VOICE)
    void Die()
    {
        if (currenthealth <= 0)
            isDed = true;
    }
}
