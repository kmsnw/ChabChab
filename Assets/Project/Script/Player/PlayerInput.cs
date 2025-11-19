using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 입력 관리
//조작키 설정 등..

public class PlayerInput : MonoBehaviour
{
    
    public KeyCode jumpKey = KeyCode.W;          // P1: W, P2: UpArrow
    public KeyCode interactKey = KeyCode.S;     // P1: S, P2: DownArrow
    public KeyCode leftKey = KeyCode.A;          // P1: A, P2: LeftArrow
    public KeyCode rightKey = KeyCode.D;         // P1: D, P2: RightArrow

    public float HorizontalInput { get; private set; }
    public bool JumpKeyDown { get; private set; } 
    public bool IsInteracting { get; private set; } // S / DownArrow 상태

    void Update()
    {
        // 수평 입력 처리
        HorizontalInput = 0;
        if (Input.GetKey(rightKey)) HorizontalInput = 1f;
        if (Input.GetKey(leftKey)) HorizontalInput = -1f;

        // 단발성 입력 (점프)
        JumpKeyDown = Input.GetKeyDown(jumpKey);
        
        // 지속성 입력 (상호작용/벽잡기)
        IsInteracting = Input.GetKey(interactKey);
    }
}
