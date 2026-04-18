using UnityEngine;

public class JumpBoostItem : MonoBehaviour
{
    [Header("증가할 점프력 수치")]
    public float boostAmount = 3f;

    // 👇 이 부분이 스크립트에 추가되어야 합니다.
    [Header("버프 지속 시간 (초)")]
    public float duration = 7f;

    // 아이템에 무언가 닿았을 때 실행되는 함수 (Is Trigger가 체크되어 있어야 함)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 오브젝트의 태그가 "Player"인지 확인
        if (collision.CompareTag("Player"))
        {
            // 플레이어 오브젝트에서 PlayerController 스크립트 가져오기
            PlayerController player = collision.GetComponent<PlayerController>();

            // 스크립트를 성공적으로 찾았다면
            if (player != null)
            {
                // 👇 괄호 안에 boostAmount뿐만 아니라 duration도 같이 넘겨주도록 수정!
                player.IncreaseJumpPower(boostAmount, duration);
                // 아이템은 먹었으니 화면에서 파괴
                Destroy(gameObject);
            }
        }
    }
}