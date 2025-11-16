using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐릭터 무브먼트
//캐릭터 동작 관련(이동, 점프, 벽붙기, 벽점프..)
//AddForce 기반 


[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private AudioSource _audioSource;
    [Header("Sound Clips")]
    public AudioClip jumpSound;
    
    
    
    private Rigidbody2D _rigidBody;
    private Collider2D _charCollider;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
 
    [Header("Wall Jump Fix")]
    public float wallJumpInputDenialTime = 0.2f; // 벽 점프 후 벽점프 무시 시간
    private float inputDenialTimer = 0f; // 무시 타이머
    
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float jumpPower = 10f; // 점프 시 Impulse에 곱해질 값
    public float accelerationForce = 15f; // 수평 이동 시 AddForce에 곱해질 힘
    
    [Header("Collision Layers")]
    public LayerMask platformLayer; // "Platform" 레이어 (벽/땅 감지)
    
    // --- 상태 ---
    public bool isWallClinging { get; private set; } = false; // 벽 잡기 상태
    
    private bool isGrounded;

    //애니메이션 bool변수
    private const string ANIM_IS_GROUNDED = "isGrounded";
    private const string ANIM_IS_JUMPING = "isJumping";
    private const string ANIM_IS_FALLING = "isFalling";
    private const string ANIM_IS_WALLCLING = "isWallCling";
    private const string ANIM_IS_WALKING = "isWalking";
    
    
    void Start()
    {
        // _audioSource = GetComponent<AudioSource>();
        // if(_audioSource == null) Debug.LogError("CharacterMovement: audio not found");
        //
        
        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Animator component not found on Player.");
        
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null) Debug.LogError("No rigidbody");
        
        
        _charCollider = GetComponent<BoxCollider2D>();
        if (_charCollider == null) Debug.LogError("No box collider");
        
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) Debug.LogError("No sprite renderer");
        
    }


    void Update()
    {
        isGrounded = CheckIfGrounded();
        
        
        //애니메이션 변수 갱신
        if (_animator != null)
        {
            // 지면 여부 animator 전달
            _animator.SetBool(ANIM_IS_GROUNDED, isGrounded); 

            // 땅에 닿아있지 않고, 벽을 잡고 있지 않을 때만 점프/낙하 상태 판단
            if (!isGrounded && !isWallClinging)
            {
                //jumpping
                if (_rigidBody.velocity.y > 0.01f)
                {
                    _animator.SetBool(ANIM_IS_JUMPING, true);
                    _animator.SetBool(ANIM_IS_FALLING, false);
                }
                //falling
                else if (_rigidBody.velocity.y < -0.01f)
                {
                    _animator.SetBool(ANIM_IS_JUMPING, false);
                    _animator.SetBool(ANIM_IS_FALLING, true);
                }
            }
            else // isGround or isWallCling
            {
                // 낙하 플래그 초기화
                _animator.SetBool(ANIM_IS_FALLING, false);
            }
        }
    }

    void FixedUpdate()
    {
        if (inputDenialTimer > 0)
        {
            inputDenialTimer -= Time.fixedDeltaTime;
        }
        
        //지면 감지시(by rayCast) falling 애니메이션 종료
        // if (_rigidBody.velocity.y <= 0)
        // {
        //     Vector2 rayOrigin = (Vector2)_rigidBody.position + Vector2.down * 0.5f;
        //     RaycastHit2D rayHit = Physics2D.Raycast(
        //         rayOrigin, Vector2.down, 
        //         0.6f, 
        //         platformLayer
        //     );
        //
        //     // 지면에 닿았을 때 점프 애니메이션 종료
        //     if (rayHit.collider != null && rayHit.collider != _charCollider && rayHit.distance < 0.5f)
        //         _animator.SetBool(ANIM_IS_FALLING, false);
        // }
        // ----------------------------------------------------
        
        //걷기 애니메이션..
        
        // ----------------------------------------------------
    }
    
    // 이동
    public void Move(float horizontalInput)
    {
        if (horizontalInput != 0)
        {
            _rigidBody.AddForce(Vector2.right * horizontalInput * accelerationForce, ForceMode2D.Impulse);
            Flip(horizontalInput);
        }

        //속도 제한
        if (_rigidBody.velocity.x > maxSpeed) 
            _rigidBody.velocity = new Vector2(maxSpeed, _rigidBody.velocity.y);
        else if (_rigidBody.velocity.x < -maxSpeed) 
            _rigidBody.velocity = new Vector2(-maxSpeed, _rigidBody.velocity.y);
            
        //걷기 애니메이션 설정
        _animator.SetBool(ANIM_IS_WALKING, Mathf.Abs(_rigidBody.velocity.x) > 0.01f);
    }
    
    public void Jump(bool isJumpInput)
    {
        if (!isJumpInput) return;

        // Debug.Log("wallJump");
        
        //일반 점프 
        if (isGrounded && !_animator.GetBool(ANIM_IS_JUMPING)) 
        {
            // PMove 로직: y 속도 초기화 후 AddForce(Impulse)
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0f); 
            _rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            _animator.SetBool(ANIM_IS_JUMPING, true);
            _animator.SetBool(ANIM_IS_FALLING, false);
        }
        //벽 점프
        else if (isWallClinging)
        {
            //Debug.Log("Wall jump");
            bool isTouchingWallRight = CheckWallTouch(Vector2.right);
            int direction = isTouchingWallRight ? -1 : 1; // 점프 방향(벽 반대)
            
            //벽 반대방향 우상향 대각선 addForce
            _rigidBody.AddForce(new Vector2(direction, 1) * jumpPower, ForceMode2D.Impulse);
            
            //벽점프 무시 타이머 설정
            inputDenialTimer = wallJumpInputDenialTime;
            
            //벽점프 애니메이션 -> 지금은 그냥 점프?..
            _animator.SetBool(ANIM_IS_JUMPING, true);
            _animator.SetBool(ANIM_IS_FALLING, false);
            
            //벽 잡기 상태 해제
            isWallClinging = false;
            _rigidBody.gravityScale = 1; 
            _animator.SetBool(ANIM_IS_WALLCLING, false); 
            

        }
        
        
        //사운드..
        // if (_audioSource != null && jumpSound != null)
        // {
        //     _audioSource.PlayOneShot(jumpSound);
        // }
    }
    
    public void HandleWallCling(bool isClingInput)
    {
        // 벽 붙기 무시 타이머 작동중
        if (inputDenialTimer > 0)
        {
            _rigidBody.gravityScale = 1; 
            return; //벽 잡기 로직 무시
        }
        
        
        bool isTouchingWallRight = CheckWallTouch(Vector2.right);
        bool isTouchingWallLeft = CheckWallTouch(Vector2.left);

        // 벽 잡기 조건: 입력 키가 눌림, 벽에 닿음, 땅에 닿지 않음
        if (isClingInput && (isTouchingWallLeft || isTouchingWallRight) && !CheckIfGrounded())
        {
            // Debug.Log("벽 잡기");
            //속도 및 중력 제어(0)
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.gravityScale = 0;
            
            isWallClinging = true;
            
            //벽 잡기 애니메이션..
            _animator.SetBool(ANIM_IS_WALLCLING, true);
        }
        else if (isWallClinging)
        {
            //벽 잡기 해제
            _rigidBody.gravityScale = 1;
            isWallClinging = false;
            _animator.SetBool(ANIM_IS_WALLCLING, false);
        }
    }
    
    
    //플레이어 캐릭터 콜라이더 기준 바닥감지
    private bool CheckIfGrounded()
    {
        //콜라이더 바닥의 위치 및 크기
        Vector2 boundsCenter = _charCollider.bounds.center;
        Vector2 boundsExtents = _charCollider.bounds.extents; // 콜라이더 중심으로부터의 절반 크기
    
        
        float rayHorizontalOffset = boundsExtents.x - 0.05f; 
        const float groundRayDistance = 0.1f; //ray 길이
        
        // 1. 왼쪽 레이
        Vector2 originLeft = new Vector2(boundsCenter.x - rayHorizontalOffset, _charCollider.bounds.min.y);
        
        // 2. 오른쪽 레이
        Vector2 originRight = new Vector2(boundsCenter.x + rayHorizontalOffset, _charCollider.bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, groundRayDistance, platformLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, groundRayDistance, platformLayer);
    
        // Debug.DrawRay(originLeft, Vector2.down * groundRayDistance, Color.yellow);
        // Debug.DrawRay(originRight, Vector2.down * groundRayDistance, Color.yellow);

        //둘 중 캐스팅 되면 true
        return hitLeft.collider != null || hitRight.collider != null;
    }

    // 벽 감지 (벽 잡기/점프용)
    private bool CheckWallTouch(Vector2 direction)
    {
        //플레이어 캐릭터 콜라이더 너비 기반
        float colliderHalfWidth = _charCollider.bounds.size.x / 2f;
        float rayDistance = colliderHalfWidth + 0.1f; // 너비의 절반 + 여유분 0.1f

        RaycastHit2D hit = Physics2D.Raycast(
            _rigidBody.position, 
            direction, 
            rayDistance, 
            platformLayer
        );

        // Color rayColor = hit.collider != null ? Color.red : Color.green;
        // Debug.DrawRay(
        //     _rigidBody.position,                   
        //     direction * rayDistance,              
        //     rayColor,                              
        //     Time.fixedDeltaTime                    
        // );

        return hit.collider != null;
        
    }
    
    private void Flip(float horizontalInput) 
    { 
        if (horizontalInput > 0) _spriteRenderer.flipX = false;
        else if (horizontalInput < 0) _spriteRenderer.flipX = true;
    }
}
