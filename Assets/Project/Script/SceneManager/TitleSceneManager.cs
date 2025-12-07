using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//타이틀 씬 관리

public class TitleSceneManager : MonoBehaviour
{
    [Header("Sound")]
    private AudioSource audioSource;
    public AudioClip titleSound;
    
    [Header("UI Buttons")] 
    public GameObject startButton;
    public GameObject howtoButton;
    public GameObject exitButton;
    public GameObject howToPanel;


    [Header("Fade Settings")]
    public ScreenFader screenFader; 
    public float fadeDuration = 0.5f;
    
    [Header("Scene Settings")]
    public string gameSceneName = "LoadingScene"; 
    
    private bool isTransitioning = false; // 중복 클릭 방지 플래그

    // 게임 시작 버튼 클릭 시 호출될 함수
    public void StartGame()
    {
  
        if (isTransitioning) return;
        
        // 씬 전환 코루틴 시작
        StartCoroutine(TransitionToGameScene());
    }
    
    // 씬 전환 및 페이드 처리 담당 코루틴
    private IEnumerator TransitionToGameScene()
    {
        startButton.SetActive(false);
        howtoButton.SetActive(false);
        exitButton.SetActive(false);
        
        isTransitioning = true;
        
        startButton.SetActive(false);
        
        //페이드인
        // ScreenFader 객체를 통해 FadeScreen 코루틴 호출
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
        }
        else
        {
            Debug.LogError("not connected ScreenFader");
        }
        
        // 씬 전환 (검은 화면 상태에서 진행)
        SceneManager.LoadScene(gameSceneName);
    }

    //HowTo 열기
    public void OpenHowTo()
    {
        howToPanel.SetActive(true);
    }

    //뒤로가기 (HowTo 닫기)
    public void CloseHowTo()
    {
        howToPanel.SetActive(false);
    }


    //게임종료
    public void QuitGame()
    {
       Application.Quit();
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = titleSound;
        audioSource.Play();

    }
}
