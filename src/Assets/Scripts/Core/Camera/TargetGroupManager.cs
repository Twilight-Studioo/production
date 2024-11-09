#region

using System.Collections;
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
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float minFOV = 67.3f;
        [SerializeField] private float maxFOV = 100f;
        [SerializeField] [Range(0, 25)] private float minDistance = 5f;
        [SerializeField] [Range(0, 25)] private float maxDistance = 20f;
        [SerializeField] private float fovMargin = 5f;
        [SerializeField] private float changeFOVTime = 0.8f;

        private Coroutine changeFOVCoroutine;
        private float lastCheckAt;

        private List<(Transform, CameraTargetGroupTag)> objects;

        private GameObject playerForwardObject;

        private Transform playerTransform;
        private CinemachineTargetGroup targetGroup;

        private void Awake()
        {
            objects = new List<(Transform, CameraTargetGroupTag)>();
            targetGroup = GetComponent<CinemachineTargetGroup>();
        }

        private void Update()
        {
            if (Time.time - lastCheckAt < 1f || playerTransform is null) return;

            lastCheckAt = Time.time;

            var closestDistance = float.MaxValue;
            var hasValidTarget = false;

            // 各ターゲットとの距離をチェック
            foreach (var objectTuple in objects)
            {
                var distance = Vector3.Distance(playerTransform.position, objectTuple.Item1.position);

                if (distance < objectDistanceThreshold)
                {
                    AddMember(objectTuple);
                    hasValidTarget = true;
                }
                else
                {
                    RemoveMember(objectTuple);
                }

                if (hasValidTarget && distance + fovMargin < closestDistance) closestDistance = distance + fovMargin;
            }

            // ターゲットが存在しない場合、FOVを最小値に設定
            if (!hasValidTarget)
            {
                var currentFOV = virtualCamera.m_Lens.FieldOfView;
                if (Mathf.Abs(currentFOV - minFOV) > 0.1f)
                {
                    if (changeFOVCoroutine != null) StopCoroutine(changeFOVCoroutine);
                    changeFOVCoroutine = StartCoroutine(ChangeFOV(minFOV));
                }

                return;
            }

            var t = Mathf.InverseLerp(minDistance, maxDistance, closestDistance);
            var targetFOV = Mathf.Lerp(minFOV, maxFOV, t);

            if (Mathf.Abs(virtualCamera.m_Lens.FieldOfView - targetFOV) > 0.1f)
            {
                if (changeFOVCoroutine != null) StopCoroutine(changeFOVCoroutine);
                changeFOVCoroutine = StartCoroutine(ChangeFOV(targetFOV));
            }
        }

        private IEnumerator ChangeFOV(float targetFOV)
        {
            var startFOV = virtualCamera.m_Lens.FieldOfView;
            var elapsed = 0f;

            while (elapsed < changeFOVTime)
            {
                elapsed += Time.deltaTime;
                var newFOV = Mathf.Lerp(startFOV, targetFOV, elapsed / changeFOVTime);
                virtualCamera.m_Lens.FieldOfView = newFOV;
                yield return null;
            }

            virtualCamera.m_Lens.FieldOfView = targetFOV;
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
            var isMemberFound = targetGroup.FindMember(item.Item1) != -1;
            if (isMemberFound) return;

            var tags = item.Item2;
            targetGroup.AddMember(item.Item1, tags.Weight, tags.Radius + fovMargin);
        }

        private void RemoveMember((Transform, CameraTargetGroupTag) item)
        {
            // removeは内部でcheckがあるのでここでは`find`をしない
            targetGroup.RemoveMember(item.Item1);
        }
    }
}