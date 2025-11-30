using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMoving : MonoBehaviour
{
    public float speed = 3f;

    // 이동 목표 지점 변수
    public float x1, y1;
    public float x2, y2;
    public float x3, y3;
    public float x4, y4;

    private Vector3[] points;       // 이동 지점을 담는 배열
    private int targetIndex = 0;    // 현재 목표 지점 번호

    void Start()
    {
        // 4개의 위치를 배열에 저장
        points = new Vector3[]
        {
            new Vector3(x1, y1, transform.position.z),
            new Vector3(x2, y2, transform.position.z),
            new Vector3(x3, y3, transform.position.z),
            new Vector3(x4, y4, transform.position.z)
        };
    }

    void Update()
    {
        // 현재 목표 지점
        Vector3 target = points[targetIndex];

        // 목표 지점까지 이동
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 목표 지점에 도착했는지 검사
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            // 다음 지점으로 변경
            targetIndex++;

            // 인덱스 초기화 (처음으로 되돌아가 반복)
            if (targetIndex >= points.Length)
            {
                targetIndex = 0;
            }
        }
    }
}