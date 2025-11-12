using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCameraMover : MonoBehaviour
{
    
    private Camera _camera;
    private float _initialOrthographicSize; //초기 줌
    
    //컷신 최종 줌 크기
    public float targetOrthographicSize = 1f;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera != null && _camera.orthographic)
        {
            //설정된 카메라 줌값으로 초기화
            _initialOrthographicSize = _camera.orthographicSize;
        }
    }
    
    

    public void MoveToDoor(Transform doorTarget, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMoveAndZoom(doorTarget, duration));
    }

    private IEnumerator SmoothMoveAndZoom(Transform doorTarget, float duration)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(doorTarget.position.x, doorTarget.position.y, startPosition.z);
        
        float startSize = _camera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            
            //워치 선형 보간
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (_camera != null && _camera.orthographic)
            {
                _camera.orthographicSize = Mathf.Lerp(startSize, targetOrthographicSize, t);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
            
        }
        
        //최종 위치 및 줌 크기 보정
        transform.position = targetPosition;
        if (_camera != null && _camera.orthographic)
        {
            _camera.orthographicSize = targetOrthographicSize;
        }
        
    }
    
}
