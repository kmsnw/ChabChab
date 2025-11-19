using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//함정? 점프하다가 밑에서...

public class EnemyController : MonoBehaviour
{

    public PlayerController player; //PlayerMove ��ũ��Ʈ ��������
    private bool on = false;  // ������Ʈ�� ���������� ����
    public int stage = 0;

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // currentSceneName�� ���� �� �̸� �ޱ�

        if (currentSceneName == "TestScene")
        {
            this.stage = 1;
        }
        else if (currentSceneName == "Game111s")
        {
            this.stage = 1;
        }
        else if (currentSceneName == "Game222")
        {
            this.stage = 2;
        }
        else if ( currentSceneName == "Game222s")
        {
            this.stage = 2;
        }
    }
    void FixedUpdate()
    {
        if (player != null && stage == 1)
        {
            Vector3 playerPosition = player.transform.position; //�÷��̾� ��ǥ ��������

            if (playerPosition.x > transform.position.x -1.1 && playerPosition.y > 1) //�÷��̾� x��ǥ�� (������Ʈx��ǥ-1.1) �ʰ��ϰ�, �÷��̾� y��ǥ�� 3 �ʰ��ϸ�
            {
                // �ڿ� �������� �Ⱥ��̴� ������Ʈ ���̰�
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 1;  
                
                if (!on)
                {
                    on = true;
                }
            }
            moveUp();
        }

        // if (player != null && stage == 2)
        // {
        //     Vector2 playerPosition = player.GetCurrentPosition(); //�÷��̾� ��ǥ ��������
        //
        //     if (playerPosition.x > transform.position.x - 1.1 && playerPosition.y > 6) //�÷��̾� x��ǥ�� (������Ʈx��ǥ-1.1) �ʰ��ϰ�, �÷��̾� y��ǥ�� 6 �ʰ��ϸ�
        //     {
        //         // �ڿ� �������� �Ⱥ��̴� ������Ʈ ���̰�
        //         SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //         spriteRenderer.sortingOrder = 1;
        //         if (!on)
        //         {
        //             on = true;
        //         }
        //     }
        //     moveUp();
        // }
    }


    void moveUp()
    {
        if (on)
        {
            if (stage == 1)
                transform.Translate(0, 0.08f, 0);
            if (stage == 2)
                transform.Translate(0, 0.4f, 0);
        }
    }
}
