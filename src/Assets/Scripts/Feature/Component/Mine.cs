using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        private bool accelation = true;
        private Collider collider;
        private float count;
        private float distance;
        private bool playerRightSide;
        private Transform playerTransform;
        private Rigidbody rb;

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

        private void FixedUpdate()
        {
            if (accelation)
            {
                if (playerRightSide)
                {
                    rb.AddForce(bossPrams.mineSpeedVertical, bossPrams.mineSpeedBeside, 0);
                }
                else
                {
                    rb.AddForce(-bossPrams.mineSpeedVertical, bossPrams.mineSpeedBeside, 0);
                }

                count++;
            }

            if (count > 10 && accelation)
            {
                accelation = false;
                collider.isTrigger = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.mineDamage, transform.position, transform);
                gameObject.GetComponent<ISwappable>().Delete();
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("enemy pushed");
                other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.mineDamage, transform.position, transform);
                gameObject.GetComponent<ISwappable>().Delete();
            }
        }
    }
}