using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어 컨트롤러
//플레이어 조작 관리(상호작용, 이동...)

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    public PlayerMovement _movement;
    
    private IInteractable _currentInteractable = null;
    private bool _currentInteractState = false;
    private bool _lastInteractingState = false;
    
    private Animator _animator;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //트리거된 오브젝트 캐스팅
        IInteractable interactable = other.GetComponent<IInteractable>();
        
        //캐스팅성공 -> 상호작용 가능 오브젝트(인터페이스(IInteractable) 구현 오브젝트)
        if (interactable != null)
            _currentInteractable = interactable; //현재 상호작용 가능한 오브젝트 할당
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //트리거 해제된 순간 null
        _currentInteractable = null;
    }

    void Start()
    {
        
    }

    //일정 시간간격 호출
    private void FixedUpdate()
    {
        //horizontal 값 기반 이동함수(좌우) 프레임마다 호출
        _movement.Move(_playerInput.HorizontalInput);
        
        //벽 붙기..
        _movement.HandleWallCling(_playerInput.IsInteracting);
    }

    void Update()
    {
        
        if (_playerInput.JumpKeyDown)
        {
            _movement.Jump(_playerInput.JumpKeyDown);
        }
        
        //상호작용 키 누름 -> true
        _currentInteractState = _playerInput.IsInteracting;

        //최적화: 상호작용 시작(키 눌렀을 때), 종료(땠을 때) 에만 Interact 함수 호출
        if (_currentInteractState != _lastInteractingState)
        {
            if (_currentInteractable != null)
            {
                //Interact(): 상호작용 가능 오브젝트에서 구현
                _currentInteractable.Interact(_currentInteractState, this);
            }

            _lastInteractingState = _currentInteractState;
        }

    }
}
