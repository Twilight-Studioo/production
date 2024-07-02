using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;

    private PlayerPresenter presenter;

    void Start()
    {  
        CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
        PlayerView view = player.GetComponent<PlayerView>();
        PlayerModel model = new PlayerModel(capsuleCollider, moveSpeed, jumpForce, groundLayer);
        presenter = new PlayerPresenter(model, view);

    }

    void Update()
    {
        if (presenter != null)
        {
            presenter.Update();
        }
    }
}
