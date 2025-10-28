using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponParent : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 PointerPosition { get; set; }

    [SerializeField] private Animator animator;
    [SerializeField] private float delay = 0.3f;
    
    private bool attackBlocked;

    public bool IsAttacking { get; private set;}

    public Transform circleOrigin;
    public float radius;

    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }
    private void Update()
    {
        if (IsAttacking)
            return;
        Vector2 toPointer = (PointerPosition - (Vector2)transform.position);
        if (toPointer.sqrMagnitude > 0.0001f)
        {
            // Rotate weapon so its +X faces the pointer
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
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

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
