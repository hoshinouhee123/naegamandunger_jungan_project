using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public int hp = 12;

    [Header("Hurt & Invincibility Settings")]
    public GameObject[] damagePrefabs;
    public float hurtDuration = 0.5f;
    public float invincibilityDuration = 1.5f;
    public int damageAmount = 30;
    private bool isHurt = false;
    private bool isInvincible = false;

    [Header("Half HP Phase Settings")]
    public GameObject halfHpPrefab;
    private int maxHp;
    private bool hasSpawnedHalfHpPrefab = false;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float jumpCooldown = 2f;
    public float heightToJump = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float jumpTimer = 0f;
    public bool isGrounded;

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
    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        maxHp = hp;
        UpdateHpUI();
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking || isHurt || Time.timeScale == 0) return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        }

        jumpTimer -= Time.fixedDeltaTime;

        if (player.position.y > rb.position.y + heightToJump && isGrounded && jumpTimer <= 0f && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpTimer = jumpCooldown;
        }

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

            if (dashTimer <= 0f && distanceToTarget > dashTriggerDistance && isGrounded)
            {
                StartDash(targetX);
                idleTimer = 0f;
            }
            else
            {
                if (distanceToTarget > aggroDistance)
                {
                    idleTimer = 0f;
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                }
                else
                {
                    if(distanceToTarget > 0.05f) // 0.5f였던 것을 아주 작게 수정
{
                        float directionX = Mathf.Sign(targetX - rb.position.x);
                        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
                        idleTimer = 0f;
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
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

        // 방향 전환
        if (!isAttacking && !isHurt)
        {
            if (player.position.x < rb.position.x) transform.localScale = new Vector3(2, 2, 1);
            else if (player.position.x > rb.position.x) transform.localScale = new Vector3(-2, 2, 1);
        }

        // ★ 애니메이션 한꺼번에 업데이트 (새로 추가된 핵심 로직)
        UpdateAnimations();
    }

    // ★★★ 흩어져 있던 애니메이션 코드를 한 곳으로 모은 함수 ★★★
    void UpdateAnimations()
    {
        if (isHurt) return;

        animator.SetBool("isDashing", isDashing);
        animator.SetBool("isAttacking", isAttacking);

        // ★ 점프 상태 추가: 바닥에 없거나 위로 솟구치고 있을 때
        bool isJumping = !isGrounded || rb.linearVelocity.y > 0.5f;
        animator.SetBool("isJumping", isJumping);

        // 걷기 상태: 점프, 대쉬, 공격 중이 아닐 때만 걷기
        bool isMoving = !isDashing && !isAttacking && !isJumping && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void StartDash(float targetX)
    {
        isDashing = true;
        dashTarget = new Vector2(targetX, rb.position.y);
        // 애니메이션 세팅은 이제 UpdateAnimations()에서 자동으로 처리됨
    }

    void UpdateHpUI()
    {
        if (bossHpText != null) bossHpText.text = "Boss HP: " + hp;
    }

    IEnumerator HurtRoutine()
    {
        isInvincible = true;
        isHurt = true;
        isDashing = false;
        isAttacking = false;
        dashTimer = dashCooldown;

        rb.linearVelocity = Vector2.zero; // 물리 이동 완전 정지

        // ★ 수정된 부분: Trigger 대신 Bool을 켜서 피격 상태를 강제로 유지시킵니다.
        animator.SetBool("isWalking", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHurt", true); // <--- 애니메이터에 isHurt 파라미터를 추가해야 합니다!

        hp -= damageAmount;
        UpdateHpUI();

        if (hp <= maxHp / 2 && !hasSpawnedHalfHpPrefab)
        {
            if (halfHpPrefab != null)
            {
                Instantiate(halfHpPrefab, transform.position, Quaternion.identity);
                hasSpawnedHalfHpPrefab = true;
            }
        }

        if (hp <= 0)
        {
            SceneManager.LoadScene("Ending_Scene");
            yield break;
        }

        StartCoroutine(BlinkRoutine());

        // 여기서 설정된 시간(hurtDuration) 동안은 무조건 피격 상태가 유지됩니다.
        yield return new WaitForSeconds(hurtDuration);

        // ★ 수정된 부분: 대기 시간이 끝나면 피격 애니메이션 Bool을 꺼줍니다.
        isHurt = false;
        animator.SetBool("isHurt", false);

        idleTimer = 0f;

        float remainingInvincibility = invincibilityDuration - hurtDuration;
        if (remainingInvincibility > 0) yield return new WaitForSeconds(remainingInvincibility);

        isInvincible = false;
    }

    IEnumerator BlinkRoutine()
    {
        if (spriteRenderer == null) yield break;

        while (isInvincible)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator AttackRoutine()
    {
        if (isHurt) yield break;

        isDashing = false;
        isAttacking = true;
        // 애니메이션 세팅은 이제 UpdateAnimations()에서 자동으로 처리됨

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
            if (isHurt) break;

            rb.MovePosition(Vector2.Lerp(startPos, lungeTarget, elapsed / lungeTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackDuration - lungeTime);

        isAttacking = false;
        dashTimer = dashCooldown;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) TriggerGameOver();
        CheckDamage(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 부딪히고 있는 중(비비는 중)일 때도 강제로 게임 오버 판정
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) TriggerGameOver();
        CheckDamage(other.gameObject);
    }

    void CheckDamage(GameObject hitObject)
    {
        bool isDamageWeapon = false;

        foreach (GameObject prefab in damagePrefabs)
        {
            if (prefab != null && hitObject.name.Contains(prefab.name))
            {
                isDamageWeapon = true;
                break;
            }
        }

        if (isDamageWeapon && !isInvincible)
        {
            StartCoroutine(HurtRoutine());
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