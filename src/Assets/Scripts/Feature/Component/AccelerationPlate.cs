#region

using UnityEngine;

#endregion

namespace Feature.Component
{
    public class AccelerationPlate : MonoBehaviour
    {
        [SerializeField] private float acceleration = 0.5f;
        [SerializeField] private float escapeSpeed = 20f;
        private readonly Vector3 directionMovement = new(1, 0, 0);

        private void OnCollisionExit(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Rigidbody>()?.AddForce(directionMovement * escapeSpeed);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Rigidbody>()?
                    .MovePosition(other.transform.position + directionMovement * acceleration * Time.deltaTime);
            }
        }
    }
}