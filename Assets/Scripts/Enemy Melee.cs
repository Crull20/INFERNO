using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    // hitbox
    [SerializeField] private Transform attackPoint;      // assign the child
    [SerializeField] private float hitRadius = 0.55f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask targetLayers;     // set to Player
    [SerializeField] private string targetTag = "Player";

    // sound
    [SerializeField] private AudioSource sfx;         
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip attackHit;
    [SerializeField] private Vector2 pitchJitter = new(0.96f, 1.04f);

    private void Awake()
    {
        // find audioSource
        if (!sfx) sfx = GetComponent<AudioSource>();
    }

    // called by anmation event on swing frame
    public void Anim_PlayAttackSound()
    {
        if (sfx && attackSound)
        {
            sfx.pitch = Random.Range(pitchJitter.x, pitchJitter.y);
            sfx.PlayOneShot(attackSound);
        }
    }

    // called on impact frame
    public void Anim_DoDamage()
    {
        if (!attackPoint) attackPoint = transform;

        var hits = Physics2D.OverlapCircleAll(attackPoint.position, hitRadius, targetLayers);
        foreach (var h in hits)
        {
            if (!string.IsNullOrEmpty(targetTag) && !h.CompareTag(targetTag)) continue;
            if (h.gameObject.layer == gameObject.layer) continue; // same-layer rule

            // find targets inside hit circle
            var hp = h.GetComponentInParent<Health>();
            if (hp != null && !hp.isDead)
                hp.GetHit(damage, this.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // visualize hit circle in editor
        if (!attackPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, hitRadius);
    }
}