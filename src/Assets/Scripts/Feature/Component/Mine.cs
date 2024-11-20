using System;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        private Rigidbody rb;
        private float distance;
        private float count = 0;
        private Transform playerTransform;
        private bool playerRightSide = false;
        private bool accelation = true;
        private Collider collider;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            distance = transform.position.x - playerTransform.position.x;
            if (distance < 0)
            {
                playerRightSide = true;
            }
        }

        private void Update()
        {
            if (accelation)
            {
                if (playerRightSide)
                {
                    rb.AddForce(bossPrams.mineSpeedVertical,bossPrams.mineSpeedBeside,0);
                }
                else
                {
                    rb.AddForce(-bossPrams.mineSpeedVertical,bossPrams.mineSpeedBeside,0);
                }
                count++;
            }

            if (count > 10 && accelation)
            {
                accelation = false;
                collider.isTrigger = false;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.mineDamage,transform.position,transform);
                Destroy(this.gameObject);
            }
        }
    }
}