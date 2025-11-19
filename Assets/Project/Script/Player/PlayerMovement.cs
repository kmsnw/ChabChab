using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐릭터 무브먼트
//캐릭터 동작 관련(이동, 점프, 벽붙기, 벽점프..)
//AddForce 기반 


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private AudioSource _audioSource;
    [Header("Sound Clips")]
    public AudioClip jumpSound;
    
    
    
    
    private Collider2D _charCollider;
    private SpriteRenderer _spriteRenderer;
    //private groundCheckRay _groundCheckRay;
    public Animator animator;
    public Rigidbody2D rigidBody;

    
    [Header("Wall Jump Fix")]
    public float wallJumpInputDenialTime = 3f; // 벽 점프 후 벽점프 무시 시간
    private float inputDenialTimer = 0f; // 무시 타이머
    
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float jumpPower = 10f; // 점프 시 Impulse에 곱해질 값
    public float accelerationForce = 15f; // 수평 이동 시 AddForce에 곱해질 힘
    public float gravityValue = 1f;
    
    [Header("Collision Layers")]
    public LayerMask platformLayer; // "Platform" 레이어 (벽/땅 감지)
    
    // --- 상태 ---
    public bool isWallClinging { get; private set; } = false; // 벽 잡기 상태
    private bool canWallCling = true;
    
    private bool isGrounded;
    public bool isDoubleJump = false;
    
    
    //애니메이션 bool변수
    private const string ANIM_IS_GROUNDED = "isGrounded";
    private const string ANIM_IS_JUMPING = "isJumping";
    private const string ANIM_IS_FALLING = "isFalling";
    private const string ANIM_IS_WALLCLING = "isWallCling";
    private const string ANIM_IS_WALKING = "isWalking";
    private const string ANIM_IS_DOUBLEJUMP = "isDoubleJump";
    
    void Start()
    {
        // _audioSource = GetComponent<AudioSource>();
        // if(_audioSource == null) Debug.LogError("CharacterMovement: audio not found");
        //
        
        // _groundCheckRay = GetComponent<groundCheckRay>();
        // if(_groundCheckRay == null) Debug.LogError("Ground Check Ray is null");
        
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator component not found on Player.");
        
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null) Debug.LogError("No rigidbody");
        
        
        _charCollider = GetComponent<BoxCollider2D>();
        if (_charCollider == null) Debug.LogError("No box collider");
        
        
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) Debug.LogError("No sprite renderer");
        
    }


    void Update()
    {
        isGrounded = CheckIfGrounded();
        
        if (isGrounded)
        {
            
           canWallCling = true;
            
            if(isDoubleJump) isDoubleJump = false;
        }
        
        //애니메이션 변수 갱신
        if (animator != null)
        {
            // 지면 여부 animator 전달
            animator.SetBool(ANIM_IS_GROUNDED, isGrounded); 

            // 땅에 닿아있지 않고, 벽을 잡고 있지 않을 때만 점프/낙하 상태 판단
            if (!isGrounded && !isWallClinging)
            {
                //jumpping
                if (gravityValue > 0f)
                {
                    if (rigidBody.velocity.y > 0.01f)
                    {
                        animator.SetBool(ANIM_IS_JUMPING, true);
                        animator.SetBool(ANIM_IS_FALLING, false);
                    }
                    //falling
                    else if (rigidBody.velocity.y < -0.01f)
                    {
                        animator.SetBool(ANIM_IS_JUMPING, false);
                        animator.SetBool(ANIM_IS_FALLING, true);
                    }
                }
                else
                {
                    if (rigidBody.velocity.y < -0.01f)
                    {
                        Debug.Log("reverse jumping");
                        animator.SetBool(ANIM_IS_JUMPING, true);
                        animator.SetBool(ANIM_IS_FALLING, false);
                    }
                    //falling
                    else if (rigidBody.velocity.y > 0.01f)
                    {
                        Debug.Log("reverse falling");
                        animator.SetBool(ANIM_IS_JUMPING, false);
                        animator.SetBool(ANIM_IS_FALLING, true);
                    }
                    
                }
            }

            
            else // isGround or isWallCling
            {
                // 낙하 플래그 초기화
                animator.SetBool(ANIM_IS_FALLING, false);
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
            rigidBody.AddForce(Vector2.right * horizontalInput * accelerationForce, ForceMode2D.Impulse);
            Flip(horizontalInput);
        }

        //속도 제한
        if (rigidBody.velocity.x > maxSpeed) 
            rigidBody.velocity = new Vector2(maxSpeed, rigidBody.velocity.y);
        else if (rigidBody.velocity.x < -maxSpeed) 
            rigidBody.velocity = new Vector2(-maxSpeed, rigidBody.velocity.y);
            
        //걷기 애니메이션 설정
        animator.SetBool(ANIM_IS_WALKING, Mathf.Abs(rigidBody.velocity.x) > 0.01f);
    }
    
    public void Jump(bool isJumpInput)
    {
        if (!isJumpInput) return;

        // Debug.Log("wallJump");
        
        //일반 점프 
        if (isGrounded && !animator.GetBool(ANIM_IS_JUMPING)) 
        {
            Debug.Log("Jumping");
            // PMove 로직: y 속도 초기화 후 AddForce(Impulse)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f); 
            rigidBody.AddForce(Vector2.up * jumpPower * gravityValue, ForceMode2D.Impulse);
            animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetBool(ANIM_IS_FALLING, false);
        }
        //벽 점프
        else if (isWallClinging)
        {
            Debug.Log("Wall jump");
            bool isTouchingWallRight = CheckWallTouch(Vector2.right);
            int direction = isTouchingWallRight ? -1 : 1; // 점프 방향(벽 반대)
            
            //벽 반대방향 상향 대각선 addForce
            Vector2 jumpVector = new Vector2(direction, 1f *gravityValue);
            rigidBody.AddForce(jumpVector * jumpPower, ForceMode2D.Impulse);
            
            //벽점프 무시 타이머 설정
            inputDenialTimer = wallJumpInputDenialTime;
            
            //벽점프 애니메이션 -> 지금은 그냥 점프?..
<<<<<<<< HEAD:Assets/Project/Script/PlayerMovement.cs
            animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetBool(ANIM_IS_FALLING, false);
========
            //_animator.SetBool(ANIM_IS_JUMPING, true);
            _animator.SetTrigger("WallJumpTrigger");
            _animator.SetBool(ANIM_IS_FALLING, false);
>>>>>>>> 1f7f99ed0226d20a9eac7125789e2141807b5fad:Assets/Project/Script/Player/PlayerMovement.cs
            
            //벽 잡기 상태 해제
            isWallClinging = false;
            rigidBody.gravityScale = gravityValue;
            
            animator.SetBool(ANIM_IS_WALLCLING, false); 
            
            canWallCling = false;
        }
        else if (isDoubleJump)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f); 
            rigidBody.AddForce(Vector2.up * jumpPower * gravityValue, ForceMode2D.Impulse);

            animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetBool(ANIM_IS_FALLING, false);
            
            animator.SetBool(ANIM_IS_DOUBLEJUMP, false);
            isDoubleJump = false;
        }
        
        
        //사운드..
        // if (_audioSource != null && jumpSound != null)
        // {
        //     _audioSource.PlayOneShot(jumpSound);
        // }
    }
    
    public void HandleWallCling(bool isClingInput)
    {
        if(!canWallCling) return;
        
        //벽 붙기 무시 타이머 작동중
        if (inputDenialTimer > 0)
        {
            rigidBody.gravityScale = gravityValue; 
            return; //벽 잡기 로직 무시
        }
        
        
        bool isTouchingWallRight = CheckWallTouch(Vector2.right);
        bool isTouchingWallLeft = CheckWallTouch(Vector2.left);

        // 벽 잡기 조건: 입력 키가 눌림, 벽에 닿음, 땅에 닿지 않음
        if (isClingInput && (isTouchingWallLeft || isTouchingWallRight) && !CheckIfGrounded())
        {
            // Debug.Log("벽 잡기");
            //속도 및 중력 제어(0)
            rigidBody.velocity = Vector2.zero;
            rigidBody.gravityScale = 0;
            
            isWallClinging = true;
            
            //벽 잡기 애니메이션..
<<<<<<<< HEAD:Assets/Project/Script/PlayerMovement.cs
            animator.SetBool(ANIM_IS_WALLCLING, true);
========
            Debug.Log("jumping false");
            _animator.SetBool(ANIM_IS_JUMPING, false);
            _animator.SetBool(ANIM_IS_WALLCLING, true);
>>>>>>>> 1f7f99ed0226d20a9eac7125789e2141807b5fad:Assets/Project/Script/Player/PlayerMovement.cs
        }
        else if (isWallClinging)
        {
            //벽 잡기 해제
            rigidBody.gravityScale = gravityValue;
            isWallClinging = false;
            animator.SetBool(ANIM_IS_WALLCLING, false);
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
        Vector2 rayDirection = Vector2.down * gravityValue;
        
        float originY = (gravityValue >0) ? _charCollider.bounds.min.y : _charCollider.bounds.max.y;
        
        // 1. 왼쪽 레이
        Vector2 originLeft = new Vector2(boundsCenter.x - rayHorizontalOffset, originY);
        
        // 2. 오른쪽 레이
        Vector2 originRight = new Vector2(boundsCenter.x + rayHorizontalOffset, originY);
        
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, rayDirection, groundRayDistance, platformLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, rayDirection, groundRayDistance, platformLayer);
        
        // Debug.DrawRay(originLeft, Vector2.down * groundRayDistance, Color.yellow);
        // Debug.DrawRay(originRight, Vector2.down * groundRayDistance, Color.yellow);
        
        //둘 중 캐스팅 되면 true
        return hitLeft.collider != null || hitRight.collider != null;

        //return _groundCheckRay.CheckIfGrounded();
    }

    // 벽 감지 (벽 잡기/점프용)
    private bool CheckWallTouch(Vector2 direction)
    {
        //플레이어 캐릭터 콜라이더 너비 기반
        float colliderHalfWidth = _charCollider.bounds.size.x / 2f;
        float rayDistance = colliderHalfWidth + 0.1f; // 너비의 절반 + 여유분 0.1f

        RaycastHit2D hit = Physics2D.Raycast(
            rigidBody.position, 
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
