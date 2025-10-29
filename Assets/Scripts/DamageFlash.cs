using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DamageFlash : MonoBehaviour
{
    [Header("What to tint")]
    [Tooltip("Leave empty to search on this object. Assign VisualRoot to only tint visuals.")]
    [SerializeField] private Transform spritesRoot;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float flashDuration = 0.12f;   // seconds
    [SerializeField]
    private AnimationCurve fadeCurve =
        AnimationCurve.EaseInOut(0, 1, 1, 0); // 1 -> 0 over duration

    [Header("Auto-hook to Health")]
    [SerializeField] private bool flashOnHit = true;
    [SerializeField] private bool flashOnDeath = true;

    private readonly List<SpriteRenderer> _sprites = new();
    private readonly List<Color> _originalColors = new();
    private Coroutine _flashCo;
    private Health _health;

    private void Awake()
    {
        // Collect SpriteRenderers (on spritesRoot or on this object + children)
        Transform root = spritesRoot ? spritesRoot : transform;
        root.GetComponentsInChildren(true, _sprites);

        if (_sprites.Count == 0)
            Debug.LogWarning($"{name}: DamageFlash found no SpriteRenderers to tint.");

        _originalColors.Clear();
        foreach (var sr in _sprites) _originalColors.Add(sr.color);

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

    private void OnHitEvent(GameObject _sender) => Flash();

    /// <summary>Call this to start a flash (you can also call from an Animation Event).</summary>
    public void Flash()
    {
        if (_flashCo != null) StopCoroutine(_flashCo);
        _flashCo = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;

        // instant set to flash color (peak)
        for (int i = 0; i < _sprites.Count; i++)
            if (_sprites[i]) _sprites[i].color = flashColor;

        while (t < flashDuration)
        {
            float k = Mathf.Clamp01(t / flashDuration);
            float w = fadeCurve.Evaluate(k); // 1->0 over time

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

        _flashCo = null;
    }
}