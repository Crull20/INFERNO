using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)] // run before most game logic
public class PlayerInputReader : MonoBehaviour
{
    [Header("Events pushed to the Agent")]
    public UnityEvent<Vector2> OnMovementInput;
    public UnityEvent<Vector2> OnPointerInput;
    public UnityEvent OnAttack;

    [Header("Input Actions (New Input System)")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference attack;
    [SerializeField] private InputActionReference pointerPosition;

    [Header("Pointer conversion to world space")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private Transform pointerDepthTarget; // usually the Agent transform

    private void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
        if (pointerDepthTarget == null) pointerDepthTarget = transform;
    }

    private void OnEnable()
    {
        movement?.action?.Enable();
        pointerPosition?.action?.Enable();
        attack?.action?.Enable();

        if (attack != null && attack.action != null)
            attack.action.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        if (attack != null && attack.action != null)
            attack.action.performed -= OnAttackPerformed;

        movement?.action?.Disable();
        pointerPosition?.action?.Disable();
        attack?.action?.Disable();
    }

    private void Update()
    {
        // Movement (Vector2)
        if (movement != null && movement.action != null)
        {
            var move = movement.action.ReadValue<Vector2>();
            OnMovementInput?.Invoke(move);
        }

        // Pointer -> world position
        var screen = ReadPointerScreen();
        if (screen.HasValue && worldCamera != null)
        {
            float z = worldCamera.WorldToScreenPoint(pointerDepthTarget.position).z;
            Vector3 world = worldCamera.ScreenToWorldPoint(new Vector3(screen.Value.x, screen.Value.y, z));
            OnPointerInput?.Invoke((Vector2)world);
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx) => OnAttack?.Invoke();

    private Vector2? ReadPointerScreen()
    {
        if (pointerPosition != null && pointerPosition.action != null && pointerPosition.action.enabled)
            return pointerPosition.action.ReadValue<Vector2>();

        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        return null;
    }
}
