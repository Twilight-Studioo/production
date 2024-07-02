#region

using Cinemachine;
using UnityEngine;

#endregion

namespace Core.Camera
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupManager : MonoBehaviour
    {
        private CinemachineTargetGroup targetGroup;

        private void Awake()
        {
            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        public void AddTarget(Transform target, CameraTargetGroupTag tag)
        {
            targetGroup.AddMember(target, tag.Weight, tag.Radius);
        }
    }
}