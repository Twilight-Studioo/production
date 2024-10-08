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

        public static GizmoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("GizmoManager");
                    instance = go.AddComponent<GizmoManager>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var drawable in gizmoDrawables)
            {
                drawable.DrawGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var drawable in gizmoDrawables)
            {
                drawable.DrawGizmosSelected();
            }
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
    }
}