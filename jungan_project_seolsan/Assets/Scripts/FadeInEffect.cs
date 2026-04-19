using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInEffect : MonoBehaviour
{
    [Header("페이드 인 설정")]
    public Image fadeImage;
    public float fadeDuration = 2.0f;
    public float delayTime = 3.0f;

    [Header("오브젝트 이동 설정")]
    public Transform targetObject;
    public float moveSpeed = 50f;
    public float stopYPosition = 4247f; // ★ 추가됨: 멈출 Y축 좌표

    [Header("이동 완료 후 활성화할 오브젝트")]
    public GameObject objectToActivate; // ★ 추가됨: 도착 시 나타날 오브젝트 (버튼 등)

    void Start()
    {
        // 시작 시 활성화될 오브젝트는 미리 꺼둡니다.
        if (objectToActivate != null)
            objectToActivate.SetActive(false);

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            StartCoroutine(DoFadeIn());
        }
    }

    IEnumerator DoFadeIn()
    {
        yield return new WaitForSeconds(delayTime);

        float currentTime = 0f;
        Color color = fadeImage.color;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        if (targetObject != null)
        {
            StartCoroutine(MoveUpRoutine());
        }
    }

    IEnumerator MoveUpRoutine()
    {
        // 현재 Y위치가 목표 위치보다 낮은 동안 계속 반복
        // (UI인 경우 localPosition을 쓰는 것이 정확할 수 있습니다)
        while (targetObject.localPosition.y < stopYPosition)
        {
            targetObject.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 목표 위치에 도달하면 위치를 정확히 고정 (약간 지나칠 수 있으므로)
        Vector3 finalPos = targetObject.localPosition;
        finalPos.y = stopYPosition;
        targetObject.localPosition = finalPos;

        // ★ 도착 완료! 지정한 오브젝트를 켭니다.
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log("목표 지점 도달! 오브젝트 활성화.");
        }
    }
}