using System;
using UnityEngine;

namespace Feature.Component
{
    public class AccelerationPlate : MonoBehaviour
    {
        [SerializeField] private float acceleration = 0.5f;
        [SerializeField] private float escapeSpeed = 20f;
        private Vector3 DirectionMovement = new Vector3(1, 0, 0);

        private void OnCollisionStay(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Rigidbody>().MovePosition(other.transform.position + DirectionMovement * acceleration * Time.deltaTime);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            //if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce(DirectionMovement * escapeSpeed);
            }
        }
    }
}