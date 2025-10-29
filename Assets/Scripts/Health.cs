using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // current/max health value
    [SerializeField]
    private int currentHealth, maxHealth;

    // fired on hit and deahth passing attacker GameObject
    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    // flag for processing damage
    [SerializeField]
    public bool isDead = false;

    // set starting health 
    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;

    }

    // damage from sender
    public void GetHit(int amount, GameObject sender)
    {
        // ignore if dead or same layer
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        // subtract health
        currentHealth -= amount;

        // still alive>> fire hit event
        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);

        }
        // no health, destroy object
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            Destroy(gameObject);
        }
    }
}