using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// This component can be used together with the UnityEventOnTrigger-component to make a character in your game have health
// If you don't know how to use UnityEvents, watch this video first: https://play.his.se/media/UnityEvents/0_nq9m8qin
// Call the method TakeDamage or Heal from this script from other events (such as On Trigger Enter in the UnityEventOnTrigger-component), and set the number to the damage you want to apply (or the health you want to gain)
// Add events to decide what happens when the character is damaged (OnDamage), when object can be damaged again (OnExitSafeTime), when health reaches zero (OnDie) or when the character is Healed (OnHealed)
public class Health : MonoBehaviour
{
    [Tooltip("Maximum amount of health")]
    [SerializeField] float maxHealth = 10f;
    [Tooltip("Minimum amount of health")]
    [SerializeField] float minHealth = 0f;

    [Tooltip("Will not take damage when invincible")]
    [SerializeField] bool invincible;

    [Tooltip("How long, after the damage is applied, will the object be invulnerable?")]
    [SerializeField] private float safeTime = 1f;
    
    private float currentHealth;
    private float invulnerabilityTimer;
    private bool isHurt = false;
    
    [Tooltip("Event triggered when this object takes damage")]
    [SerializeField] UnityEvent OnDamaged;
    [Tooltip("Event triggered when this object exits 'safe time' after damage")]
    [SerializeField] UnityEvent OnExitSafeTime;
    [Tooltip("Event triggered when this object is healed")]
    [SerializeField] UnityEvent OnHealed;
    [Tooltip("Event triggered when this object dies (health is zero)")]
    [SerializeField] UnityEvent OnDie;

    

    // Awake is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        invulnerabilityTimer = safeTime;
    }

    private void Update()
    {
        // If the isHurt-variable is set to "true", start counting down time from the set "safeTime"
        
        if (isHurt == true)
        {
            invulnerabilityTimer -= Time.deltaTime;
            
            // Trigger the UnityEvent "OnExitSafeTime" when timer reached 0 or lower. Reset variables.
            if (invulnerabilityTimer <= 0f)
            {
                isHurt = false;
                invulnerabilityTimer = safeTime;
                OnExitSafeTime?.Invoke();
            }
        }
    }

    public void Heal(float healAmount)
    {
        //only add health if it's currently less than max
        if (currentHealth < maxHealth)
        {
            //add healAmount to current health, adjust if health ends up outside of allowed range
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
            
            // call OnHealed action
            OnHealed?.Invoke();
        }
    }


    public void TakeDamage(float damage)
    {
        //only take damage if not invincible or dead
        if (invincible == false && currentHealth > minHealth && isHurt == false)
        {
            isHurt = true;
            //apply damage, adjust if health ends up outside of allowed range
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth); 
        
            Debug.Log(currentHealth);
            // call OnDamage action
            OnDamaged?.Invoke();
        
            HandleDeath();
        }
    }

    public void Kill()
    {
        currentHealth = minHealth;

        // call OnDamage action
        OnDamaged?.Invoke();
        
        HandleDeath();
    }

    void HandleDeath()
    {
        // call OnDie action if no health
        if (currentHealth <= minHealth)
        {
            OnDie?.Invoke();
            //Add animation-trigger here
        }
    }
}

