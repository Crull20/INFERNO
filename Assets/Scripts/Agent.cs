using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Agent : MonoBehaviour
{
    [SerializeField] private float _speed = 6f;
    [SerializeField] private WeaponParent weaponParent;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;

    private Vector2 _pointerWorld;
    public Vector2 PointerInput => _pointerWorld;

    // cached reference to an input provider on the same GameObject (e.g., PlayerInputReader)
    private PlayerInputReader _input;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        if (!weaponParent)
            weaponParent = GetComponentInChildren<WeaponParent>(true);
    }

    private void OnEnable()
    {
        _input = GetComponent<PlayerInputReader>();
        if (_input != null)
        {
            _input.OnMovementInput.AddListener(HandleMove);
            _input.OnPointerInput.AddListener(HandlePointer);
            _input.OnAttack.AddListener(HandleAttack);
        }
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnMovementInput.RemoveListener(HandleMove);
            _input.OnPointerInput.RemoveListener(HandlePointer);
            _input.OnAttack.RemoveListener(HandleAttack);
        }
    }

    private void FixedUpdate()
    {
        ApplyVelocity();
    }

    private void HandleMove(Vector2 input) => _movementInput = input;

    private void HandlePointer(Vector2 worldPos)
    {
        _pointerWorld = worldPos;

        if (weaponParent != null)
            weaponParent.PointerPosition = _pointerWorld;

        FlipCharacterX(_pointerWorld.x);
    }

    private void HandleAttack()
    {
        if (weaponParent != null) weaponParent.Attack();
        else Debug.LogWarning($"{name}: weaponParent is null when attacking.");
    }

    private void ApplyVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
            _smoothedMovementInput,
            _movementInput,
            ref _movementInputSmoothVelocity,
            0.1f);

        _rigidbody.velocity = _smoothedMovementInput * _speed;
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
