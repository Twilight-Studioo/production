#region

using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

#endregion

namespace Core.Camera
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupManager : MonoBehaviour
    {
        [SerializeField] private float objectDistanceThreshold = 20f;

        private float lastCheckAt;

        private List<(Transform, CameraTargetGroupTag)> objects;

        private GameObject playerForwardObject;

        private Transform playerTransform;
        private CinemachineTargetGroup targetGroup;

        private void Awake()
        {
            objects = new();
            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        private void Update()
        {
            if (Time.time - lastCheckAt < 1f || playerTransform is null)
            {
                return;
            }

            lastCheckAt = Time.time;

            foreach (var objectTuple in objects)
            {
                if (Vector3.Distance(playerTransform.position, objectTuple.Item1.position) < objectDistanceThreshold)
                {
                    AddMember(objectTuple);
                }
                else
                {
                    RemoveMember(objectTuple);
                }
            }
        }

        public void SetPlayer(Transform player)
        {
            var playerForward = new GameObject("PlayerForward");
            playerForward.transform.position = player.position;
            playerForward.transform.parent = player;
            targetGroup.AddMember(playerForward.transform, CameraTargetGroupTag.Player().Weight,
                CameraTargetGroupTag.Player().Radius);
            playerForwardObject = playerForward;
            playerTransform = player;
        }

        public void UpdatePlayerForward(Vector3 forward, float distance)
        {
            if (playerForwardObject is null)
            {
                return;
            }

            playerForwardObject.transform.position = playerTransform.position + forward.normalized * distance;
        }

        public void AddTarget(Transform target, CameraTargetGroupTag targetGroupTag)
        {
            if (!objects.Contains((target, targetGroupTag)))
            {
                objects.Add((target, targetGroupTag));
            }
        }

        public void RemoveTarget(Transform target)
        {
            var index = objects.FindIndex(tuple => tuple.Item1 == target);
            if (index > -1 && index < objects.Count)
            {
                objects.RemoveAt(index);
            }
        }

        private void AddMember((Transform, CameraTargetGroupTag) item)
        {
            var find = targetGroup.FindMember(item.Item1) != -1;
            if (find)
            {
                return;
            }

            var tags = item.Item2;
            targetGroup.AddMember(item.Item1, tags.Weight, tags.Radius);
        }

        private void RemoveMember((Transform, CameraTargetGroupTag) item)
        {
            // removeは内部でcheckがあるのでここでは`find`をしない
            targetGroup.RemoveMember(item.Item1);
        }
    }
}