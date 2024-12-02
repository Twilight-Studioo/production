using System;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class Debris : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        private Transform playerTransform;
        private float distance;
        private Rigidbody debrisRb;
        private bool accelation = true;
        private int count = 0;
        private bool playerRightSide = false;

        private void Start()
        {
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            debrisRb = GetComponent<Rigidbody>();
            distance = transform.position.x - playerTransform.position.x;
            if (distance < 0)
            {
                playerRightSide = true;
            }
        }

        private void FixedUpdate()
        {
            if (accelation)
            {
                if (playerRightSide)
                {
                    debrisRb.AddForce(bossPrams.debrisSpeed,0,0);
                }
                else
                {
                    debrisRb.AddForce(-bossPrams.debrisSpeed,0,0);
                }
                count++;
            }

            if (count > 10 && accelation)
            {
                accelation = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.debrisDamage,transform.position,transform);
                this.gameObject.GetComponent<ISwappable>().Delete();
            }
            else if(other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.debrisDamage,transform.position,transform);
                this.gameObject.GetComponent<ISwappable>().Delete();
            }

            if (other.gameObject.CompareTag("Ground"))
            {
                Destroy(this);
            }
        }
    }
}