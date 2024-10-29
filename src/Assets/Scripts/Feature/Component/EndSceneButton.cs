using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Feature.Component
{
    public class EndSceneController : MonoBehaviour
    {
        public Image wbg;
        public Button restartButton;
        public Button gameEndButton;
        public InputActionAsset inputPlayer;

        private InputAction navigateAction;
        private InputAction submitAction;
        private InputAction cancelAction;

        public float fadeDuration = 2f;
        private float fadeTimer;
        private Image gameEndButtonImage;
        private Text gameEndButtonText;
        private bool isFading;
        private Image restartButtonImage;
        private Text restartButtonText;

        private void Awake()
        {
            if (inputPlayer == null)
            {
                Debug.LogError("InputPlayer is not assigned.");
                return;
            }

            var uiActionMap = inputPlayer.FindActionMap("UI", true);
            navigateAction = uiActionMap.FindAction("Navigate", true);
            submitAction = uiActionMap.FindAction("Submit", true);
            cancelAction = uiActionMap.FindAction("Cancel", true);
        }

        private void OnEnable()
        {
            navigateAction.Enable();
            submitAction.Enable();
            cancelAction.Enable();

            navigateAction.performed += OnNavigate;
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
        }

        private void OnDisable()
        {
            navigateAction.performed -= OnNavigate;
            submitAction.performed -= OnSubmit;
            cancelAction.performed -= OnCancel;

            navigateAction.Disable();
            submitAction.Disable();
            cancelAction.Disable();
        }

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

        private void OnNavigate(InputAction.CallbackContext context)
        {
            Vector2 navigation = context.ReadValue<Vector2>();
            if (navigation.y > 0)
            {
                restartButton.Select();
            }
            else if (navigation.y < 0)
            {
                gameEndButton.Select();
            }
        }

        private void OnSubmit(InputAction.CallbackContext context)
        {
            var selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject == restartButton.gameObject)
            {
                OnRestartButtonClicked();
            }
            else if (selectedObject == gameEndButton.gameObject)
            {
                OnGameEndButtonClicked();
            }
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            Debug.Log("Cancel action triggered.");
        }

        private void OnRestartButtonClicked()
        {
            isFading = true;
        }

        private void OnGameEndButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
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
