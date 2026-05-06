using System.Linq;
using UnityEngine;
using TMPro;

public class RankPage : MonoBehaviour
{
    [SerializeField] Transform contentRoot;     // Content 오브젝트
    [SerializeField] GameObject rowPrefab;      // RankRow 프리팹

    StageResultList allData;

    void Awake()
    {
        allData = StageResultSaver.LoadRank();
        RefreshRankList(1); // 처음 랭킹창을 열었을 땐 1스테이지 띄우기
    }

    public void RefreshRankList(int stageNumber)
    {
        // 기존 기록 삭제
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        // 선택한 스테이지(stageNumber) 데이터만 정렬
        var sortedData = allData.results.Where(r => r.stage == stageNumber).OrderByDescending(x => x.score).ToList();

        // UI 생성
        for (int i = 0; i < sortedData.Count; i++)
        {
            GameObject row = Instantiate(rowPrefab, contentRoot);
            TMP_Text rankText = row.GetComponentInChildren<TMP_Text>();
            rankText.text = $"{i + 1}. {sortedData[i].playerName} - {sortedData[i].score}";
        }
    }
}