using Feature.Common.Parameter;
using UnityEngine;
using VContainer;
using UnityEngine.UI;

namespace Feature.Component
{
    public class VoltageBar :MonoBehaviour
    {
        private CharacterParams characterParams;
        [SerializeField] private Slider voltageBar;
        [SerializeField] private Image fill;

        [Inject]
        public void Initialize(CharacterParams characterParams)
        {
            this.characterParams = characterParams;
        }

        public void UpdateVoltageBar()
        {
            if (characterParams.voltageValue >= characterParams.useVoltageAttackValue)
            {
                fill.color = Color.yellow;
            }
            else
            {
                fill.color = Color.blue;
            }
            Debug.Log(characterParams.voltageValue);
            voltageBar.value = (float)characterParams.voltageValue;
        }
    }
}