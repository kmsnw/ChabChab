using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//둘 다 상호작용키 누를 때 중력적용(발판 떨어짐??) -> cnt 변수 활용
//체크포인트 회귀: 시작 위치 복귀 및 중력 작용 설정 초기화


[RequireComponent(typeof(Rigidbody2D))]

public class InterTest : MonoBehaviour, IInteractable, ICheckpointSavable
{
    // public PlayerMove player;
    // public Player2Move player2;
    public string InteractionName => "interTest";

    private Rigidbody2D rb; //������ٵ�2D 

    //var == 2(모두 상호작용 키 눌렀을 때 작용)
    private int _isInteractingCnt = 0;
    
    
    //체크포인트 로드
    private Vector3 _initialPosition;
    private bool _initialState = true;
    
    
    public void Start()
    {
        _initialPosition = transform.position;
        
        
        rb = GetComponent<Rigidbody2D>(); //������ٵ�2D�� ������ rb�� �޾ƿ���
        rb.isKinematic = _initialState; // �߷�ȿ�� �ȹް�
    }

    // void Update()
    // {
    //     if ((player != null && player.isInteracting) || (player2 != null && player2.isInteracting))
    //     {
    //         DoInteract();
    //     }
    // }

    
    
    // void DoInteract()
    // {
    //     rb.isKinematic = false; // �߷�ȿ�� �ް�
    //     rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; // ȸ������, X�� �̵�����
    // }

    void Update()
    {
        //Debug.Log("cnt: " + _isInteractingCnt);
        
        if (!rb.isKinematic) return;
        
        if (_isInteractingCnt == 2)
        {
            rb.isKinematic = false; // �߷�ȿ�� �ް�
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }

        if (_isInteractingCnt < 0)
        {
            _isInteractingCnt = 0;
        }
    }

    //상호작용 동작: cnt조작
    public void Interact(bool isInteracting, PlayerController player)
    {
        if (isInteracting)
        {
            _isInteractingCnt++;
        }
        else
        {
            _isInteractingCnt--;
        }
    }

    //체크포인트 리스폰 시 복귀 
    public void SaveState()
    {
        
    }

    public void LoadState() //상태 초기화
    {
        transform.position = _initialPosition;
        rb.isKinematic = _initialState; //true
        rb.velocity = Vector3.zero;
        _isInteractingCnt = 0;
        
    }
}
