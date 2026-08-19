using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchGestureReader : MonoBehaviour
{
    [SerializeField] private float swipeThreshold = 90f;
    private Vector2 startPosition;
    private bool tracking;

    private void OnEnable() => EnhancedTouchSupport.Enable();
    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        if (Touch.activeTouches.Count == 0) return;

        var touch = Touch.activeTouches[0];

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            startPosition = touch.screenPosition;
            tracking = true;
        }

        if (tracking && touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            Vector2 delta = touch.screenPosition - startPosition;
            
            if (delta.magnitude >= swipeThreshold)
            {
                Debug.Log("Swipe detectado: " + delta.normalized);
            }
            else
            {
                Debug.Log("Tap detectado");
            }

            tracking = false;
        }
    }
}