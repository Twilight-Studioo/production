using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Core.Camera
{
    
    [ExecuteAlways,DisallowMultipleComponent]
    public class LookAtTransform: CinemachineExtension
    {
        
        public CinemachineSmoothPath lookAtSmoothPath;
        
        public Transform lookAtTargetTransform;

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                // ロジックをここに追加
                var vcamComponent = vcam as CinemachineVirtualCamera;
                if (vcamComponent == null)
                {
                    return;
                }

                var dolly = vcamComponent.GetCinemachineComponent<CinemachineTrackedDolly>();
                if (dolly == null)
                {
                    return;
                }

                if (lookAtTargetTransform == null)
                {
                    return;
                }

                var pathPosition = dolly.m_PathPosition;
                var lookAtTarget = lookAtSmoothPath.EvaluatePosition(pathPosition);
                lookAtTargetTransform.position = lookAtTarget;
                vcamComponent.LookAt = lookAtTargetTransform;
            }
        }
    }
}