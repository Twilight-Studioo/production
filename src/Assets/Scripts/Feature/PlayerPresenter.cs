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
    }

    public void Update()
    {
        float moveInputX = Input.GetAxis("Horizontal");
        float moveInputZ = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveInputX, 0, moveInputZ) * model.MoveSpeed;
        view.Move(movement);

        if (Input.GetKeyDown(KeyCode.Space) && model.IsGrounded())
        {
            view.Jump(model.JumpForce);
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleSlowTimeStart();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            HandleSlowTimeEnd();
        }

        if (isSlowingTime && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            HandleSwapPosition(mousePos);
        }
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
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Item") && hit.collider.transform != view.transform)
            {
                Vector3 tempPosition = view.transform.position;
                view.transform.position = hit.collider.transform.position;
                hit.collider.transform.position = tempPosition;
            }
        }
    }
}
