using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrailSanity : MonoBehaviour
{
    public TrailRenderer tr;
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            tr.emitting = true;
            tr.Clear();
            StartCoroutine(StopSoon());
        }
    }
    private System.Collections.IEnumerator StopSoon()
    {
        yield return new WaitForSeconds(0.3f);
        tr.emitting = false;
    }
}