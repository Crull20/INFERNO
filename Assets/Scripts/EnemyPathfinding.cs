using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private SpriteRenderer sprite;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;
    private const float FaceThreshold = 0.01f;

    private void Awake()
    {
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (knockback != null && knockback.gettingKnockedBack)
        {
            return;
        }
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        // flip sprite on horizontal movement
        if (sprite && Mathf.Abs(moveDir.x) > FaceThreshold)
        {
            // flip going left
            sprite.flipX = moveDir.x < 0f;
        }
    }

    public void MoveTo(Vector2 direction)
    {
        moveDir = direction;
    }

    // move towards a world position
    public void MoveTowards(Vector2 targetWorldPos)
    {
        Vector2 dir = (targetWorldPos - rb.position).normalized;
        moveDir = dir;
    }

    public void Stop()
    {
        moveDir = Vector2.zero;
    }
}
