using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Feature.Component
{
    public class EndSceneController : MonoBehaviour
    {
        [FormerlySerializedAs("WBG")] public Image wbg; 
        [FormerlySerializedAs("RestartButton")] public Button restartButton; 
        [FormerlySerializedAs("GameEndButton")] public Button gameEndButton; 
        public float fadeDuration = 2f; 
        private bool isFading = false;
        private float fadeTimer = 0f;

        private Text restartButtonText;
        private Text gameEndButtonText;
        private Image restartButtonImage;
        private Image gameEndButtonImage;

        void Start()
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

        void OnRestartButtonClicked()
        {
            isFading = true;
        }

        void OnGameEndButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        
            Application.Quit();
#endif
        }

        void Update()
        {
            if (isFading)
            {
                fadeTimer += Time.deltaTime;
                float alpha = Mathf.Clamp01(fadeTimer / fadeDuration); 

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

        void SetImageAlpha(Image image, float alpha)
        {
            if (image != null)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        void SetTextAlpha(Text text, float alpha)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }
    }
}
