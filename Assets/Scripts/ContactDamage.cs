using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class ContactDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 0.4f;

    [Header("Filtering")]
    [SerializeField] private string targetTag = "Player";   // optional: leave empty to ignore
    [SerializeField] private LayerMask targetLayers = ~0;

    [Header("Animation")]
    [SerializeField] private Animator animator;                 // drag your Animator
    [SerializeField] private string attackTriggerName = "Attack";

    private readonly Dictionary<Health, float> _nextHitTime = new();

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerStay2D(Collider2D other) => TryHit(other);
    private void OnCollisionStay2D(Collision2D c) => TryHit(c.collider);

    private void OnTriggerExit2D(Collider2D other) => Clear(other);
    private void OnCollisionExit2D(Collision2D c) => Clear(c.collider);

    private void TryHit(Collider2D col)
    {
        if (!col) return;

        // layer filter
        if ((targetLayers.value & (1 << col.gameObject.layer)) == 0) return;
        // optional tag filter
        if (!string.IsNullOrEmpty(targetTag) && !col.CompareTag(targetTag)) return;

        var hp = col.GetComponentInParent<Health>();
        if (!hp || hp.isDead) return;

        // your Health.GetHit ignores same-layer hits; ensure Player/Enemy layers differ
        if (col.gameObject.layer == gameObject.layer) return;

        float now = Time.time;
        if (!_nextHitTime.TryGetValue(hp, out float next) || now >= next)
        {
            // 1) Play attack animation (one-shot)
            if (animator && !string.IsNullOrEmpty(attackTriggerName))
                animator.SetTrigger(attackTriggerName);

            // 2) Apply damage
            hp.GetHit(damage, this.gameObject);

            // 3) Start per-target cooldown
            _nextHitTime[hp] = now + damageInterval;
        }
    }

    private void Clear(Collider2D col)
    {
        var hp = col ? col.GetComponentInParent<Health>() : null;
        if (hp != null) _nextHitTime.Remove(hp);
    }
}