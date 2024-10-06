using Feature.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Feature.Component.Environment
{
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private uint damege = 5;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")
                || other.gameObject.CompareTag("Enemy"))
            {
                var obj = other.gameObject.GetComponent<IDamaged>();

                obj?.OnDamage(damege, transform.position, transform);
            }
            else
            {
                Destroy(other);
            }
        }
    }
}