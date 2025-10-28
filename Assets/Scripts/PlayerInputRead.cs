using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputRead : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;
    public UnityEvent OnAttack;

    [SerializeField]
    private InputActionReference movement, attack, pointerPosition;

    [SerializeField] private Camera worldCamera;
    [SerializeField] private Transform pointerDepthTarget;


    private void Update()
    {
        OnMovementInput?.Invoke(movement.action.ReadValue<Vector2>().normalized);
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = 
    }
}
