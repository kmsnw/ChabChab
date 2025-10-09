using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 입력 관리
//조작키 설정 등..

public class PlayerInput : MonoBehaviour
{


    private float _horizontalInput = 0f;
    public float HorizontalInput {get {return _horizontalInput;}}
    
    //각 플레이어별 조작키 설정 -> 변경, 확장 가능..
    public KeyCode right;
    public KeyCode left;
    public KeyCode interactKey;
    
    
    void Update()
    {
        //좌, 우 입력기반 수평 방향 값 산출
        _horizontalInput = 0;
        if (Input.GetKey(right)) _horizontalInput = 1f;
        if (Input.GetKey(left)) _horizontalInput = -1f;
    }
}
