using Cinemachine;
using UnityEngine;

namespace Core.Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase mainCamera;
        [SerializeField] private CinemachineVirtualCameraBase swapCamera;

        public void UseSwapCamera(bool isSwap)
        {
            if (isSwap)
            {
                mainCamera.Priority = 1;
                swapCamera.Priority = 0;
            }
            else
            {
                mainCamera.Priority = 0;
                swapCamera.Priority = 1;
            }
        }
    }
}