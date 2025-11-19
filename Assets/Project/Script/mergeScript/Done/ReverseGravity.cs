using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGravity : MonoBehaviour, IInteractable
{
    
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;

    public string InteractionName => "GravityReverser";

    public void Interact(bool isInteracting, PlayerController player)
    {
        if (isInteracting)
        {
            _playerMovement = player.GetComponent<PlayerMovement>();
            _playerInput = player.GetComponent<PlayerInput>();

            _playerMovement.animator.SetBool("isGravityReversed", true);
            
            if (_playerInput == null || _playerMovement == null) return;
            
            
            //중력반전 연출
            _playerMovement.ReverseGravityEffectCo();
            
            
            //스케일 변수 반전 및 중력 스케일 적용
            _playerMovement.gravityValue *= -1;
            _playerMovement.rigidBody.gravityScale = _playerMovement.gravityValue;
            
            //점프키 전환(W <--> S)
            _playerInput.IsGravityReverse = _playerMovement.gravityValue < 0; 
            
            
            
        } 
    }
}

