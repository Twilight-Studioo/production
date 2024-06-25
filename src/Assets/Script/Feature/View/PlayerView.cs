using System;
using UnityEngine;

namespace Script.Feature.View
{
    
    public class PlayerView : MonoBehaviour
    {
        private Rigidbody rb;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void AddPower(Vector2 moveDirection)
        {
            rb.AddForce(moveDirection);
        }
    }
}