using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float x1;  // 최소 x 위치
    public float x2;  // 최대 x 위치
    public float speed = 3f; // 이동 속도

    private bool movingToX2 = true; // 현재 이동 방향 체크

    void Update()
    {
        Vector3 pos = transform.position;

        // x 방향으로 이동
        if (movingToX2)
        {
            pos.x += speed * Time.deltaTime;
            if (pos.x >= x2)
                movingToX2 = false; // 반대로 전환
        }
        else
        {
            pos.x -= speed * Time.deltaTime;
            if (pos.x <= x1)
                movingToX2 = true; // 반대로 전환
        }

        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")||collision.collider.CompareTag("BouncePlatform"))
        {
            collision.collider.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("BouncePlatform"))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}

