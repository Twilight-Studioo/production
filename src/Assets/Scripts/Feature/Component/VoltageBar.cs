#region

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
#endregion

namespace Feature.Component
{
    public class VoltageBar : MonoBehaviour
    {
        [SerializeField] private Slider voltageBar;
        [SerializeField] private Image fill;
        [SerializeField] private GameObject voltageGameObject;
        private VisualEffect visualEffect;

        private void Awake()
        {
            visualEffect = voltageGameObject.GetComponentInChildren<VisualEffect>();
        }

        public void UpdateVoltageBar(int voltageValue, int useVoltageAttackValue, int votageTwoAttackValue, int maxVoltage)
        {
            if (voltageValue >= maxVoltage)
            {
                visualEffect.SetUInt("Voltage",3);
                fill.color = Color.red;
            }
            else if (voltageValue >= votageTwoAttackValue)
            {
                visualEffect.SetUInt("Voltage",2);
                fill.color = Color.green;
            }
            else if (voltageValue >= useVoltageAttackValue)
            {
                visualEffect.SetUInt("Voltage",1);
                fill.color = Color.yellow;                
            }
            else
            {
                visualEffect.SetUInt("Voltage",0);
                fill.color = Color.blue;
            }


            voltageBar.value = voltageValue;
        }
    }
}