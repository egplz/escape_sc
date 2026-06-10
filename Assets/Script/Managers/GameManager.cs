using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("게임 설정")]
    public int totalBooks = 5;

    [Header("상태")]
    public int bookCount = 0;
    public bool isGameOver = false;
    public bool isWin = false;

    // 이벤트: UI 등 다른 시스템이 구독
    public event System.Action<int, int> OnBookCountChanged;   // (현재, 전체)
    public event System.Action OnGameOver;
    public event System.Action OnWin;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // 책 획득 시 호출
    public void CollectBook()
    {
        if (isGameOver || isWin) return;

        bookCount++;
        OnBookCountChanged?.Invoke(bookCount, totalBooks);

        Debug.Log($"책 획득! {bookCount}/{totalBooks}");

        if (bookCount >= totalBooks)
            Debug.Log("모든 책 수집 완료! 출구로 이동하세요.");
    }

    public bool AllBooksCollected() => bookCount >= totalBooks;

    // NPC 접촉 시 호출
    public void GameOver()
    {
        if (isGameOver || isWin) return;

        isGameOver = true;
        Debug.Log("게임 오버!");
        OnGameOver?.Invoke();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 탈출 성공 시 호출
    public void Win()
    {
        if (isGameOver || isWin) return;

        isWin = true;
        Debug.Log("승리!");
        OnWin?.Invoke();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
