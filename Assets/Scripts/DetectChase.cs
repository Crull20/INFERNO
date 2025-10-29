using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class SimpleDetectChase : MonoBehaviour
{
    // target set up
    [SerializeField] private Transform target;          
    [SerializeField] private string targetTag = "Player";

    // chase behavior
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float stopDistance = 0.35f;
    [SerializeField] private float moveSpeed = 2.5f;

    // visuals
    [SerializeField] private SpriteRenderer sprite;    
    [SerializeField] private Animator animator;        

    private const float FaceThreshold = 0.01f;
    private Rigidbody2D rb;

    private void Awake()
    {
        // cache RB and set 2D top down defaults
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // auto-find visuals if not assigned
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // auto find target by tag if no assignment
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) 
                target = go.transform;
        }
    }

    private void FixedUpdate()
    {
        bool isMoving = false;

        if (target)
        {
            // vector to target and distance
            Vector2 myPos = rb.position;
            Vector2 targetPos = target.position;
            Vector2 toTarget = targetPos - myPos;
            float dist = toTarget.magnitude;

            // flip sprite toward target
            if (sprite && Mathf.Abs(toTarget.x) > FaceThreshold)
                sprite.flipX = (toTarget.x < 0f);

            // move when inside detection radius
            if (dist <= detectionRadius && dist > stopDistance)
            {
                Vector2 dir = toTarget / Mathf.Max(dist, 0.0001f);
                Vector2 step = dir * (moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(myPos + step);
                isMoving = true;
            }
        }

        // update animator movement state
        if (animator) animator.SetBool("IsMoving", isMoving);
    }

    private void OnDrawGizmosSelected()
    {
        // visualize detection/stop distances
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}