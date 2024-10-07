using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstGameProg2Game
{
    public class EvolvePanelScript : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button dualBarrelButton;
        [SerializeField] private Button spikesButton;
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
            dualBarrelButton.onClick.AddListener(DualBarrel);
            spikesButton.onClick.AddListener(Spikes);

            GameManager.Instance.OnGameStateChanged.AddListener(GameManager_OnGameStateChanged);

            gameObject.SetActive(false);
        }

        private void GameManager_OnGameStateChanged(GameState newState)
        {
            gameObject.SetActive(newState == GameState.EVOLVE);
            if(newState == GameState.EVOLVE) Open();
        }

        private void Resume()
        {
            GameManager.Instance.ChangeState(GameState.PLAYING);
            gameObject.SetActive(false);
        }

        private void DualBarrel()
        {
            FindObjectOfType<Player>().SwitchToDualBarrel();

            Close();
        }

        private void Spikes()
        {
            FindObjectOfType<Player>().SwitchToSpikes();

            Close();
        }

        private void Open()
        {
            panelImage.DOColor(initialPanelColor, 1f).SetEase(Ease.OutQuint).SetUpdate(true);
            backgroundImage.transform.DOScale(initialBackgroundScale, 1f).SetEase(Ease.OutQuint).SetUpdate(true);

            dualBarrelButton.interactable = true;
            spikesButton.interactable = true;
        }

        private void Close()
        {
            panelImage.DOColor(Color.clear, 1f).SetEase(Ease.OutQuint).OnComplete(Resume).SetUpdate(true);
            backgroundImage.transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InQuint).SetUpdate(true);

            dualBarrelButton.interactable = false;
            spikesButton.interactable = false;
        }
    }
}

