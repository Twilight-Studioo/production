using System;
using Main.Scene.Generated;
using UnityEngine;

namespace Main.Scene
{
    [RequireComponent(typeof(Collider))]
    public class FixedLoadSceneArea : MonoBehaviour
    {
        [SerializeField] private Generated.Scene sceneName;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SceneLoaderFeatures.GetSceneLoader(sceneName, null).Bind(RootInstance.Shared).Load();
            }
        }
    }
}