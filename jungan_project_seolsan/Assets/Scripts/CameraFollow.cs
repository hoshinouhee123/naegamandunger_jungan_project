using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 대상 (플레이어를 끌어다 놓으세요)")]
    public Transform player; // 인스펙터에서 플레이어 오브젝트를 연결해줄 변수

    private float fixedY;    // 고정해둘 카메라의 처음 Y 위치
    private float fixedZ;    // 고정해둘 카메라의 처음 Z 위치 (2D에선 보통 -10)

    void Start()
    {
        // 게임 시작할 때 카메라가 처음 가지고 있던 Y, Z 좌표를 기억해 둡니다.
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    // Update 대신 LateUpdate를 쓰면 화면이 덜덜 떨리는 현상을 막을 수 있습니다.
    void LateUpdate()
    {
        // 만약 플레이어가 파괴되거나 연결되지 않았다면 오류를 막기 위해 멈춥니다.
        if (player == null) return;

        // 카메라의 위치를 업데이트 합니다.
        // X축: 플레이어의 X 위치를 그대로 따라감
        // Y, Z축: Start()에서 기억해둔 고정값을 그대로 사용함
        transform.position = new Vector3(player.position.x, fixedY, fixedZ);
    }
}