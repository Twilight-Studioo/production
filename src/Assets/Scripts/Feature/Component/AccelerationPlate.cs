using System;
using UnityEngine;

namespace Feature.Component
{
    public class AccelerationPlate : MonoBehaviour
    {
        [SerializeField] private float acceleration = 0.5f;
        private bool OnAccelerationPlate = false;

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnAccelerationPlate = true;
                other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(acceleration,0,0),ForceMode.VelocityChange);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                OnAccelerationPlate = false;
            }
        }
    }
}