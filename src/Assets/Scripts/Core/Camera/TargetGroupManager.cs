#region

using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using System.Collections;

#endregion

namespace Core.Camera
{
    [RequireComponent(typeof(CinemachineTargetGroup))]
    public class TargetGroupManager : MonoBehaviour
    {
        [SerializeField] private float objectDistanceThreshold = 20f;
        [SerializeField] private CinemachineVirtualCamera battleCamera;
        [SerializeField] private float minFOV = 67.3f;
        [SerializeField] private float maxFOV = 100f;
        [SerializeField] private float minDistance = 5f;
        [SerializeField] private float maxDistance = 20f;
        private CameraSwitcher cameraSwitcher;

        private float lastCheckAt;

        private List<(Transform, CameraTargetGroupTag)> objects;

        private GameObject playerForwardObject;

        private Transform playerTransform;
        private CinemachineTargetGroup targetGroup;

        private void Awake()
        {
            objects = new List<(Transform, CameraTargetGroupTag)>();
            targetGroup = GetComponent<CinemachineTargetGroup>();
            cameraSwitcher = GetComponent<CameraSwitcher>();
        }

        private void Update()
        {
            if (Time.time - lastCheckAt < 1f || playerTransform is null) return;

            lastCheckAt = Time.time;

            float closestDistance = float.MaxValue;
            foreach (var objectTuple in objects)
                if (Vector3.Distance(playerTransform.position, objectTuple.Item1.position) < objectDistanceThreshold)
                    AddMember(objectTuple);
                else
                    RemoveMember(objectTuple);
            foreach (var objectTuple in objects)
            {
                float distance = Vector3.Distance(playerTransform.position, objectTuple.Item1.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }
            
            float t = Mathf.InverseLerp(minDistance, maxDistance, closestDistance);
            float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
            if (Mathf.Abs(battleCamera.m_Lens.FieldOfView - targetFOV) > 0.1f)
            {
                StartCoroutine(ChangeFOV(targetFOV));
            }
        }
        private IEnumerator ChangeFOV(float targetFOV)
        {
            float startFOV = battleCamera.m_Lens.FieldOfView;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newFOV = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
                battleCamera.m_Lens.FieldOfView = newFOV;
                yield return null;
            }

            battleCamera.m_Lens.FieldOfView = targetFOV;
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
            if (playerForwardObject is null) return;

            playerForwardObject.transform.position = playerTransform.position + forward.normalized * distance;
        }

        public void AddTarget(Transform target, CameraTargetGroupTag targetGroupTag)
        {
            if (!objects.Contains((target, targetGroupTag))) objects.Add((target, targetGroupTag));
        }

        public void RemoveTarget(Transform target)
        {
            var index = objects.FindIndex(tuple => tuple.Item1 == target);

            if (index > -1 && index < objects.Count)
            {
                RemoveMember(objects[index]);
                objects.RemoveAt(index);
            }
        }

        private void AddMember((Transform, CameraTargetGroupTag) item)
        {
            var find = targetGroup.FindMember(item.Item1) != -1;
            if (find) return;

            var tags = item.Item2;
            targetGroup.AddMember(item.Item1, tags.Weight, tags.Radius);
            cameraSwitcher.InBattleCamera();
        }

        private void RemoveMember((Transform, CameraTargetGroupTag) item)
        {
            // removeは内部でcheckがあるのでここでは`find`をしない
            targetGroup.RemoveMember(item.Item1);
            cameraSwitcher.OutBattleCamera();
        }
    }
}