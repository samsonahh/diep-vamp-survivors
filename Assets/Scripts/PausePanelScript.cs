using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class PausePanelScript : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Image backgroundImage;

        private Image panelImage;
        private Color initialPanelColor;
        private Vector3 initialBackgroundScale;

        private void Awake()
        {
            panelImage = GetComponent<Image>();
            initialPanelColor = panelImage.color;
            initialBackgroundScale = backgroundImage.transform.localScale;

            panelImage.color = Color.clear;
            backgroundImage.transform.localScale = Vector3.zero;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnGameStateChanged.RemoveListener(GameManager_OnGameStateChanged);
        }

        private void Start()
        {
            restartButton.onClick.AddListener(Restart);
            resumeButton.onClick.AddListener(Close);

            GameManager.Instance.OnGameStateChanged.AddListener(GameManager_OnGameStateChanged);

            gameObject.SetActive(false);
        }

        private void GameManager_OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.PAUSED)
            {
                gameObject.SetActive(newState == GameState.PAUSED);
                Open();
            }

            if (newState == GameState.PLAYING)
            {
                Close();
            }
        }

        private void Restart()
        {
            SceneManager.LoadScene("Main");
        }

        private void Resume()
        {
            GameManager.Instance.ChangeState(GameState.PLAYING);
            gameObject.SetActive(false);
        }

        private void Open()
        {
            DOTween.Kill(panelImage);
            DOTween.Kill(backgroundImage.transform);

            panelImage.DOColor(initialPanelColor, 1f).SetEase(Ease.OutQuint).SetUpdate(true);
            backgroundImage.transform.DOScale(initialBackgroundScale, 1f).SetEase(Ease.OutQuint).SetUpdate(true);

            restartButton.interactable = true;
            resumeButton.interactable = true;
        }

        private void Close()
        {
            DOTween.Kill(panelImage);
            DOTween.Kill(backgroundImage.transform);

            panelImage.DOColor(Color.clear, 1f).SetEase(Ease.OutQuint).OnComplete(Resume).SetUpdate(true);
            backgroundImage.transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InQuint).SetUpdate(true);

            restartButton.interactable = false;
            resumeButton.interactable = false;
        }
    }
}

