using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public Image WBG; 
    public Button RestartButton; 
    public Button GameEndButton; 
    public float fadeDuration = 2f; 
    private bool isFading = false;
    private float fadeTimer = 0f;

    private Text restartButtonText;
    private Text gameEndButtonText;
    private Image restartButtonImage;
    private Image gameEndButtonImage;

    void Start()
    {
        SetImageAlpha(WBG, 0);

        restartButtonText = RestartButton.GetComponentInChildren<Text>();
        gameEndButtonText = GameEndButton.GetComponentInChildren<Text>();
        restartButtonImage = RestartButton.GetComponent<Image>();
        gameEndButtonImage = GameEndButton.GetComponent<Image>();

        SetImageAlpha(restartButtonImage, 1);
        SetTextAlpha(restartButtonText, 1);
        SetImageAlpha(gameEndButtonImage, 1);
        SetTextAlpha(gameEndButtonText, 1);

        RestartButton.onClick.AddListener(OnRestartButtonClicked);
        GameEndButton.onClick.AddListener(OnGameEndButtonClicked);
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

            SetImageAlpha(WBG, alpha);

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
