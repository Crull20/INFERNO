using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    // target to attack
    [SerializeField] private Transform target;
    // auto find the target "Player" using tag
    [SerializeField] private string targetTag = "Player";
    // range for attack trigger
    [SerializeField] private float attackRange = 1.2f;
    // time between attacks
    [SerializeField] private float attackCooldown = 1f;
    // attack animation
    [SerializeField] private Animator animator;
    // trigger parameter 4 animator
    [SerializeField] private string attackTriggerName = "Attack";

    private float nextAttack;
    private void Start()
    {
        // finds target by tag if nothing assigned
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) target = go.transform;
        }
        if (!animator)
        {
            animator = GetComponentInChildren<Animator>();
        }

    }
    private void Update()
    {
        // if no target found, do nothing
        if (!target || Time.time < nextAttack) return;

        // if target is inside range, fire attack animation
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackRange)
        {
            animator?.SetTrigger(attackTriggerName);
            nextAttack = Time.time + attackCooldown; // prevents spam
        }
    }
}