using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReverseGravity : MonoBehaviour, IInteractable, ICheckpointSavable
{
    
    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;
    
    
    public Camera FirstCamera;
    public Camera SecondCamera;
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

            if (player.gameObject.name == "FirstPlayer")
            {
                FlipCameraVertical(FirstCamera);

            }
            else if (player.gameObject.name == "SecondPlayer")
            {
                FlipCameraVertical(SecondCamera);
            }
            
            
            gameObject.SetActive(false);
            
        } 
    }
    
    
    // 화면 상하 반전, 좌우는 유지
    public void FlipCameraVertical(Camera cam)
    {
        Matrix4x4 mat = cam.projectionMatrix;
        mat *= Matrix4x4.Scale(new Vector3(1, -1, 1)); // y축만 반전
        cam.projectionMatrix = mat;
    }


    public void SaveState()
    {
    }
    
    public void LoadState() //상태 초기화
    {
        gameObject.SetActive(true);
        
        
        
        
        if (_playerMovement != null)
        {
            if (_playerMovement.gravityValue < 0)
            {
                _playerMovement.gravityValue = 1;
                
                if (_playerMovement.gameObject.name == "FirstPlayer")
                {
                    FlipCameraVertical(FirstCamera);

                }
                else if (_playerMovement.gameObject.name == "SecondPlayer")
                {
                    FlipCameraVertical(SecondCamera);
                }
                
            }
            
            
           
        }
    }
}







