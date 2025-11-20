using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject howToPanel;

    private bool isPaused = false;

    void Update()
    {
        // ESC 또는 P키로 일시정지 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // 게임 일시정지
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;  // 전체 게임 정지
    }

    // 게임 다시 시작
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        howToPanel.SetActive(false);
        Time.timeScale = 1f; // 정상 속도
    }

    // 메인 타이틀로 이동
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 씬 이동 전 반드시 원래대로
        SceneManager.LoadScene("TitleScene");
    }

    // 게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }

    // 일시정지 상태에서 HowTo 열기
    public void OpenHowTo()
    {
        howToPanel.SetActive(true);
    }

    public void CloseHowTo()
    {
        howToPanel.SetActive(false);
    }
}
