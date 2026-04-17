using UnityEngine;

public class InvincibilityItem : MonoBehaviour
{
    // 아이템은 부딪혀서 통과해야 하므로 OnTrigger를 씁니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 게 플레이어인지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어에게 붙어있는 Choongdol_GameOver 스크립트를 가져옵니다.
            Choongdol_GameOver player = collision.GetComponent<Choongdol_GameOver>();

            if (player != null)
            {
                // 플레이어의 무적 함수를 실행시킵니다.
                player.ActivateInvincibility();
            }

            // 아이템은 먹었으니 파괴해서 화면에서 없앱니다.
            Destroy(gameObject);
        }
    }
}