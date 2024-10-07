using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class EndPanelScript : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button retryButton;
        [SerializeField] private TMP_Text scoreText;
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
            retryButton.onClick.AddListener(Retry);

            GameManager.Instance.OnGameStateChanged.AddListener(GameManager_OnGameStateChanged);

            gameObject.SetActive(false);
        }

        private void GameManager_OnGameStateChanged(GameState newState)
        {
            if(newState == GameState.END)
            {
                gameObject.SetActive(newState == GameState.END);
                scoreText.text = $"You died!\nScore: {GameManager.Instance.Score}";
                Open();
            }
        }

        private void Retry()
        {
            SceneManager.LoadScene("Main");
        }

        private void Open()
        {
            DOTween.Kill(panelImage);
            DOTween.Kill(backgroundImage.transform);

            panelImage.DOColor(initialPanelColor, 1f).SetEase(Ease.OutQuint).SetUpdate(true);
            backgroundImage.transform.DOScale(initialBackgroundScale, 1f).SetEase(Ease.OutQuint).SetUpdate(true);

            retryButton.interactable = true;
        }
    }
}
