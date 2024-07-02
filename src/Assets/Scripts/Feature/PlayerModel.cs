using UnityEngine;

public class PlayerModel
{
    public float MoveSpeed { get; set; }
    public float JumpForce { get; set; }
    public LayerMask GroundLayer { get; set; }
    public CapsuleCollider CapsuleCollider { get; private set; }

    public PlayerModel(CapsuleCollider capsuleCollider, float moveSpeed, float jumpForce, LayerMask groundLayer)
    {
        CapsuleCollider = capsuleCollider;
        MoveSpeed = moveSpeed;
        JumpForce = jumpForce;
        GroundLayer = groundLayer;
    }

    public bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(CapsuleCollider.bounds.center.x, CapsuleCollider.bounds.min.y, CapsuleCollider.bounds.center.z);
        bool grounded = Physics.CheckCapsule(CapsuleCollider.bounds.center, capsuleBottom, CapsuleCollider.radius * 0.9f, GroundLayer);
        return grounded;
    }
}
