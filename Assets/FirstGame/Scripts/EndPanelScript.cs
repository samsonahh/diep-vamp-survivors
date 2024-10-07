using FirstGameProg2Game;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanelScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button retryButton;


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
        if (newState != GameState.END) return;

        gameObject.SetActive(true);
    }

    private void Retry()
    {
        SceneManager.LoadScene("Main");
    }
}
