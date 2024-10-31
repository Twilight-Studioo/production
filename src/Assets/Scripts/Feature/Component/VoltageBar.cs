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


        public void UpdateVoltageBar(int voltageValue, int useVoltageAttackValue)
        {
            if (voltageValue >= useVoltageAttackValue)
            {
                fill.color = Color.yellow;
            }
            else
            {
                fill.color = Color.blue;
            }

            voltageBar.value = voltageValue;
        }
    }
}