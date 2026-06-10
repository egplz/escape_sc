using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 2.5f;
    public LayerMask interactLayer;

    [Header("UI")]
    public GameObject interactPrompt;   // "E키를 눌러 상호작용" 텍스트
    public TextMeshProUGUI promptText;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        CheckInteractable();

        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();
    }

    void CheckInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (interactPrompt != null) interactPrompt.SetActive(true);
                if (promptText != null) promptText.text = interactable.GetPromptText();
                return;
            }
        }

        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void TryInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }
}

// 상호작용 가능한 오브젝트가 구현할 인터페이스
public interface IInteractable
{
    void Interact();
    string GetPromptText();
}
