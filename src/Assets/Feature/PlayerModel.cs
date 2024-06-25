using UnityEngine;

public class PlayerModel
{
    public float MoveSpeed { get; set; }
    public float JumpForce { get; set; }
    public LayerMask GroundLayer { get; set; }
    public Rigidbody Rigidbody { get; private set; }
    public CapsuleCollider CapsuleCollider { get; private set; }

    public PlayerModel(Rigidbody rb, CapsuleCollider capsuleCollider, float moveSpeed, float jumpForce, LayerMask groundLayer)
    {
        Rigidbody = rb;
        CapsuleCollider = capsuleCollider;
        MoveSpeed = moveSpeed;
        JumpForce = jumpForce;
        GroundLayer = groundLayer;
    }

    public bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(CapsuleCollider.bounds.center.x, CapsuleCollider.bounds.min.y, CapsuleCollider.bounds.center.z);
        return Physics.CheckCapsule(CapsuleCollider.bounds.center, capsuleBottom, CapsuleCollider.radius * 0.9f, GroundLayer);
    }

    public void Move(float moveInputX, float moveInputZ)
    {
        Vector3 movement = new Vector3(moveInputX, 0, moveInputZ) * MoveSpeed;
        Rigidbody.velocity = new Vector3(movement.x, Rigidbody.velocity.y, movement.z);
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpForce, Rigidbody.velocity.z);
        }
    }

    public void SwapPositionWith(Transform target)
    {
        Vector3 tempPosition = Rigidbody.position;
        Rigidbody.position = target.position;
        target.position = tempPosition;
    }
}