using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐릭터 무브먼트
//이동관련.. -> 현 기준 단순 좌우이동(move)만 존재
//몬스터 이동도 horizontal을 조작하는 방식을 달리해 응용 가능할 듯



[RequireComponent(typeof(Rigidbody2D))]

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    [Header("Movement")]
    public float speed = 5f;


    //horizontal 값 기반 오브젝트 이동
    public void Move(float horizontalInput)
    {
        float velocityX = speed * horizontalInput;
        _rigidBody.velocity = new Vector2(velocityX, _rigidBody.velocity.y);
    }
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        
    }
    //일정 시간간격 호출

}
