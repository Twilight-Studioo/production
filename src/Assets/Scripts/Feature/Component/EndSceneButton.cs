#region

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

#endregion

namespace Feature.Component
{
    public class EndSceneController : MonoBehaviour
    {
        [FormerlySerializedAs("WBG")] public Image wbg;

        [FormerlySerializedAs("RestartButton")]
        public Button restartButton;

        [FormerlySerializedAs("GameEndButton")]
        public Button gameEndButton;

        public float fadeDuration = 2f;
        private float fadeTimer;
        private Image gameEndButtonImage;
        private Text gameEndButtonText;
        private bool isFading;
        private Image restartButtonImage;

        private Text restartButtonText;

        private void Start()
        {
            restartButton.Select();
            SetImageAlpha(wbg, 0);

            restartButtonText = restartButton.GetComponentInChildren<Text>();
            gameEndButtonText = gameEndButton.GetComponentInChildren<Text>();
            restartButtonImage = restartButton.GetComponent<Image>();
            gameEndButtonImage = gameEndButton.GetComponent<Image>();

            SetImageAlpha(restartButtonImage, 1);
            SetTextAlpha(restartButtonText, 1);
            SetImageAlpha(gameEndButtonImage, 1);
            SetTextAlpha(gameEndButtonText, 1);

            restartButton.onClick.AddListener(OnRestartButtonClicked);
            gameEndButton.onClick.AddListener(OnGameEndButtonClicked);
        }

        private void Update()
        {
            if (isFading)
            {
                fadeTimer += Time.deltaTime;
                var alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

                SetImageAlpha(wbg, alpha);

                SetImageAlpha(restartButtonImage, 1 - alpha);
                SetTextAlpha(restartButtonText, 1 - alpha);
                SetImageAlpha(gameEndButtonImage, 1 - alpha);
                SetTextAlpha(gameEndButtonText, 1 - alpha);

                if (alpha >= 1)
                {
                    isFading = false;
                    SceneManager.LoadScene("stage");
                }
            }
        }

        private void OnRestartButtonClicked()
        {
            isFading = true;
        }

        private void OnGameEndButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void SetImageAlpha(Image image, float alpha)
        {
            if (image != null)
            {
                var color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        private void SetTextAlpha(Text text, float alpha)
        {
            if (text != null)
            {
                var color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
    }
}