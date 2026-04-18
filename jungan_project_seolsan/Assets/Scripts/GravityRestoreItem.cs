using UnityEngine;

public class GravityRestoreItem : MonoBehaviour
{
    public GameObject GroundCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 1. 중력을 무조건 정상(양수)으로 되돌리기
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // 만약 중력이 마이너스(뒤집힌 상태)라면 다시 양수로 만듭니다.
                if (playerRb.gravityScale < 0)
                {
                    playerRb.gravityScale = playerRb.gravityScale * -1;
                }
            }

            // 2. 이미지를 무조건 원래대로(정방향) 되돌리기
            SpriteRenderer playerSR = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerSR != null)
            {
                // flipY를 무조건 false(꺼짐) 상태로 강제 지정합니다.
                playerSR.flipY = false;
            }

            // 3. 바닥 판정(GroundCheck) 위치를 무조건 발밑으로 되돌리기
            if (GroundCheck != null)
            {
                Vector3 newPos = GroundCheck.transform.localPosition;
                // 만약 GroundCheck가 머리 위(양수)에 있다면 발밑(음수)으로 내립니다.
                if (newPos.y > 0)
                {
                    newPos.y = newPos.y * -1;
                }
                GroundCheck.transform.localPosition = newPos;
            }

            // 4. 아이템 파괴
            Destroy(gameObject);
        }
    }
}