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
        Rigidbody rb = player.GetComponent<Rigidbody>();
        CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
        PlayerModel model = new PlayerModel(rb, capsuleCollider, moveSpeed, jumpForce, groundLayer);
        PlayerView view = player.GetComponent<PlayerView>();

        presenter = new PlayerPresenter(model, view);
    }

    void Update()
    {
        presenter.Update();
    }
}
