using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour, ICheckpointSavable
{
    public Rigidbody2D rb; //������ٵ�2D 
    
    //체크포인트 로드
    private Vector3 _initialPosition;
    private bool _initialState = true;
    
    public void Start()
    {
        _initialPosition = transform.position;
        
        
        rb = GetComponent<Rigidbody2D>(); //������ٵ�2D�� ������ rb�� �޾ƿ���
        rb.isKinematic = true; // �߷�ȿ�� �ȹް�
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") // �浹�� ��ü �±� "Player"�� ��
        {
            rb.isKinematic = false; // �߷�ȿ�� �ް�
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; // ȸ������, X�� �̵�����
        }
    }

    public void SaveState()
    {
        
    }

    public void LoadState()
    {
        transform.position = _initialPosition;
        rb.isKinematic = _initialState;
        
    }
}
