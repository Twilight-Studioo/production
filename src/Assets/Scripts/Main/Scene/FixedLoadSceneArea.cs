using System;
using Main.Scene.Generated;
using UnityEngine;

namespace Main.Scene
{
    [RequireComponent(typeof(Collider))]
    public class FixedLoadSceneArea : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        private Generated.Scene? scene;

        private void Awake()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("SceneName is empty.");
            }

            if (Enum.TryParse<Generated.Scene>(sceneName, out var result))
            {
                scene = result;
            }
            else
            {
                Debug.LogError($"SceneName is invalid. SceneName: {sceneName}");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && scene.HasValue)
            {
                SceneLoaderFeatures.GetSceneLoader(scene.Value, null).Bind(RootInstance.Shared).Load();
            }
        }
    }
}