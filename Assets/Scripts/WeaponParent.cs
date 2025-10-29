using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponParent : MonoBehaviour
{
    // world space position weapon aims at
    public Vector2 PointerPosition { get; set; }

    [SerializeField] private Animator animator;
    [SerializeField] private float delay = 0.3f;
    
    private bool attackBlocked;

    // true when attack animation is playing
    public bool IsAttacking { get; private set;}

    public Transform circleOrigin;
    public float radius;

    // called by animation event at end of attack
    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }
    private void Update()
    {
        // do not rotate while attacking
        if (IsAttacking)
            return;
        Vector2 toPointer = (PointerPosition - (Vector2)transform.position);
        if (toPointer.sqrMagnitude > 0.0001f)
        {
            // rotate weapon so its +X faces the pointer
            transform.right = toPointer.normalized;

            // Flip sprite vertically to avoid upside-down art when aiming left/right
            Vector3 scale = transform.localScale;
            scale.y = (toPointer.x < 0f) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            transform.localScale = scale;
        }
    }

    
    public void Attack()
    {
        if (attackBlocked)
        {
            return;
        }

        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

   
    private IEnumerator DelayAttack()
    {
        // wait for new attack
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    { 
        // visualize the hit circle in editor
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    // called by an animation event at impact frame
    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            Debug.Log(collider.name);
            Health health;
            if (health = collider.GetComponent<Health>())
            {
                health.GetHit(1, transform.parent.gameObject);
            }
        }
    }
}
