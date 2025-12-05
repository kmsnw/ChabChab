using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour, ICheckpointSavable
{

    public bool twoPlayerInteract = false;
    public int interactCount = 0;

    
    public Rigidbody2D rb; //������ٵ�2D 
    
    //체크포인트 로드
    private Vector3 _initialPosition;
    private bool _initialState = true;
    
    public void Start()
    {
        _initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        
    }

    void Update()
    {
        if (twoPlayerInteract)
        {
            if (interactCount == 2)
            {
                rb.isKinematic = false;
            }
            
        }
        else
        {
            if (interactCount >= 1)
            {
                rb.isKinematic = false;
            }
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
    
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
             transform.SetParent(collision.transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
    
  
}
