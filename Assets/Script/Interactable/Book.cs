using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    [Header("책 설정")]
    public string bookName = "책";
    public GameObject pickupEffect;    // 획득 이펙트 (선택)

    private bool isCollected = false;

    public void Interact()
    {
        if (isCollected) return;
        Collect();
    }

    public string GetPromptText()
    {
        return $"[E] {bookName} 줍기";
    }

    void Collect()
    {
        isCollected = true;

        // 이펙트 재생 (있다면)
        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // GameManager에 알림
        GameManager.Instance?.CollectBook();

        // 자신 삭제
        Destroy(gameObject);
    }

    // 트리거로도 획득 가능하게 (선택)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}
