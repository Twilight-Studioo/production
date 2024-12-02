#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Feature.Component
{
    public class VoltageBar : MonoBehaviour
    {
        [SerializeField] private Slider voltageBar;
        [SerializeField] private Image fill;


        public void UpdateVoltageBar(int voltageValue, int useVoltageAttackValue, int votageTwoAttackValue, int maxVoltage)
        {
            if (voltageValue >= maxVoltage)
                fill.color = Color.red;
            else if (voltageValue >= votageTwoAttackValue)
                fill.color = Color.green;
            else if (voltageValue >= useVoltageAttackValue)
                fill.color = Color.yellow;
            else
                fill.color = Color.blue;

            voltageBar.value = voltageValue;
        }
    }
}