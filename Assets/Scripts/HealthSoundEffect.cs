using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSoundEffect : MonoBehaviour
{
    [SerializeField] private AudioSource source;          // assign a 2D SFX source
    // sound banks
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip[] deathClips;

    // reference to health on object
    private Health health;

    private void Awake()
    {
        // cache components 
        health = GetComponent<Health>();
        if (!source) source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // subscribe to Health events
        health.OnHitWithReference.AddListener(_ => Play(hurtClips));
        health.OnDeathWithReference.AddListener(_ => Play(deathClips));
    }

    private void OnDisable()
    {
        // unsubscribe from Health events
        health.OnHitWithReference.RemoveListener(_ => Play(hurtClips));
        health.OnDeathWithReference.RemoveListener(_ => Play(deathClips));
    }

    private void Play(AudioClip[] bank)
    {
        if (!source || bank == null || bank.Length == 0) return;
        var clip = bank[Random.Range(0, bank.Length)];
        source.PlayOneShot(clip);
    }
}