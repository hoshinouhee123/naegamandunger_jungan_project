using UnityEngine;

public class Snowball : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // 바닥에 닿으면 스스로 파괴됩니다. (바닥 오브젝트의 태그를 "Ground"로 설정해주세요)
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}