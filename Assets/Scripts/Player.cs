using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private WeaponParent weaponParent; 

    private Rigidbody2D _rigidbody;
    private Camera _cam;

    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;

    // pointer world position
    private Vector2 _pointerWorld;
    public Vector2 PointerInput => _pointerWorld;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cam = Camera.main;

        if (!weaponParent)
        {
            weaponParent = GetComponentInChildren<WeaponParent>(true);
        }
    }

    private void OnEnable()
    {
        if (attack != null && attack.action != null)
        {
            attack.action.Enable();
            attack.action.performed += PerformAttack;
        }
        else
        {
            Debug.LogWarning($"{name}: 'attack' InputActionReference is not assigned.");
        }

        if (pointerPosition != null && pointerPosition.action != null)
        {
            pointerPosition.action.Enable();
        }
        else
        {
            Debug.LogWarning($"{name}: 'pointerPosition' InputActionReference is not assigned.");
        }
    }

    private void OnDisable()
    {
        if (attack != null && attack.action != null)
            attack.action.performed -= PerformAttack;

        if (attack != null && attack.action != null)
            attack.action.Disable();

        if (pointerPosition != null && pointerPosition.action != null)
            pointerPosition.action.Disable();
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        if (weaponParent != null) weaponParent.Attack();
        else Debug.LogWarning($"{name}: weaponParent is null when attacking.");
    }

    private void Update()
    {
        _pointerWorld = GetPointerWorld();
        if (weaponParent != null)
        {
            weaponParent.PointerPosition = _pointerWorld; // send to weapon
        }
        FlipCharacterX(_pointerWorld.x);
        
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();

    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    // responds to changes in direction quickly
                    ref _movementInputSmoothVelocity,
                    0.1f);

        _rigidbody.velocity = _smoothedMovementInput * _speed;
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    private Vector2 GetPointerWorld()
    {
        if (_cam == null) return _pointerWorld;

        Vector2 screenPos;

        if (pointerPosition != null && pointerPosition.action != null && pointerPosition.action.enabled)
            screenPos = pointerPosition.action.ReadValue<Vector2>();

        else if (Mouse.current != null)
            screenPos = Mouse.current.position.ReadValue();
        else
            return _pointerWorld;

        float z = _cam.WorldToScreenPoint(transform.position).z;
        Vector3 world = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
        return (Vector2)world;
    }

    private void FlipCharacterX(float targetX)
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (targetX < transform.position.x);
        }
        else
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(targetX - transform.position.x);
            transform.localScale = s;
        }
    }
}
