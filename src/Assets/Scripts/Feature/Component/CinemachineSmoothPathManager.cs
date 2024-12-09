using Cinemachine;
using UnityEngine;

namespace Feature.Component
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineSmoothPathManager: MonoBehaviour
    {
        public enum PathPoint
        {
            Option = 0,
            Title = 1,
            Noop = 2,
            StageSelect = 3,
        }
        private CinemachineVirtualCamera vcam;
        private CinemachineTrackedDolly dolly;
        
        private float targetPathPosition;
        private float currentPathPosition;
        
        [SerializeField] private float smoothSpeed = 1.4f;
        private void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();
            dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        private void Start()
        {
            SetPathPoint(PathPoint.StageSelect);
        }
        
        public void SetPathPoint(PathPoint pathPoint)
        {
            SetPathPointInternal(pathPoint);
        }
        
        public void SetPathPointFast(PathPoint pathPoint)
        {
            SetPathPointInternal(pathPoint);
            currentPathPosition = targetPathPosition;
        }

        private void Update()
        {
            currentPathPosition = Mathf.Lerp(currentPathPosition, targetPathPosition, Time.deltaTime * smoothSpeed);
            dolly.m_PathPosition = currentPathPosition;
        }
        
        private void SetPathPointInternal(PathPoint pathPoint)
        {
            targetPathPosition = (int)pathPoint;
        }
    }
}