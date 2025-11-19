using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheckRay : MonoBehaviour
{

    private Collider2D _playerCollider;
    
    [Header("Collision Layers")]
    public LayerMask platformLayer; // "Platform" 레이어 (벽/땅 감지)
    
    void Awake()
    {
        _playerCollider = GetComponent<Collider2D>();
    }

    public bool CheckIfGrounded()
    {
        //콜라이더 바닥의 위치 및 크기
        Vector2 boundsCenter = _playerCollider.bounds.center;
        Vector2 boundsExtents = _playerCollider.bounds.extents; // 콜라이더 중심으로부터의 절반 크기
    
        
        float rayHorizontalOffset = boundsExtents.x - 0.05f; 
        const float groundRayDistance = 0.1f; //ray 길이
        
        // 1. 왼쪽 레이
        Vector2 originLeft = new Vector2(boundsCenter.x - rayHorizontalOffset, _playerCollider.bounds.min.y);
        
        // 2. 오른쪽 레이
        Vector2 originRight = new Vector2(boundsCenter.x + rayHorizontalOffset, _playerCollider.bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, groundRayDistance, platformLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, groundRayDistance, platformLayer);
    
        // Debug.DrawRay(originLeft, Vector2.down * groundRayDistance, Color.yellow);
        // Debug.DrawRay(originRight, Vector2.down * groundRayDistance, Color.yellow);

        //둘 중 캐스팅 되면 true
        return hitLeft.collider != null || hitRight.collider != null;
    }
}
