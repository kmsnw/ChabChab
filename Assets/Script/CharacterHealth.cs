using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//체력 클래스
//최대 및 현재 체력, 무적시간(데미지 딜레이), 

public class CharacterHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    
    
    [SerializeField]
    private int _currentHealth;
    
    [Header("Invisibility")]
    public float invincibilityTime = 0.5f;
    private bool _isInvincible = false;

    private StageManager _stageManager;

    public void TakeDamage(int damage)
    {
        //무적일 경우 cancel
        if (_isInvincible) return;
        
        _currentHealth -= damage;
        
        //죽음 판정
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
        else
        {
            StartCoroutine(SetInvincible());
        }
    }
    
    private void Die()
    {
        if(_stageManager != null)
            _stageManager.objectDeath(this.gameObject);
        
        //gameObject.SetActive(false);
    }
    
    //코루틴 함수 -> 지정한 무적 시간만큼 TakeDamage() 무력화
    private IEnumerator SetInvincible()
    {
        _isInvincible = true;
  
        //무적시간(invincibilityTime) 만큼 대기 -> _isInvincible = true 유지
        yield return new WaitForSeconds(invincibilityTime);

        _isInvincible = false;
    }
    
    //체력 일정량 회복 -> 힐팩을 먹었을때.....
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
        
    }

    //체력 최대 회복 -> 리스폰 등..
    public void HealFull()
    {
        _currentHealth = maxHealth;
        _isInvincible = false;
    }
    
    
    void Start()
    {
        _stageManager = FindObjectOfType<StageManager>();
        if (_stageManager == null)
        {
            Debug.LogError("StageManager가 씬에 없음");
        }
        
        _currentHealth = maxHealth;
    }
}
