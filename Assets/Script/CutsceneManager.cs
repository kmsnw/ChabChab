using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image comicCutPanel; // 만화 컷 이미지를 표시할 Image 컴포넌트
    [SerializeField] private TextMeshProUGUI nextPromptText;  // "Press any key..." 텍스트 컴포넌트
    
    [Header("Cutscene Data")]
    public Sprite[] comicCuts; // 만화 컷 이미지 배열 (에디터에서 연결)
    public string nextSceneName = "NextLevelScene"; // 만화 끝난 후 전환될 씬 이름
    
    
    [Header("Fade Panel")]
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private float cutTransitionTime = 0.3f;

    //
    // [Header("Text Blink Settings")]
    // public float blinkSpeed = 1.0f; // 깜빡이는 속도 (높을수록 빠름)
    //

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

        if (Input.anyKeyDown)
        {
            // 💡 호출 시점: 사용자 입력 감지 시 (컷 전환 시작)
            StartCoroutine(TransitionToNext());
        }
    }
    
    private IEnumerator StartInitialFadeOut()
    {
        // 💡 호출 시점: Start() 직후 (씬이 로드된 후 첫 번째 컷을 보여줄 때)
        // 0f: 투명도 0 (화면 공개)
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
            
            // 💡 Blink 코루틴을 시작하고 저장합니다. (Null 체크 후)
            if (_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);
            //_blinkCoroutine = StartCoroutine(BlinkText(nextPromptText)); 
            _blinkCoroutine = StartCoroutine(AnimateDots(nextPromptText));
            
        }
    }
    
    // ... (SetReadyForInput, BlinkText 함수는 동일) ...
    
    private void ShowCurrentCut()
    {
        // 페이드 아웃 후 호출되어 스프라이트만 교체합니다.
        if (currentCutIndex < comicCuts.Length)
        {
            comicCutPanel.sprite = comicCuts[currentCutIndex];
            
            // 씬 시작 시에는 StartInitialFadeOut에서 SetReadyForInput을 호출하므로 주석 처리
            // SetReadyForInput(); 
        }
    }

    private IEnumerator TransitionToNext()
    {
        // 입력 처리가 진행되는 동안 추가 입력을 막고 텍스트 깜빡임 중지
        isReadyForInput = false; 
        
        if(_blinkCoroutine != null) StopCoroutine(_blinkCoroutine);

        // 1. 🖤 페이드 인: 화면을 검게 만들어 컷 전환을 숨김
        // 💡 호출 시점: 컷 전환 직전 (다음 컷으로 넘어가기 위해 숨길 때)
        // 1f: 투명도 1 (완전 불투명)
        yield return StartCoroutine(screenFader.FadeScreen(1f, cutTransitionTime)); 

        // 2. 컷 인덱스 업데이트
        if (currentCutIndex < comicCuts.Length - 1)
        {
            currentCutIndex++;
            ShowCurrentCut(); // 검은 화면 뒤에서 다음 컷으로 스프라이트 교체
            
            // 3. 💡 페이드 아웃: 새로운 컷 공개
            // 💡 호출 시점: 스프라이트 교체 직후 (새로운 컷을 보여줄 때)
            yield return StartCoroutine(screenFader.FadeScreen(0f, cutTransitionTime)); 
            
            // 페이드 아웃 후 다음 입력을 받도록 준비
            SetReadyForInput();
        }
        // 4. 마지막 컷인 경우: 씬 전환
        else
        {
            // 씬 전환 시에는 이미 페이드 인 상태이므로 바로 로드
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    
    // private IEnumerator BlinkText(TextMeshProUGUI targetText)
    // {
    //     if (targetText == null) yield break;
    //
    //     Color originalColor = targetText.color;
    //
    //     while (true) // 무한 루프 (다음 씬으로 전환될 때까지)
    //     {
    //         // 1. 투명도 변화 계산: Mathf.PingPong을 사용해 0과 1 사이를 반복
    //         // Time.time * blinkSpeed를 입력으로 주면 시간에 따라 값이 반복됩니다.
    //         float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.0f); 
    //     
    //         // 2. 새로운 색상 적용: 원래 색상의 투명도(alpha)만 변경
    //         targetText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    //     
    //         yield return null; // 다음 프레임까지 대기
    //     }
    // }
    
    private IEnumerator AnimateDots(TextMeshProUGUI targetText)
    {
        if (targetText == null) yield break;

        // 점을 추가하는 간격 (깜빡임 속도와 연관됨)
        float delay = 1.0f / blinkSpeed; // blinkSpeed가 1.0이면 1초마다 변경

        while (true) // 무한 루프
        {
            for (int i = 0; i <= 3; i++) // 0개, 1개, 2개, 3개 순환
            {
                // 1. 점 문자열 생성 (i가 0이면 점 없음)
                string dots = new string('.', i); 
            
                // 2. 텍스트 내용 업데이트
                targetText.text = BASE_PROMPT + dots;
            
                // 3. 지정된 딜레이만큼 대기
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
