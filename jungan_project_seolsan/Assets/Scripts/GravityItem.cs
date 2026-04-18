using UnityEngine;

public class GravityItem : MonoBehaviour
{
    public GameObject GroundCheck;


    // 유니티에서 어떤 물체가 나(아이템)에게 닿았을 때 자동으로 실행되는 구역입니다.
    // collision 이라는 변수 안에 나에게 닿은 물체의 정보가 들어옵니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 나에게 닿은 물체의 이름표(Tag)가 "Player"인지 확인합니다.
        // (몬스터나 벽이 닿았을 때 중력이 바뀌면 안 되니까요!)
        if (collision.gameObject.tag == "Player")
        {
            // 2. 닿은 플레이어의 물리 엔진(Rigidbody2D) 부품을 가져옵니다.
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // 3. 플레이어의 중력을 뒤집습니다. (양수면 음수로, 음수면 양수로)
            if (playerRb != null)
            {
                playerRb.gravityScale = playerRb.gravityScale * -1;
            }

            // ★ 2. localScale 대신 flip Y를 사용해서 이미지 뒤집기 (애니메이션 충돌 방지)
            SpriteRenderer playerSR = collision.gameObject.GetComponent<SpriteRenderer>();
            if (playerSR != null)
            {
                playerSR.flipY = !playerSR.flipY;
            }

            // ★ 3. GroundCheck(바닥 판정) 위치 위아래로 뒤집기
            if (GroundCheck != null)
            {
                Vector3 newPos = GroundCheck.transform.localPosition;
                newPos.y = newPos.y * -1;
                GroundCheck.transform.localPosition = newPos;
            }


            // 5. 아이템 효과를 다 줬으니, 화면에서 아이템을 파괴(삭제)합니다.
            Destroy(gameObject);
        }
    }
}