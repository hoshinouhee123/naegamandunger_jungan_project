using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float originalSpeed; // 원래 속도를 기억해둘 변수

    [Header("버프 상태 확인 (디버그용)")]
    private bool isSpeedBoosted = false; // 지금 버프 상태인지 확인하는 스위치
    private float speedBoostTimer = 0f;  // 버프가 끝날 때까지 남은 시간

    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private bool isObstacle; // 장애물 위에 있는지 확인하는 변수 추가
    public float obstacleCheckRadius = 0.2f; // 장애물 감지 범위
    private float moveInput;

    // 원래 점프력을 기억해둘 변수
    private float originalJumpForce;

    // 점프력 버프 타이머 관련 변수들
    private float boostTimer = 0f;     // 남은 버프 시간
    private bool isBoosted = false;    // 현재 버프 상태인지 확인

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 게임이 시작될 때 원래 속도를 저장해 둡니다.
        originalSpeed = moveSpeed;
        // 게임이 시작될 때 원래의 기본 점프력을 저장해 둡니다.
        originalJumpForce = jumpForce;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 2. 애니메이션 및 좌우반전
        if (moveInput < 0) // 왼쪽 방향키를 눌러서 이동값이 마이너스일 때
        {
            pAni.SetBool("isRunning", true);   // 뛰는 애니메이션 켜기
        }
        else if (moveInput > 0) // 오른쪽 방향키를 눌러서 이동값이 플러스일 때
        {
            pAni.SetBool("isRunning", true);   // 뛰는 애니메이션 켜기
        }
        else // 방향키에서 손을 떼서 이동값이 0이 되었을 때
        {
            pAni.SetBool("isRunning", false);  // 뛰는 애니메이션 끄기
        }

        if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // ★ 매 프레임마다 바닥 판정 변수들을 초기화합니다 (무한 점프 방지)
        isObstacle = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, obstacleCheckRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                isObstacle = true;
            }
        }

        // --- 스피드 버프 타이머 로직 ---
        if (isSpeedBoosted)
        {
            // 매 프레임마다 남은 시간을 줄입니다. (Time.deltaTime은 1초에 1씩 감소하게 해줍니다)
            speedBoostTimer -= Time.deltaTime;

            // 타이머가 0 이하가 되면 (시간이 다 되면)
            if (speedBoostTimer <= 0f)
            {
                moveSpeed = originalSpeed; // 원래 속도로 복구
                isSpeedBoosted = false;    // 버프 상태 끄기
            }
        }

        // --- 점프력 버프 타이머 로직 ---
        if (isBoosted)
        {
            // Time.deltaTime은 이전 프레임부터 지금까지 걸린 시간(초)입니다.
            // 남은 시간에서 계속 빼줍니다.
            boostTimer -= Time.deltaTime;

            // 남은 시간이 0 이하가 되면 버프 종료
            if (boostTimer <= 0f)
            {
                isBoosted = false;                // 버프 상태 해제
                jumpForce = originalJumpForce;    // 점프력을 원래대로 복구
                Debug.Log("점프력 버프 종료! 현재 점프력: " + jumpForce);
            }
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        // ★ 점프 조건 논리 수정: (바닥이거나 장애물일 때) 점프!
        if (value.isPressed && (isGrounded || isObstacle))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // 중력 상태에 따라 점프 방향 결정
            Vector2 jumpDirection = Vector2.up; // 기본값은 위쪽 방향

            if (rb.gravityScale < 0)
            {
                jumpDirection = Vector2.down; // 중력이 마이너스면 아래쪽 방향으로 점프!
            }

            // 수정된 방향(jumpDirection)으로 점프 힘을 가합니다.
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }
    }

    // 아이템 스크립트에서 이 함수를 부릅니다.
    public void BoostSpeed(float amount, float duration)
    {
        // 이미 버프를 받은 상태에서 아이템을 또 먹었을 때 속도가 무한정 올라가지 않도록 처리
        if (!isSpeedBoosted)
        {
            moveSpeed += amount; // 버프 상태가 아닐 때만 속도를 올려줍니다.
        }

        // 시간은 무조건 다시 꽉 채워줍니다. (연속으로 먹으면 지속 시간이 갱신됨)
        speedBoostTimer = duration;
        isSpeedBoosted = true; // 버프 상태 켜기
    }

    // 아이템이 호출할 함수
    public void IncreaseJumpPower(float amount, float duration)
    {
        // 버프 중이 아닐 때만 점프력을 올림 (중복 적용 방지)
        if (!isBoosted)
        {
            jumpForce += amount;
            isBoosted = true;
        }

        // 시간은 무조건 다시 지정된 시간(duration)으로 꽉 채워줌 (아이템을 연속으로 먹었을 때 연장 효과)
        boostTimer = duration;
    }
}