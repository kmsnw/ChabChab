using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour
{
    private Renderer hide;  // 오브젝트의 Renderer

    void Start()
    {
        hide = GetComponent<Renderer>();

        // 초기 상태에서 오브젝트를 보이지 않게 설정
        hide.enabled = false;
    }

    // 플레이어와 충돌
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")  // 충돌한 객체 이름이 "Player"일 때
        {
            // 오브젝트를 보이게 설정
            hide.enabled = true;
        }
    }
}
