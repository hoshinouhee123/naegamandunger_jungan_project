using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject HelpPanel;

    public GameObject StagePanel;

    public void GameStartButtonAction()
    {
        // 본인 첫 씬 이름 쓰기
        SceneManager.LoadScene("Stage_1_Scene");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void OpenHelpPanel()
    {
        HelpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        HelpPanel.SetActive(false);
    }

    public void GameStartButtonAction2()
    {
        SceneManager.LoadScene("Stage_2_Scene");
    }

    public void GameStartButtonAction3()
    {
        SceneManager.LoadScene("Stage_3_Scene");
    }

    public void GameStartButtonAction4()
    {
        SceneManager.LoadScene("Stage_4_Scene");
    }

    public void GameStartButtonAction5()
    {
        SceneManager.LoadScene("Stage_5_Scene");
    }

    public void OpenStagePanel()
    {
        StagePanel.SetActive(true);
    }

    public void CloseStagePanel()
    {
        StagePanel.SetActive(false);
    }

    public void MainQuitButtonAction()
    {
        // 메인 화면으로 이동
        SceneManager.LoadScene("MainMenu_Scene");
    }
}