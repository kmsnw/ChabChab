using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{

    public IEnumerator RespawnRoutine(GameObject item, float delay)
    {
        Debug.Log("false active");
        item.SetActive(false);
        
        yield return new WaitForSeconds(delay);
        
        Debug.Log("true active");
        item.SetActive(true);
    }
    
}
