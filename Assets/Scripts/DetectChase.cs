using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class SimpleDetectChase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;          // drag Player here, or leave null to find by tag
    [SerializeField] private string targetTag = "Player";

    [Header("Behavior")]
    [SerializeField] private float detectionRadius = 6f;
    [SerializeField] private float stopDistance = 0.35f;
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer sprite;     // drag your Skeleton SpriteRenderer
    [SerializeField] private Animator animator;         // drag your Animator (on Skeleton)

    private const float FaceThreshold = 0.01f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (!target)
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            if (go) target = go.transform;
            else Debug.LogWarning($"{name}: No object tagged '{targetTag}' found. Enemy will idle.");
        }
    }

    private void FixedUpdate()
    {
        bool isMoving = false;

        if (target)
        {
            Vector2 myPos = rb.position;
            Vector2 targetPos = target.position;
            Vector2 toTarget = targetPos - myPos;
            float dist = toTarget.magnitude;

            // Face towards the player for visuals
            if (sprite && Mathf.Abs(toTarget.x) > FaceThreshold)
                sprite.flipX = (toTarget.x < 0f);

            if (dist <= detectionRadius && dist > stopDistance)
            {
                Vector2 dir = toTarget / Mathf.Max(dist, 0.0001f);
                Vector2 step = dir * (moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(myPos + step);
                isMoving = true;
            }
        }

        if (animator) animator.SetBool("IsMoving", isMoving);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}