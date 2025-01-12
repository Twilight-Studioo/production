using UnityEngine;
using UnityEngine.SceneManagement;

namespace Feature.Component
{
    [RequireComponent(typeof(Collider))]
    public class FixedLoadSceneArea : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        private void Awake()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("SceneName is empty.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}