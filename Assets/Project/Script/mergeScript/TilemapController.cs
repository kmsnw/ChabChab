using System;
using UnityEngine;

// �� ��ũ��Ʈ�� ���� ������Ʈ�� �÷��̾ �浹 �� ��� tilemap�� Ȱ��ȭ/��Ȱ��ȭ�ϴ� ��ũ��Ʈ

public class TilemapController : MonoBehaviour
{
    public GameObject OnTilemap; // �۵��� �巯���� Ÿ�ϸ�
    public GameObject OffTilemap; // �۵��� ������� Ÿ�ϸ�

    private Animator animator;
    
    void Awake()
    {
        OnTilemap.SetActive(false); // ���� �� ����
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && this.CompareTag("Key"))
        {
            animator.SetBool("IsTrigger", true);
            
            if(OnTilemap != null)
                OnTilemap.SetActive(true);
            
            if(OffTilemap != null)
                OffTilemap.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && this.CompareTag("Key"))
        {
            animator.SetBool("IsTrigger", false);
            
            if(OnTilemap != null)
                OnTilemap.SetActive(false);
            
            if(OffTilemap != null)
                OffTilemap.SetActive(true);
        }
    } 
} 