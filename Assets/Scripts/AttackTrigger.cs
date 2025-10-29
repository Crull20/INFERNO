using UnityEngine;

public class SimpleAttackTrigger : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";

    private float _nextReady;

    private void Start()
    {
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) target = go.transform;
        }
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!target || Time.time < _nextReady) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackRange)
        {
            animator?.SetTrigger(attackTriggerName);
            _nextReady = Time.time + attackCooldown; // prevents retrigger spam
        }
    }
}