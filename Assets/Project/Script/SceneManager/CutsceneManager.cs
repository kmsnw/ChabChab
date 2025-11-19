using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//스테이지 클리어 이후 컷신 넘기는 연출

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image comicCutPanel; // 만화 컷 이미지를 표시할 Image 컴포넌트
    [SerializeField] private TextMeshProUGUI nextPromptText;  // "Press any key..." 
    
    [Header("Cutscene Data")]
    public Sprite[] comicCuts; // 만화 컷 이미지 배열 -> 에디터에서 연결
    public string nextSceneName = "NextLevelScene"; // 다음 씬 이름
    
    
    [Header("Fade Panel")]
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private float cutTransitionTime = 0.3f;
    
    private int currentCutIndex = 0;
    private bool isReadyForInput = false;
    private Coroutine _blinkCoroutine = null;

    private const string BASE_PROMPT = "Press any key";


    [Header("Dots Speed")] public float blinkSpeed = 1.0f;
    
void Start()
    {
        if (comicCuts.Length > 0)
        {
            ShowCurrentCut(); 
            StartCoroutine(StartInitialFadeOut());
        }
        else
        {
            Debug.LogError("Comic cut images are not set!");
        }
    }



    void Update()
    {
        if (!isReadyForInput) return;

        //사용자 입력 감지 시 (컷 전환 시작)
        if (Input.anyKeyDown)
        {
            StartCoroutine(TransitionToNext());
        }
    }
    
    private IEnumerator StartInitialFadeOut()
    {
        // Start() 직후 호출(씬 로드 이후 연출)
        // 코루틴 활용. -> 
        yield return StartCoroutine(screenFader.FadeScreen(0f, cutTransitionTime));
        
        // 페이드가 끝난 후 입력을 활성화하고 깜빡임 시작
        SetReadyForInput();
    }
    
    public void SetReadyForInput()
    {
        isReadyForInput = true;
        if (nextPromptText != null)
        {
            nextPromptText.gameObject.SetActive(true);
            
            // Blink 코루틴을 시작 및 저장
            if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = StartCoroutine(AnimateDots(nextPromptText));
            
        }
    }
    
    
    private void ShowCurrentCut()
    {
        // 페이드 아웃 후 호출. -> 스프라이트(스토리 컷) 변경
        if (currentCutIndex < comicCuts.Length)
        {
            comicCutPanel.sprite = comicCuts[currentCutIndex];
            
            // 씬 시작 시에는 StartInitialFadeOut에서 SetReadyForInput을 호출하므로 주석 처리
            // SetReadyForInput(); 
        }
    }
    
    //컷신 전환 코루틴
    private IEnumerator TransitionToNext()
    {
        // 입력 처리가 진행되는 동안 추가 입력을 막고 텍스트 깜빡임 중지
        isReadyForInput = false; 
        
        if(_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);

        // 페이드 인
        // 컷 전환 직전 (다음 컷으로 넘어가기 위해 숨길 때)
        // 1f: 투명도 1 (완전 불투명)
        yield return StartCoroutine(screenFader.FadeScreen(1f, cutTransitionTime)); 

        // 컷 인덱스 업데이트
        if (currentCutIndex < comicCuts.Length - 1)
        {
            currentCutIndex++;
            ShowCurrentCut(); // 검은 화면 뒤에서 다음 컷으로 스프라이트 교체
            
            // 페이드 아웃: 다음 컷
            yield return StartCoroutine(screenFader.FadeScreen(0f, cutTransitionTime)); 
            
            // 페이드 아웃 후 다음 입력을 받도록 준비
            SetReadyForInput();
        }
        // 마지막 컷: 씬 전환
        else
        {
            // 씬 전환 시에는 이미 페이드 인 상태이므로 바로 로드
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    //press any key ... 동적 변화
    private IEnumerator AnimateDots(TextMeshProUGUI targetText)
    {
        if (targetText == null) yield break;

        // 점을 추가하는 간격 (깜빡임 속도와 연관됨)
        float delay = 1.0f / blinkSpeed; // blinkSpeed가 1.0이면 1초마다 변경

        while (true) // 무한 루프
        {
            for (int i = 0; i <= 3; i++) // 0~3개 순환
            {
                // 점 문자열 생성 (i가 0이면 점 없음)
                string dots = new string('.', i); 
            
                // 텍스트 내용 업데이트
                targetText.text = BASE_PROMPT + dots;
            
                // 지정된 딜레이만큼 대기
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
