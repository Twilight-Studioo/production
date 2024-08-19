using System;
using Feature.Common.Parameter;
using UniRx;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using VContainer;

namespace Feature.View
{
    public class VoltageView : MonoBehaviour
    {
        [SerializeField] private float useVoltageAttackValue = 50f;
        [SerializeField] private float addVoltageSwapValue = 10f;
        [SerializeField] private int voltageAttackPowerValue = 2;
        private float voltageValue = 0;
        private const float MAX = 100;
        private const float MIN = 0;

        [Inject] private CharacterParams characterParams;
        private void Update()
        {
            voltageValue = Mathf.Clamp(voltageValue, MIN, MAX);
            VoltagveValuebar();
        }

        public void AddVoltageSwap()
        {
            voltageValue += addVoltageSwapValue;
            Debug.Log("Swap voltage : "+ voltageValue);
        }
        
        public int UseVoltageAttack()
        {
            if (voltageValue >= useVoltageAttackValue)
            {
                voltageValue -= addVoltageSwapValue;
                Debug.Log("EnhancedAttack voltage : "+ voltageValue);
                return characterParams.attackPower * voltageAttackPowerValue;
            }
            Debug.Log("Attack voltage : "+ voltageValue);
            return characterParams.attackPower;
        }

        private void VoltagveValuebar()
        {
            
        }
    }
}