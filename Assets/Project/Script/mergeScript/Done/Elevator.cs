using System.Collections.Generic;
using UnityEngine;

// �÷��̾� 2���� ���������Ϳ� �ö󰡸� ���������Ͱ� ����ϴ� ��ũ��Ʈ

public class Elevator : MonoBehaviour
{
    private float speed = 2f; // ��� �ӵ�
    private List<GameObject> playersOnElevator = new List<GameObject>();


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("elevator collision");
        if (collision.gameObject.CompareTag("Player")) // Player1, Player2 ��� Tag = "Player"�� ����
        {
            Debug.Log("elevator collision1");

            
            if (collision.contacts.Length > 0)
            {
                Debug.Log("elevator collision2");

                Vector2 normal = collision.contacts[0].normal;

                Debug.Log("normal: " + normal.y);
                if (normal.y < -0.8f)
                {
                    Debug.Log("elevator player insert");
                    if (!playersOnElevator.Contains(collision.gameObject))
                        playersOnElevator.Add(collision.gameObject);
                }
            }
            
        }
        
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playersOnElevator.Contains(collision.gameObject))
                playersOnElevator.Remove(collision.gameObject);
        }
    }

    void Update()
    {
        // �÷��̾ 2�� ��� �ö󰡸� ���
        if (playersOnElevator.Count >= 2)
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }
}