using UnityEngine;
using UnityEngine.InputSystem;

public class MobilePlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float moveSpeed = 3f;

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

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}