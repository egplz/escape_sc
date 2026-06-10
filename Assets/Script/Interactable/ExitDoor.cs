using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("출구 설정")]
    public GameObject lockedVisual;    // 잠긴 상태 오브젝트 (자물쇠 등)
    public GameObject unlockedVisual;  // 열린 상태 오브젝트
    public Material activeMaterial;    // 활성화 시 머티리얼 (선택)

    private bool isUnlocked = false;
    private Renderer doorRenderer;

    void Start()
    {
        doorRenderer = GetComponent<Renderer>();
        UpdateVisual();

        // GameManager 이벤트 구독
        if (GameManager.Instance != null)
            GameManager.Instance.OnBookCountChanged += OnBookCountChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnBookCountChanged -= OnBookCountChanged;
    }

    void OnBookCountChanged(int current, int total)
    {
        if (current >= total)
            Unlock();
    }

    void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
        Debug.Log("출구가 열렸습니다!");
    }

    void UpdateVisual()
    {
        if (lockedVisual != null) lockedVisual.SetActive(!isUnlocked);
        if (unlockedVisual != null) unlockedVisual.SetActive(isUnlocked);
        if (doorRenderer != null && activeMaterial != null && isUnlocked)
            doorRenderer.material = activeMaterial;
    }

    // PlayerInteraction에서 E키 감지
    public void Interact()
    {
        if (!isUnlocked)
        {
            int remaining = GameManager.Instance.totalBooks - GameManager.Instance.bookCount;
            Debug.Log($"책이 {remaining}권 더 필요합니다!");
            return;
        }

        GameManager.Instance?.Win();
    }

    public string GetPromptText()
    {
        if (!isUnlocked)
        {
            int remaining = GameManager.Instance != null
                ? GameManager.Instance.totalBooks - GameManager.Instance.bookCount
                : 0;
            return $"[잠김] 책 {remaining}권 더 필요";
        }
        return "[E] 탈출하기";
    }

    // 트리거로도 탈출 가능
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUnlocked)
            GameManager.Instance?.Win();
    }
}
