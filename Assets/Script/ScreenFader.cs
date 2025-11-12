using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        }
        if (fadeImage == null)
        {
            Debug.LogError("ScreenFader: Image 컴포넌트가 부착되어 있어야 합니다.");
        }
    }
    
    public IEnumerator FadeScreen(float targetAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            fadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        fadeImage.color = targetColor;
    }
}
