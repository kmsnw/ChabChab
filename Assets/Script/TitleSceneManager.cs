using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//타이틀 씬 관리
public class TitleSceneManager : MonoBehaviour
{

    void GameSceneLoad()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    void Update()
    {   
        //press any key.. -> 씬 전환(title -> tutorial)
        if (Input.anyKeyDown)
        {
            GameSceneLoad();
        }
    }
}
