using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//�÷��̾�� ���� �� �������� �����ϰ� �ϴ� ������ ��ũ��Ʈ

public class DoubleJumpItem : MonoBehaviour, ICheckpointSavable
{
    public bool canRespawn = false;
    
    [Header("Respawn Settings")] 
    public float respawnTime = 5f;
    
    
    private PlayerMovement _playerMovement;
    private RespawnController _respawnController;

    void Start()
    {
        _respawnController = FindObjectOfType<RespawnController>();
    }
    

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        _playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

        if (_playerMovement == null || !collision.CompareTag("Player")) return;

        _playerMovement.animator.SetBool("isDoubleJump", true);
        _playerMovement.isDoubleJump = true;
        
        
        Debug.Log("active double jump");
        _playerMovement.isDoubleJump = true;

        
        gameObject.SetActive(false);
        
        if (canRespawn)
        {
            
            _respawnController.StartCoroutine(_respawnController.RespawnRoutine(gameObject, respawnTime));

            
        }


        
    }
    
    public void SaveState()
    {
    }
    
    public void LoadState() //상태 초기화
    {
        gameObject.SetActive(true);
        
    }
    
}
