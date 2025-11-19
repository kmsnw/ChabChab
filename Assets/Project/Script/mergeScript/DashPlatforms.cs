using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPlatforms : MonoBehaviour
{
    public float dashSpeed = 20f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            transform.position += Vector3.up * dashSpeed * Time.deltaTime;
            
        }
    }
}
