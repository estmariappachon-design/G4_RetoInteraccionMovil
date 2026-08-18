using UnityEngine;
using UnityEngine.InputSystem;

public class MobilePlayerMovement : MonoBehaviour
{
    [Header("Referencias de Componentes")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private InputActionReference moveAction;

    [Header("Ajustes de Movimiento")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    private Vector3 velocity;

    private void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null)
            return;

        // Lectura oficial a través del New Input System
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Dirección de movimiento orientada a la rotación del jugador
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Control de gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}