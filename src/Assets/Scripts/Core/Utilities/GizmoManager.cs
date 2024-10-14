#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Core.Utilities
{
    [ExecuteInEditMode]
    public class GizmoManager : MonoBehaviour
    {
        private static GizmoManager instance;

        private readonly List<IGizmoDrawable> gizmoDrawables = new();

        private readonly List<GizmoData> gizmos = new();

        public static GizmoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = ObjectFactory.SuperObject;
                    instance = go.AddComponent<GizmoManager>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        private void Update()
        {
            // 有効期限が切れたGizmoを削除
            gizmos.RemoveAll(g => g.ExpirationTime < Time.time);
        }

        private void OnDrawGizmos()
        {
            foreach (var drawable in gizmoDrawables)
            {
                drawable.DrawGizmos();
            }

            if (gizmos == null)
            {
                return;
            }

            foreach (var gizmo in gizmos)
            {
                // Gizmoの色を設定
                Gizmos.color = new(1f, 0f, 0f, 0.5f); // 半透明の赤

                // 初期ボックスの描画
                Gizmos.DrawWireCube(gizmo.Origin, gizmo.HalfExtents * 2);

                // キャスト方向と距離の線を描画
                var endPosition = gizmo.Origin + gizmo.Direction * gizmo.MaxDistance;
                Gizmos.DrawLine(gizmo.Origin, endPosition);

                // 終点のボックスの描画
                Gizmos.DrawWireCube(endPosition, gizmo.HalfExtents * 2);

                // デフォルトのGizmoマトリックスに戻す
                Gizmos.matrix = Matrix4x4.identity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var drawable in gizmoDrawables)
            {
                drawable.DrawGizmosSelected();
            }
        }

        public void RequestGizmo(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance,
            float duration)
        {
            var data = new GizmoData
            {
                Origin = origin,
                HalfExtents = halfExtents,
                Direction = direction,
                MaxDistance = maxDistance,
                ExpirationTime = Time.time + duration,
            };
            gizmos.Add(data);
        }

        public void Register(IGizmoDrawable drawable)
        {
            if (!gizmoDrawables.Contains(drawable))
            {
                gizmoDrawables.Add(drawable);
            }
        }

        public void Unregister(IGizmoDrawable drawable)
        {
            if (gizmoDrawables.Contains(drawable))
            {
                gizmoDrawables.Remove(drawable);
            }
        }

        // Gizmo描画のデータ構造
        private class GizmoData
        {
            public Vector3 Direction;
            public float ExpirationTime;
            public Vector3 HalfExtents;
            public float MaxDistance;
            public Vector3 Origin;
        }
    }
}