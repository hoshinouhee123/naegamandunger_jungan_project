using UnityEngine;
using System.Collections;

public class Choongdol_GameOver : MonoBehaviour
{
    [Header("UI Settings")]
    // 게임 오버 패널을 연결할 변수입니다.
    public GameObject gameOverPanel;
    public GameObject Player;


    [Header("Invincibility Settings")]
    public bool isInvincible = false; // 현재 무적 상태인지 확인
    public float invincibilityDuration = 3f; // 무적 지속 시간 (기본 3초)


    // 물리적으로 다른 콜라이더와 부딪혔을 때 실행되는 함수입니다.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 대상(gameObject)의 태그가 "Obstacle"인지 확인합니다.
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 만약 무적 상태라면 아무 일도 일어나지 않고(죽지 않고) 함수를 종료합니다.
            if (isInvincible)
            {
                Debug.Log("무적 상태! 장애물 무시!");
                return;
            }

            // 게임 오버 패널을 활성화하여 화면에 띄웁니다.
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // (선택 사항) 부딪히자마자 게임을 멈추고 싶다면 아래 주석을 해제하세요.
            Time.timeScale = 0f;
        }

        // 부딪힌 대상(gameObject)의 태그가 "ChuRak"인지 확인합니다.
        if (collision.gameObject.CompareTag("ChuRak"))
        {
            // 플레이어 오브젝트 파괴
            Destroy(Player);
            // 게임 오버 패널을 활성화하여 화면에 띄웁니다.
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // (선택 사항) 부딪히자마자 게임을 멈추고 싶다면 아래 주석을 해제하세요.
            Time.timeScale = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 닿아있는 대상(gameObject)의 태그가 "Obstacle"인지 확인합니다.
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // [추가된 핵심 코드] 무적 상태일 때는 여기서 함수를 종료합니다.
            // 무적이 풀리는 순간(isInvincible이 false가 되면) 이 줄을 통과해서 게임 오버가 됩니다!
            if (isInvincible)
            {
                return;
            }

            // 게임 오버 패널을 활성화하여 화면에 띄웁니다.
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // (선택 사항) 부딪히자마자 게임을 멈추고 싶다면 아래 주석을 해제하세요.
            Time.timeScale = 0f;
        }
    }

    // 아이템을 먹었을 때 실행될 함수
    public void ActivateInvincibility()
    {
        // 이미 무적 상태가 아닐 때만 타이머를 시작합니다.
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true; // 무적 켜기

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        // 정해진 시간만큼 기다립니다.
        yield return new WaitForSeconds(invincibilityDuration);

        // 무적 끄기 및 색상 원상복구
        isInvincible = false;
        sr.color = originalColor;

        // [수정 2] 무적이 끝나는 순간 잠들어있을 수 있는 물리 엔진을 강제로 깨웁니다.
        // 그러면 장애물과 닿아있을 경우 OnCollisionStay2D가 즉시 다시 실행되어 게임오버가 됩니다.
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().WakeUp();
        }
    }
}