using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//타이틀 씬 관리
public class TitleSceneManager : MonoBehaviour
{
    [Header("UI Buttons")] 
    public GameObject startButton;
    
    
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
    
    // 씬 전환 및 페이드 처리를 담당하는 코루틴
    private IEnumerator TransitionToGameScene()
    {
        isTransitioning = true;
        
        startButton.SetActive(false);
        
        // 1. 🖤 페이드 인: 화면을 검게 만들어 씬 전환을 숨깁니다.
        // ScreenFader 객체를 통해 FadeScreen 코루틴 호출
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeScreen(1f, fadeDuration)); 
        }
        else
        {
            Debug.LogError("ScreenFader가 연결되지 않아 페이드 없이 전환됩니다.");
        }
        
        // 2. 씬 전환 (검은 화면 상태에서 진행)
        SceneManager.LoadScene(gameSceneName);
    }
    //게임종료
    public void QuitGame()
    {
        
    }
}
