#region

using Cinemachine;
using UnityEngine;

#endregion

namespace Core.Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase mainCamera;
        [SerializeField] private CinemachineVirtualCameraBase swapCamera;
        [SerializeField] private CinemachineVirtualCameraBase battleCamera;

        public void UseSwapCamera(bool isSwap)
        {
            if (isSwap)
            {
                mainCamera.Priority = 0;
                swapCamera.Priority = 1;
                battleCamera.Priority = 0;
            }
            else
            {
                mainCamera.Priority = 1;
                swapCamera.Priority = 0;
                battleCamera.Priority = 0;
            }
        }

        public void InBattleCamera()
        {
            mainCamera.Priority = 0;
            swapCamera.Priority = 0;
            battleCamera.Priority = 1;
        }

        public void OutBattleCamera()
        {
            mainCamera.Priority = 1;
            swapCamera.Priority = 0;
            battleCamera.Priority = 0;
        }
    }
}