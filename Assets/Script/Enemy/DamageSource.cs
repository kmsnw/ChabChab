using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데미지가 있는 오브젝트들의 기반 클래스
//개별 고유의 데미지 및 해당 데미지 기반 TakeDamage(characterhealth)호출

public class DamageSource : MonoBehaviour
{
    [Header("Damage")]
    public int damageValue = 10;

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            CharacterHealth healthComp = other.GetComponent<CharacterHealth>();

            if (healthComp != null)
            {
                healthComp.TakeDamage(damageValue);
            }
            
        }
        
        
    }
    
}
