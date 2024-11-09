using UnityEngine;

namespace Feature.Component
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject katana;
        [SerializeField] private GameObject sheath;
        [SerializeField] private GameObject katanaReverse;
        public void StartAnimation()
        {
            katana.SetActive(true);
            sheath.SetActive(true);
        }
        public void FinishAnimation()
        {
            katana.SetActive(false);
            sheath.SetActive(false);
            katanaReverse.SetActive(false);
        }

        public void ThirdAttack()
        {
           katanaReverse.SetActive(true); 
        }
    }
}