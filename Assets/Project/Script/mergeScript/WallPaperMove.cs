using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPaperMove : MonoBehaviour
{
    public PlayerController player; //PlayerMove ����
    public PlayerController player2; //Player2Move ����
    private float backGroundY;
    
    void Start()
    {
        backGroundY = FindObjectOfType<Camera>().transform.position.y;
    }
    void Update()
    {
        // �÷��̾� ��ġ�� ���� ���ȭ��x��ǥ�� ���� �̵�. ī�޶� �̵��� ����
        if (player != null)
        {
            Vector2 playerPosition = player.transform.position;
            float newX = playerPosition.x;
            transform.position = new Vector3(newX, backGroundY, -6);
        }
        if (player2 != null)
        {
            Vector2 playerPosition = player2.transform.position;
            float newX = playerPosition.x;
            transform.position = new Vector3(newX, backGroundY, -6);
        }

    }
}
