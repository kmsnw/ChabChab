using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFaller : MonoBehaviour, IInteractable
{

    private Animator animator;
    
    public Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb.isKinematic = true;   
    }


    
    public void Interact(bool isInteracting, PlayerController player)
    {
        if (isInteracting)
        {
            animator.SetBool("Interacted", true);
            rb.gameObject.GetComponent<FallingPlatforms>().interactCount++;
        }
        else
        {
            animator.SetBool("Interacted", false);
            rb.gameObject.GetComponent<FallingPlatforms>().interactCount--;
        }
  
    }
    
    public string InteractionName => "Falling Platformer"; //본인 이름
}
