using UnityEngine;

public class SmartParallax : MonoBehaviour
{
    // 0에 가까울수록 카메라에 찰떡처럼 붙어있고 (먼 배경)
    // 1에 가까울수록 카메라에서 멀어집니다 (가까운 배경)
    // 보통 0.1 ~ 0.5 사이의 값을 추천합니다.
    public float parallaxEffect;

    private Transform cam;      // 카메라 위치
    private float startPosX;    // 배경의 처음 시작 위치

    void Start()
    {
        cam = Camera.main.transform;
        startPosX = transform.position.x; // 처음 위치 기억
    }

    // Update보다 조금 늦게 실행되는 LateUpdate가 카메라 추적에 더 안정적입니다.
    void LateUpdate()
    {
        // 1. 카메라가 움직인 총 거리를 구합니다.
        float distance = cam.position.x * parallaxEffect;

        // 2. 배경의 위치를 '처음 위치 + 이동 거리'로 고정합니다.
        // 이렇게 하면 배경이 카메라를 따라다니면서도 조금씩 밀려 보입니다.
        transform.position = new Vector3(startPosX + distance, transform.position.y, transform.position.z);
    }
}