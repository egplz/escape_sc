using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI bookCountText;
    public TextMeshProUGUI interactPromptText;

    [Header("패널")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    [Header("승리 패널")]
    public Button winRestartButton;
    public Button winQuitButton;

    [Header("패배 패널")]
    public Button gameOverRestartButton;
    public Button gameOverQuitButton;

    void Start()
    {
        // 패널 숨기기
        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // GameManager 이벤트 구독
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBookCountChanged += UpdateBookCount;
            GameManager.Instance.OnWin += ShowWinPanel;
            GameManager.Instance.OnGameOver += ShowGameOverPanel;
        }

        // 버튼 연결
        winRestartButton?.onClick.AddListener(RestartGame);
        winQuitButton?.onClick.AddListener(QuitGame);
        gameOverRestartButton?.onClick.AddListener(RestartGame);
        gameOverQuitButton?.onClick.AddListener(QuitGame);

        // 초기 UI
        UpdateBookCount(0, GameManager.Instance != null ? GameManager.Instance.totalBooks : 5);

        StartCoroutine(HidePromptAfterDelay());
    }

    IEnumerator HidePromptAfterDelay()
    {
        if (interactPromptText != null)
        {
            interactPromptText.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);

            interactPromptText.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBookCountChanged -= UpdateBookCount;
            GameManager.Instance.OnWin -= ShowWinPanel;
            GameManager.Instance.OnGameOver -= ShowGameOverPanel;
        }
    }

    void UpdateBookCount(int current, int total)
    {
        if (bookCountText != null)
            bookCountText.text = $"📚 {current} / {total}";
    }

    void ShowWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;   // 게임 일시정지
    }

    void ShowGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 재시작 시 timeScale 복구
    public void RestartGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.RestartGame();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.QuitGame();
    }
}
