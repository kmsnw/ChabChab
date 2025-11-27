using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject howToPanel;
    public GameObject PauseCanvas;
    
    
    public StageManager stageManager;

    private bool isPaused = false;

    void Update()
    {
        // ESC �Ǵ� PŰ�� �Ͻ����� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ���� �Ͻ�����
    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;  // ��ü ���� ����
    }

    // ���� �ٽ� ����
    public void ResumeGame()
    {
        Time.timeScale = 1f; // ���� �ӵ�

        isPaused = false;
        pausePanel.SetActive(false);
        howToPanel.SetActive(false);
        
        
    }

    // ���� Ÿ��Ʋ�� �̵�
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // �� �̵� �� �ݵ�� �������
        SceneManager.LoadScene("TitleScene");
    }

    // ���� ����
    public void QuitGame()
    {
        Application.Quit();
    }


    public void ReloadCheckpoint()
    {
        Time.timeScale = 1f;  // �Ͻ����� ���� �� ����
        stageManager.ReloadCheckPoint();
        pausePanel.SetActive(false);
    }



    // �Ͻ����� ���¿��� HowTo ����
    public void OpenHowTo()
    {
        howToPanel.SetActive(true);
    }

    public void CloseHowTo()
    {
        howToPanel.SetActive(false);
    }
}
