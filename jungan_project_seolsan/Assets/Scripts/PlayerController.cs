using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private bool isObstacle; // 장애물 위에 있는지 확인하는 변수 추가
    public float obstacleCheckRadius = 0.2f; // 장애물 감지 범위
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, obstacleCheckRadius);
        foreach(Collider2D col in colliders)
        {
            if (col.CompareTag("Obstacle"))
            {
                isObstacle = true;
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
        if (value.isPressed && isGrounded || isObstacle)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
}