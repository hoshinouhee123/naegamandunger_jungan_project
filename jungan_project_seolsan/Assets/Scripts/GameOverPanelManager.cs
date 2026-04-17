using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 필요합니다!

public class GameOverPanelManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string firstStageName = "Stage_1_Scene"; // 첫 번째 스테이지 씬 이름
    public string mainMenuName = "MainMenu_Scene"; // 메인 화면 씬 이름

    // '다시 시작' 버튼에 연결할 함수
    public void RestartGame()
    {
        // 중요: 만약 게임 오버 시 Time.timeScale = 0f으로 멈췄다면 다시 1로 돌려줘야 합니다.
        Time.timeScale = 1f;

        // 지정한 첫 번째 스테이지 씬을 불러옵니다.
        SceneManager.LoadScene(firstStageName);
    }

    // '메인 화면' 버튼에 연결할 함수
    public void GoToMainMenu()
    {
        // 시간 흐름을 정상으로 돌려놓고 메인 메뉴로 이동합니다.
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuName);
    }
}