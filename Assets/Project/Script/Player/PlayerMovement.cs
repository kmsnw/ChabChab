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
    public float wallJumpInputDenialTime = 0.1f; // 벽 점프 후 벽점프 무시 시간
    private float inputDenialTimer = 0f; // 무시 타이머
    
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float jumpPower = 10f; // 점프 시 Impulse에 곱해질 값
    public float accelerationForce = 15f; // 수평 이동 시 AddForce에 곱해질 힘
    public float gravityValue = 1f;
    
    [Header("Collision Layers")]
    public LayerMask platformLayer; // "Platform" 레이어 (벽/땅 감지)
    
    
    [Header("Gravity Reverse Effect")]
    public float rotationSpeed = 360f; // 초당 회전 각도 (예: 1초에 360도)
    private Quaternion targetRotation; // 목표 회전값
    private bool isRotating = false; // 현재 회전 중인지 여부
    public float liftTime = 0.5f;        // 캐릭터가 서서히 뜰 시간
    public float liftHeight = 0.5f;      // 캐릭터가 뜰 높이
    public float inputLockDuration = 1.0f; // 입력 잠금 유지 시간 (회전 시간 포함)
    private bool isControlLocked = false;
    
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
    public const string ANIM_IS_GRAVITYREVERSED = "isGravityReversed";
    
    

    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null) Debug.LogError("CharacterMovement: audio not found");
        
        
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
           animator.SetBool(ANIM_IS_GRAVITYREVERSED, false);

           if (isDoubleJump)
           {
               isDoubleJump = false;
               
           }
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
                        animator.SetBool(ANIM_IS_JUMPING, true);
                        animator.SetBool(ANIM_IS_FALLING, false);
                    }
                    //falling
                    else if (rigidBody.velocity.y > 0.01f)
                    {
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
        
        if (isRotating)
        {
            // Lerp 또는 Slerp로 부드럽게 회전
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );

            // 목표 회전에 거의 도달하면 회전 중지
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // 정확히 목표 각도로 설정
                isRotating = false; // 회전 종료
            }
        }
        
    }

    void FixedUpdate()
    {
        if (inputDenialTimer > 0)
        {
            inputDenialTimer -= Time.fixedDeltaTime;
        }

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

        bool jumpSuccessful = false;
        // Debug.Log("wallJump");
        
        //일반 점프 
        if (isGrounded && !animator.GetBool(ANIM_IS_JUMPING)) 
        {
            // PMove 로직: y 속도 초기화 후 AddForce(Impulse)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f); 
            rigidBody.AddForce(Vector2.up * jumpPower * gravityValue, ForceMode2D.Impulse);
            animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetBool(ANIM_IS_FALLING, false);
            
            jumpSuccessful = true;
        }
        //벽 점프
        else if (isWallClinging)
        {
            bool isTouchingWallRight = CheckWallTouch(Vector2.right);
            int direction = isTouchingWallRight ? -1 : 1; // 점프 방향(벽 반대)
            
            //벽 반대방향 상향 대각선 addForce
            Vector2 jumpVector = new Vector2(direction, 1f *gravityValue);
            rigidBody.AddForce(jumpVector * jumpPower, ForceMode2D.Impulse);
            
            //벽점프 무시 타이머 설정
            inputDenialTimer = wallJumpInputDenialTime;
            
            // //벽점프 애니메이션 -> 지금은 그냥 점프?..
            // animator.SetBool(ANIM_IS_JUMPING, true);
            // animator.SetBool(ANIM_IS_FALLING, false);
            //_animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetTrigger("WallJumpTrigger");
            //animator.SetBool(ANIM_IS_FALLING, false);

            //벽 잡기 상태 해제
            isWallClinging = false;
            rigidBody.gravityScale = gravityValue;
            
            animator.SetBool(ANIM_IS_WALLCLING, false); 
            
            canWallCling = false;
            
            jumpSuccessful = true;
        }
        //더블 점프(아이템 습득)
        else if (isDoubleJump)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f); 
            rigidBody.AddForce(Vector2.up * jumpPower * gravityValue, ForceMode2D.Impulse);

            animator.SetBool(ANIM_IS_JUMPING, true);
            animator.SetBool(ANIM_IS_FALLING, false);
            
            Debug.Log("더블점프 state change");
            isDoubleJump = false;
            
            jumpSuccessful = true;
        }

        if (jumpSuccessful)
        {
            _audioSource.PlayOneShot(jumpSound);
        }

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
            // if (gravityValue < 0f)
            // {
            //     _spriteRenderer.flipX = false;
            // }

            // Debug.Log("벽 잡기");
            //속도 및 중력 제어(0)
            rigidBody.velocity = Vector2.zero;
            rigidBody.gravityScale = 0;
            
            isWallClinging = true;
            
            bool shouldFlip = isTouchingWallLeft;
            if (gravityValue < 0f)
            {
                shouldFlip = !shouldFlip;
            }
            _spriteRenderer.flipX = shouldFlip;
            
            
            //벽 잡기 애니메이션..
            animator.SetBool(ANIM_IS_WALLCLING, true);

            //Debug.Log("jumping false");
            animator.SetBool(ANIM_IS_JUMPING, false);
            animator.SetBool(ANIM_IS_WALLCLING, true);
        }
        else if (isWallClinging)
        {
            //벽 잡기 해제
            rigidBody.gravityScale = gravityValue;
            isWallClinging = false;
            animator.SetBool(ANIM_IS_WALLCLING, false);
            
            // if (gravityValue < 0f)
            // {
            //     _spriteRenderer.flipX = true;
            // }
            
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
    
    
    public Coroutine ReverseGravityEffectCo()
    {
        // 이미 연출 중이면 중복 방지
        if (isControlLocked) return null; 

        // 모든 제어를 멈추고 연출 코루틴 시작
        return StartCoroutine(ReverseGravityFlow());
    }
    
    //중력반전 연출
    private IEnumerator ReverseGravityFlow()
    {
        isControlLocked = true;
    
        rigidBody.velocity = Vector2.zero; 

    
        rigidBody.gravityScale = 0; 
    
        // 서서히 뜨는 연출 (liftTime 동안)
        float verticalDirection = Mathf.Sign(gravityValue); 

        Vector3 startPos = transform.position; 
        
        Vector3 endPos = startPos + new Vector3(0, liftHeight * verticalDirection, 0); 
        float elapsed = 0f;

        while (elapsed < liftTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / liftTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        
        // 목표 회전 설정 (180도 or 0도)
        targetRotation = (gravityValue < 0) ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
        isRotating = true; 
    
        
        // 회전이 완료될 때까지 대기
        yield return new WaitForSeconds(inputLockDuration); 
    
        // 조작 활성화
        isControlLocked = false;
    }
}
