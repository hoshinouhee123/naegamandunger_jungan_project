using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public int hp = 12; // 주의: 기본 체력이 12인데, 데미지가 30이면 한 방에 죽습니다! 인스펙터에서 체력을 올리거나 데미지를 줄여주세요.

    [Header("Hurt Settings")]
    public GameObject NunDungi;            // 인스펙터에 등록할 특정 오브젝트
    public float hurtDuration = 0.5f;      // 아파하는 시간
    public int damageAmount = 30;          // ★ 한 번 맞을 때 깎일 체력 (기본값 30)
    private bool isHurt = false;

    [Header("Distance Settings")]
    public float aggroDistance = 10f;
    public float stopDistance = 0.5f;

    [Header("Movement Range")]
    public float minX = -10f;
    public float maxX = 10f;

    [Header("Dash & Attack Settings")]
    public float dashTriggerDistance = 6f;
    public float dashSpeed = 12f;
    public float dashCooldown = 3f;
    public GameObject obstaclePrefab;
    public float obstacleOffsetY = 0.2f;
    public float attackDuration = 1.0f;
    public float attackLungeDistance = 1.5f;
    public float idleAttackTime = 1.0f;

    [Header("UI Settings")]
    public GameObject gameOverPanel;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isDashing = false;
    private bool isAttacking = false;
    private float dashTimer = 0f;
    private float idleTimer = 0f;
    private Vector2 dashTarget;

    public TMP_Text bossHpText;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateHpUI(); // ★ 시작할 때 현재 체력을 UI에 표시
    }

    void FixedUpdate()
    {
        // 아파하는 중(isHurt)이거나 공격 중이면 이동을 멈춤
        if (player == null || isAttacking || isHurt || Time.timeScale == 0) return;

        if (isDashing)
        {
            Vector2 nextPos = Vector2.MoveTowards(rb.position, dashTarget, dashSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);

            if (Mathf.Abs(rb.position.x - dashTarget.x) < 0.1f)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            dashTimer -= Time.fixedDeltaTime;
            float targetX = Mathf.Clamp(player.position.x, minX, maxX);
            float distanceToTarget = Mathf.Abs(targetX - rb.position.x);

            if (dashTimer <= 0f && distanceToTarget > dashTriggerDistance)
            {
                StartDash(targetX);
                idleTimer = 0f;
            }
            else
            {
                if (distanceToTarget > aggroDistance)
                {
                    animator.SetBool("isWalking", false);
                    idleTimer = 0f;
                }
                else
                {
                    Vector2 targetPos = new Vector2(targetX, rb.position.y);

                    if (distanceToTarget > stopDistance)
                    {
                        Vector2 nextPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
                        rb.MovePosition(nextPos);
                        animator.SetBool("isWalking", true);
                        idleTimer = 0f;
                    }
                    else
                    {
                        animator.SetBool("isWalking", false);
                        idleTimer += Time.fixedDeltaTime;

                        if (idleTimer >= idleAttackTime)
                        {
                            idleTimer = 0f;
                            StartCoroutine(AttackRoutine());
                        }
                    }
                }
            }
        }

        // 방향 전환 (아프거나 공격 중이 아닐 때만)
        if (!isAttacking && !isHurt)
        {
            if (player.position.x < rb.position.x) transform.localScale = new Vector3(2, 2, 1);
            else if (player.position.x > rb.position.x) transform.localScale = new Vector3(-2, 2, 1);
        }
    }

    void StartDash(float targetX)
    {
        isDashing = true;
        dashTarget = new Vector2(targetX, rb.position.y);
        animator.SetBool("isWalking", false);
        animator.SetBool("isDashing", true);
    }

    // ★ UI 글자를 업데이트하는 함수
    void UpdateHpUI()
    {
        if (bossHpText != null)
        {
            bossHpText.text = "Boss HP: " + hp;
        }
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;
        isDashing = false;

        // ★ 대쉬 쿨타임을 즉시 초기화하여, 아픈 게 끝난 후 바로 대쉬하지 못하게 함
        dashTimer = dashCooldown;

        animator.SetBool("isWalking", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isAttacking", false);
        animator.SetTrigger("Hurt");

        hp -= damageAmount;
        UpdateHpUI(); // ★ 체력이 깎일 때마다 UI 업데이트
        Debug.Log("보스 피격! 쿨타임 리셋. 현재 체력: " + hp);

        if (hp <= 0)
        {
            SceneManager.LoadScene("Ending_Scene");
            yield break;
        }

        // hurtDuration(아파하는 시간) 동안 대기
        yield return new WaitForSeconds(hurtDuration);

        isHurt = false;

        // 여기서 idleTimer도 초기화해주면 좋습니다 (아프고 나서 바로 근접공격 방지)
        idleTimer = 0f;
    }

    
    IEnumerator AttackRoutine()
    {
        // ★ 공격 시작 전 확인: 이미 아픈 상태면 공격 실행 안 함
        if (isHurt) yield break;

        isDashing = false;
        isAttacking = true;

        animator.SetBool("isDashing", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        if (obstaclePrefab != null)
        {
            Vector3 spawnPosition = transform.position - new Vector3(0f, obstacleOffsetY, 0f);
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            Destroy(obstacle, 5f);
        }

        float lookDir = transform.localScale.x < 0 ? 1f : -1f;
        Vector2 startPos = rb.position;
        Vector2 lungeTarget = startPos + new Vector2(lookDir * attackLungeDistance, 0);

        lungeTarget.x = Mathf.Clamp(lungeTarget.x, minX, maxX);

        float elapsed = 0f;
        float lungeTime = 0.2f;
        while (elapsed < lungeTime)
        {
            // ★ 돌진 공격 도중 확인: 갑자기 맞아서 아파지면 돌진 루프 중단
            if (isHurt) break;

            rb.MovePosition(Vector2.Lerp(startPos, lungeTarget, elapsed / lungeTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackDuration - lungeTime);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
        dashTimer = dashCooldown;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && collision.gameObject.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            TriggerGameOver();
        }

        // ★ 중복되어 꼬였던 피격 로직 하나로 깔끔하게 통합!
        // 등록된 특정 오브젝트(NunDungi)와 부딪혔을 때만 실행
        if (other.gameObject == NunDungi)
        {
            // 부딪힌 눈덩이 오브젝트를 파괴할 거라면 아래 주석을 해제하세요.
            // Destroy(other.gameObject); 

            if (!isHurt)
            {
                StartCoroutine(HurtRoutine());
            }
        }
    }

    void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}