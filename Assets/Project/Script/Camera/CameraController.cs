using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라 컨트롤러
//스테이지 매니저가 산출한 이동할 카메라의 위치를 기반으로 선형보간 이동

public class CameraController : MonoBehaviour
{
    public Transform targetPlayer;
    
    //카메라 이동 속도
    [SerializeField] 
    private float moveSpeed = 5f;
    
    //이동할 위치
    private Vector3 _targetPosition;

    [Header("Viewport")]
    public Rect viewportRect = new Rect(0f, 0f, 0.5f, 1f);
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    
    private Camera _camera;


    private void Start()
    {
        _camera = GetComponent<Camera>();

        if (targetPlayer == null)
        {
            Debug.LogError("No target player found");
            enabled = false;
            return;
        }
        
        _camera.rect = viewportRect;
    }

    
    
    //StageManager에서 호출


    //LateUpdate 이용 선형보간
    private void LateUpdate()
    {
        
        // _targetPosition = targetPlayer.position + offset;
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
        // transform.position = smoothedPosition;
        //
        
        Vector3 desiredPosition = targetPlayer.position + offset;
    
        // Y축만 현재 카메라 위치로 덮어씌우기
        desiredPosition = new Vector3(desiredPosition.x, transform.position.y, desiredPosition.z);
        
    
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSpeed);
        transform.position = smoothedPosition;
    }
}






