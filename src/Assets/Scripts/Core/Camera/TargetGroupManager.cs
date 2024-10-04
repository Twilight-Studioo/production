#region

using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

#endregion

namespace Core.Camera
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupManager : MonoBehaviour
    {
        private CinemachineTargetGroup targetGroup;

        private List<(Transform, CameraTargetGroupTag)> objects;

        private GameObject playerForwardObject;

        private Transform playerTransform;

        private void Awake()
        {
            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        private void Update()
        {
            
        }

        public void SetPlayer(Transform player)
        {
            var playerForward = new GameObject("PlayerForward");
            playerForward.transform.position = player.position;
            playerForward.transform.parent = player;
            targetGroup.AddMember(playerForward.transform, CameraTargetGroupTag.Player().Weight, CameraTargetGroupTag.Player().Radius);
            playerForwardObject = playerForward;
            playerTransform = player;
        }

        public void UpdatePlayerForward(Vector3 forward, float distance)
        {
            if (playerForwardObject == null)
            {
                return;
            }

            playerForwardObject.transform.position = playerTransform.position + forward.normalized * distance;
        }

        public void AddTarget(Transform target, CameraTargetGroupTag targetGroupTag)
        {
            targetGroup.AddMember(target, targetGroupTag.Weight, targetGroupTag.Radius);
        }
        
        public void RemoveTarget(Transform target)
        {
            targetGroup.RemoveMember(target);
        }
    }
}