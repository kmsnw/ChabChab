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
    private Collider2D _charCollider; // 접지 체크용 콜라이더
    private SpriteRenderer _spriteRenderer; // 방향 뒤집기용
    
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool isFacingRight = true; // 현재 바라보는 방향
    
    [Header("Ground Check & Coyote Time")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float jumpCoyoteTime = 0.15f; // 코요테 시간
    
    private bool isGrounded;
    private float coyoteTimeCounter;
    
    
    private Animator _animator; // Animator 컴포넌트 참조

    // Animator 파라미터 이름 (오타 방지를 위해 const로 정의 권장)
    private const string ANIM_IS_GROUNDED = "IsGrounded";
    private const string ANIM_IS_JUMPING = "IsJumping";
    private const string ANIM_IS_FALLING = "IsFalling";
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        //if (_animator == null) Debug.LogError("Animator component not found on Player.");
        
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
        {
            Debug.LogError("No rigidbody");
        }
        
        _charCollider = GetComponent<BoxCollider2D>();
        if (_charCollider == null)
        {
            Debug.LogError("No box collider");
        }
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("No sprite renderer");
        }
    }
    
    // --- Update (상태 관리) ---
    void Update()
    {
        // 1. 접지 상태 확인 및 코요테 시간 관리 (기존 로직 유지)
        isGrounded = CheckIfGrounded(); 
        if (isGrounded)
        {
            coyoteTimeCounter = jumpCoyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    
        // ----------------------------------------------------
        // 2. 점프 애니메이션 상태 업데이트 로직 추가
    
        if (_animator != null)
        {
            // IsGrounded 상태 전달 (가장 기본)
            _animator.SetBool(ANIM_IS_GROUNDED, isGrounded);

            // 땅에 닿아있지 않을 때만 점프/낙하 상태 판단
            if (!isGrounded)
            {
                // 상승 중 (점프 시작)
                if (_rigidBody.velocity.y > 0.01f) // 0.1f는 부동소수점 오차 방지
                {
                    _animator.SetBool(ANIM_IS_JUMPING, true);
                    _animator.SetBool(ANIM_IS_FALLING, false);
                }
                // 하강 중 (낙하)
                else if (_rigidBody.velocity.y < -0.01f)
                {
                    _animator.SetBool(ANIM_IS_JUMPING, false);
                    _animator.SetBool(ANIM_IS_FALLING, true);
                }
            }
            else // 땅에 닿았을 때
            {
                // 점프/낙하 플래그를 모두 초기화 (Idle, Run 애니메이션으로 전환 준비)
                _animator.SetBool(ANIM_IS_JUMPING, false);
                _animator.SetBool(ANIM_IS_FALLING, false);
            }
        }
        // ----------------------------------------------------
    }
    
    // --- Public Methods (로직 실행) ---
    
    public void Move(float horizontalInput)
    {
        float velocityX = speed * horizontalInput;
        _rigidBody.velocity = new Vector2(velocityX, _rigidBody.velocity.y);
        
        // 캐릭터 방향 뒤집기
        if (horizontalInput != 0)
        {
            Flip(horizontalInput);
        }
    }

    public void Jump()
    {
        if (coyoteTimeCounter > 0f)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, jumpForce);
            coyoteTimeCounter = 0f; 
        
            // 점프 애니메이션 시작 플래그 활성화
            if (_animator != null)
            {
                _animator.SetBool(ANIM_IS_JUMPING, true);
                // 점프와 동시에 IsFalling은 확실히 false여야 함
                _animator.SetBool(ANIM_IS_FALLING, false); 
            }
        }
    }
    
    // --- Private Helper Methods ---

    private bool CheckIfGrounded()
    {
        // 레이캐스팅 로직: 발밑에서 groundCheckDistance만큼 광선 발사
        Vector2 origin = _charCollider.bounds.center;
        origin.y = _charCollider.bounds.min.y; 

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, hit.collider != null ? Color.green : Color.red);
        
        return hit.collider != null;
    }

    private void Flip(float horizontalInput)
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            _spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            _spriteRenderer.flipX = true;
        }
    }
}
