using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.Windows;


public class Agent : MonoBehaviour
{
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float moveSmoothTime = 0.1f;
    [SerializeField] private WeaponParent weaponParent;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.3f;
    [SerializeField] private TrailRenderer myTrailRenderer;

    // audio
    [SerializeField] private AudioSource dashSfx;
    [SerializeField] AudioClip dashClip;


    // input and smoothign
    private Rigidbody2D rigidBody;
    private SpriteRenderer sprite;
    private Vector2 moveInput;
    private Vector2 smoothMovement;
    private Vector2 smoothVel;

    private bool isDashing;
    private bool canDash = true;


    // pointer world position
    private Vector2 _pointerWorld;
    public Vector2 PointerInput => _pointerWorld;

    
    private PlayerInputReader _input;

    // cache components
    // find weaponParent
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

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
            _input.OnDash.AddListener(HandleDash);
        }
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnMovementInput.RemoveListener(HandleMove);
            _input.OnPointerInput.RemoveListener(HandlePointer);
            _input.OnAttack.RemoveListener(HandleAttack);
            _input.OnDash.RemoveListener(HandleDash);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        smoothMovement = Vector2.SmoothDamp(smoothMovement, moveInput, ref smoothVel, moveSmoothTime);
        rigidBody.velocity = smoothMovement * _speed;
    }

    // input handlers
    private void HandleMove(Vector2 input) => moveInput = input;

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

    private void HandleDash()
    {
        if (!isDashing && canDash)
            StartCoroutine(DashRoutine());
    }

    private void ApplyVelocity()
    {
        smoothMovement = Vector2.SmoothDamp(
            smoothMovement,
            moveInput,
            ref smoothVel,
            0.1f);

        rigidBody.velocity = smoothMovement * _speed;
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
    private IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        if (dashSfx && dashClip)
        {
            dashSfx.PlayOneShot(dashClip);
        }

        // pick a dash direction
        Vector2 dir = moveInput.sqrMagnitude > 0.0001f
            ? moveInput.normalized
            : new Vector2((sprite && sprite.flipX) ? -1f : 1f, 0f);

        if (myTrailRenderer)
        {
            myTrailRenderer.emitting = true;
            myTrailRenderer.Clear();
        }


        float elapsed = 0f;
        // apply burst velocity for the duration
        while (elapsed < dashDuration)
        {
            rigidBody.velocity = dir * dashSpeed;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (myTrailRenderer) myTrailRenderer.emitting = false;

        isDashing = false;

        // small cooldown before next dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
