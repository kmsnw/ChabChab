using UnityEngine;

//클리어 버튼 컨트롤러
//상호작용 인터페이스 구현 -> on off 태그 변경

public class ClearButtonController : MonoBehaviour, IInteractable
{
    
    private Animator _animator;
    //상호작용 인터페이스 구현
    public string InteractionName => "Clear Button"; //본인 이름
   
    //상호작용 동작 구현
    public void Interact(bool isInteracting, PlayerController player)
    {
        //상호작용 상태에 따라 태그 변경
        if (isInteracting)
        {
            Debug.Log("Clear Button: On");
            gameObject.tag = "On";
            _animator.SetBool("isIntract", true);
            

         
        }
        else 
        {
            gameObject.tag = "Off";
    
            _animator.SetBool("isIntract", false);
            

        }
    }
    
    
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        //기본 태그값 설정(버튼 비활성화)
        gameObject.tag = "Off";
    }
    
}
