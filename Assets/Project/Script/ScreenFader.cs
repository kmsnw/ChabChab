using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Fade 연출(화면 검은색 투명도 조절해 부드럽게 넘어가는 연출..)
//Fade 위한 메서드 제공

public class ScreenFader : MonoBehaviour
{
// 페이드 패널 자체의 Image 컴포넌트 참조 (Awake에서 자동 획득 가능)
    [SerializeField] private Image fadeImage;

    void Awake()
    {
        // 스크립트가 붙은 오브젝트에서 Image 컴포넌트 자동 획득
        if (fadeImage == null)
        {
            fadeImage = GetComponent<Image>();
            //fade 이미지
        }
        if (fadeImage == null)
        {
            Debug.LogError("ScreenFader: not found fade image");
        }
    }
    
    //fade 진행
    public IEnumerator FadeScreen(float targetAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration; //진행률. t는 0~1. 
            fadeImage.color = Color.Lerp(startColor, targetColor, t); //투명도 차이 선형보간
            yield return null; //루프를 일시 중지. 현 프레임 나머지 렌더링 수행
        }
        
        fadeImage.color = targetColor;
    }
}
