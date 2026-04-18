using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    [Header("아이템 설정")]
    public float speedBoostAmount = 3f; // 얼마나 빨라질지
    public float buffDuration = 5f;     // 몇 초 동안 유지될지

    // 다른 콜라이더(플레이어)가 아이템에 닿았을 때 실행되는 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 오브젝트의 태그가 "Player"인지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어의 스크립트 가져오기 (스크립트 이름이 다르면 수정해주세요)
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // 플레이어 스크립트에 있는 속도 증가 함수 실행
                player.BoostSpeed(speedBoostAmount, buffDuration);

                // 아이템은 먹었으니 화면에서 파괴(삭제)
                Destroy(gameObject);
            }
        }
    }
}