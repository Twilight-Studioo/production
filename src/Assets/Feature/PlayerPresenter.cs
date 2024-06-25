using UnityEngine;

public class PlayerPresenter
{
    private PlayerModel model;
    private PlayerView view;
    private bool isSlowingTime = false;

    public PlayerPresenter(PlayerModel model, PlayerView view)
    {
        this.model = model;
        this.view = view;

        view.OnJump += HandleJump;
        view.OnSlowTimeStart += HandleSlowTimeStart;
        view.OnSlowTimeEnd += HandleSlowTimeEnd;
        view.OnSwapPosition += HandleSwapPosition;
    }

    public void Update()
    {
        float moveInputX = Input.GetAxis("Horizontal");
        float moveInputZ = Input.GetAxis("Vertical");
        model.Move(moveInputX, moveInputZ);
    }

    private void HandleJump()
    {
        model.Jump();
    }

    private void HandleSlowTimeStart()
    {
        Time.timeScale = 0.2f;
        isSlowingTime = true;
    }

    private void HandleSlowTimeEnd()
    {
        Time.timeScale = 1f;
        isSlowingTime = false;
    }

    private void HandleSwapPosition(Vector3 mousePosition)
    {
        if (isSlowingTime)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.transform != view.transform)
                {
                    model.SwapPositionWith(hit.collider.transform);
                }
            }
        }
    }
}