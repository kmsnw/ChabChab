using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//�÷��̾�� ���� �� �������� �����ϰ� �ϴ� ������ ��ũ��Ʈ

public class DoubleJumpItem : MonoBehaviour
{
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
        
        _respawnController.StartCoroutine(_respawnController.RespawnRoutine(gameObject, respawnTime));
        
        
    }
    
}
