using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    
    //충돌판정
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") // 충돌한 객체 태그가 "Player"일 때
        {
            Destroy(gameObject);
        }
        
    }

}
