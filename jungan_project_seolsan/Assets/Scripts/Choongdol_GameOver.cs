using UnityEngine;
using System.Collections;

public class Choongdol_GameOver : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject gameOverPanel;
    public GameObject Player;

    [Header("Invincibility Settings")]
    public bool isInvincible = false;
    public float invincibilityDuration = 3f;

    // ★ 추가됨: 무적일 때 사용할 레이어 이름
    public string invincibleLayerName = "Invincible";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isInvincible)
            {
                Debug.Log("무적 상태! 장애물 무시!");
                return;
            }

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (collision.gameObject.CompareTag("ChuRak"))
        {
            Destroy(Player);
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isInvincible) return;

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ActivateInvincibility()
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // ★ 추가됨: 현재 플레이어의 원래 레이어 번호를 기억해둡니다.
        int originalLayer = gameObject.layer;

        // ★ 추가됨: 플레이어의 레이어를 '무적' 레이어로 바꿉니다.
        gameObject.layer = LayerMask.NameToLayer(invincibleLayerName);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
        sr.color = originalColor;

        // ★ 추가됨: 무적이 끝나면 원래 레이어로 다시 되돌립니다.
        gameObject.layer = originalLayer;

        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().WakeUp();
        }
    }
}