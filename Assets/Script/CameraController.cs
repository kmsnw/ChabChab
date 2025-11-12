using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라 컨트롤러
//스테이지 매니저가 산출한 이동할 카메라의 위치를 기반으로 선형보간 이동

public class CameraController : MonoBehaviour
{
    //카메라 이동 속도
    [SerializeField] 
    private float moveSpeed = 5f;
    
    //이동할 위치
    private Vector3 _targetPosition;


    //StageManager에서 호출
    public void SetTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    //LateUpdate 이용 선형보간
    private void LateUpdate()
    {
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveSpeed);
        transform.position = smoothedPosition;
    }
}






