#region

using UnityEngine;

#endregion

namespace Feature.Component
{
    public class AccelerationPlate : MonoBehaviour
    {
        [SerializeField] private float acceleration = 0.5f;
        [SerializeField] private float escapeSpeed = 20f;
        [SerializeField] private bool moveRight = true;
        private readonly Vector3 directionMovement = new(1, 0, 0);

        private void OnCollisionExit(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            if (moveRight)
            {
                other.gameObject.GetComponent<Rigidbody>()?.AddForce(directionMovement * escapeSpeed);
            }
            else if (!moveRight)
            {
                other.gameObject.GetComponent<Rigidbody>()?.AddForce(-directionMovement * escapeSpeed);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            if (moveRight)
            {
                other.gameObject.GetComponent<Rigidbody>()?
                    .MovePosition(other.transform.position + directionMovement * acceleration * Time.deltaTime);
            }
            else if (!moveRight)
            {
                other.gameObject.GetComponent<Rigidbody>()?
                    .MovePosition(other.transform.position + -directionMovement * acceleration * Time.deltaTime);
            }
        }
    }
}