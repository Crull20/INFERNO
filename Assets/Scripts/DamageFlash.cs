using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    // holds sprites to tint
    [SerializeField] private Transform spritesRoot;

    // flash look/timing
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 1f);
    // flash length
    [SerializeField] private float flashDuration = 0.20f;
    // intensity over time
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0); 

    // trigger from Health event
    [SerializeField] private bool flashOnHit = true;
    [SerializeField] private bool flashOnDeath = true;

    // caching the sprite renders and original colors
    private readonly List<SpriteRenderer> _sprites = new();
    private readonly List<Color> _originalColors = new();

    // Coroutine handle and health reference
    private Coroutine _flash;
    private Health _health;

    private void Awake()
    {
        Transform root = spritesRoot ? spritesRoot : transform;
        root.GetComponentsInChildren(true, _sprites);

        // cache original colors to restore after flash finishes
        _originalColors.Clear();
        foreach (var sr in _sprites) _originalColors.Add(sr.color);

        // find health component for flash on hit/death
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            if (flashOnHit) _health.OnHitWithReference.AddListener(OnHitEvent);
            if (flashOnDeath) _health.OnDeathWithReference.AddListener(OnHitEvent);
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            if (flashOnHit) _health.OnHitWithReference.RemoveListener(OnHitEvent);
            if (flashOnDeath) _health.OnDeathWithReference.RemoveListener(OnHitEvent);
        }
    }

    // Health event to start flash
    private void OnHitEvent(GameObject _sender) => Flash();

    /// <summary>Call this to start a flash (you can also call from an Animation Event).</summary>
    public void Flash()
    {
        // restart red flash if one is running
        if (_flash != null) StopCoroutine(_flash);
        _flash = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;

        // instant set to flash color (peak)
        for (int i = 0; i < _sprites.Count; i++)
            if (_sprites[i]) _sprites[i].color = flashColor;

        while (t < flashDuration)
        {
            // use fadecurv to lerp from flashColor to original
            float k = Mathf.Clamp01(t / flashDuration);
            float w = fadeCurve.Evaluate(k); 

            for (int i = 0; i < _sprites.Count; i++)
            {
                var sr = _sprites[i];
                if (sr) sr.color = Color.Lerp(_originalColors[i], flashColor, w);
            }

            t += Time.deltaTime;
            yield return null;
        }

        // restore original colors
        for (int i = 0; i < _sprites.Count; i++)
            if (_sprites[i]) _sprites[i].color = _originalColors[i];

        _flash = null;
    }
}