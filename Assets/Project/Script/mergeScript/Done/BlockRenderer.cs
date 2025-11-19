using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockRenderer : MonoBehaviour
{
    private Renderer blockRenderer;  // 박스 블록의 Renderer
    private Collider2D blockCollider;  // 박스 블록의 Collider
    //private PlayerMove player;  //PlayerMove 스크립트 가져오기

    void Start()
    {
        // 박스 블록의 Renderer 컴포넌트를 가져오기
        blockRenderer = GetComponent<Renderer>();

        // 박스 블록의 Collider2D 컴포넌트를 가져오기
        blockCollider = GetComponent<Collider2D>();

        // 초기 상태에서 박스 블록을 보이지 않게 설정
        blockRenderer.enabled = false;

        // 박스 블록의 충돌을 비활성화 (isTrigger를 true로 설정)
        blockCollider.isTrigger = true;
    }

    // 플레이어와 Trigger 충돌했을 때 호출되는 메서드
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 객체가 "Player" 태그를 가진 경우에만 실행
        if (collision.gameObject.tag == "Player")
        {
            // 플레이어의 Rigidbody2D 가져오기
            Rigidbody2D playerRigidbody = collision.attachedRigidbody;

            // 플레이어가 아래에서 위로 이동하며 충돌한 경우
            if (playerRigidbody != null && playerRigidbody.velocity.y > 0)
            {
                // 박스 블록을 보이게 설정
                blockRenderer.enabled = true;

                // 물리적 충돌 활성화 (isTrigger를 false로 설정)
                blockCollider.isTrigger = false;
            }
        }
    }
}