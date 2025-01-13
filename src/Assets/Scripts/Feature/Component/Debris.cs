using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class Debris : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        [SerializeField] private float splashSpeed = 200;
        private bool accelation;
        private int count;
        private Rigidbody debrisRb;
        private float distance;
        private bool playerRightSide;
        private Transform playerTransform;
        private bool splash = true;

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
                    debrisRb.AddForce(bossPrams.debrisSpeed, 0, 0);
                }
                else
                {
                    debrisRb.AddForce(-bossPrams.debrisSpeed, 0, 0);
                }

                count++;
            }
            else if (splash)
            {
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                if (playerRightSide)
                {
                    debrisRb.AddForce(splashSpeed, splashSpeed, 0);
                }
                else
                {
                    debrisRb.AddForce(-splashSpeed, splashSpeed, 0);
                }

                splash = false;
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
                other.gameObject.GetComponent<IDamaged>()
                    .OnDamage(bossPrams.debrisDamage, transform.position, transform);
                gameObject.GetComponent<ISwappable>().Delete();
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IDamaged>()
                    .OnDamage(bossPrams.debrisDamage, transform.position, transform);
                gameObject.GetComponent<ISwappable>().Delete();
            }

            if (other.gameObject.CompareTag("Ground"))
            {
                Destroy(this);
            }
        }

        public void Kick()
        {
            accelation = true;
            splash = false;
        }
    }
}