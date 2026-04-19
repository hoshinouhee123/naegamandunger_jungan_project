using UnityEngine;

public class SnowballSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject snowballPrefab;

    // ★ 수정됨: 여러 개의 아이템 프리팹을 넣을 수 있는 '배열'로 변경
    public GameObject[] itemPrefabs;

    [Header("Spawn Settings")]
    public float spawnDelay = 0.5f;
    public float spawnRangeX = 5f;

    [Range(0f, 100f)]
    public float itemSpawnChance = 10f;

    void Start()
    {
        InvokeRepeating("SpawnSnowball", 1f, spawnDelay);
    }

    void SpawnSnowball()
    {
        float randomX = Random.Range(transform.position.x - spawnRangeX, transform.position.x + spawnRangeX);
        Vector2 spawnPos = new Vector2(randomX, transform.position.y);

        float randomValue = Random.Range(0f, 100f);

        // 아이템 확률에 당첨되었고 & 등록된 아이템 프리팹이 1개 이상 있다면
        if (randomValue <= itemSpawnChance && itemPrefabs != null && itemPrefabs.Length > 0)
        {
            // ★ 여러 아이템 중 하나를 무작위로 선택 (0번부터 등록된 개수-1 번까지)
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            GameObject selectedItem = itemPrefabs[randomIndex];

            if (selectedItem != null)
            {
                Instantiate(selectedItem, spawnPos, Quaternion.identity);
            }
            else
            {
                // 혹시 배열에 빈칸이 있다면 기본 눈덩이 생성
                Instantiate(snowballPrefab, spawnPos, Quaternion.identity);
            }
        }
        else
        {
            // 꽝이거나 아이템이 없을 때는 눈덩이 생성
            Instantiate(snowballPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 left = new Vector3(transform.position.x - spawnRangeX, transform.position.y, 0);
        Vector3 right = new Vector3(transform.position.x + spawnRangeX, transform.position.y, 0);
        Gizmos.DrawLine(left, right);
    }
}