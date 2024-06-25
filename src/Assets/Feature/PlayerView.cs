using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public delegate void InputHandler();
    public delegate void MouseInputHandler(Vector3 mousePosition);

    public event InputHandler OnJump;
    public event InputHandler OnSlowTimeStart;
    public event InputHandler OnSlowTimeEnd;
    public event MouseInputHandler OnSwapPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJump?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnSlowTimeStart?.Invoke();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            OnSlowTimeEnd?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            OnSwapPosition?.Invoke(mousePos);
        }
    }
}
