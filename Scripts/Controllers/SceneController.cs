using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void Scene()
    {
        SceneManager.LoadScene("The_Viking_Village");
        //��ʽ�� SceneManager.LoadScene(0);
    }
    public void QuitAll()
    {
        Application.Quit();
    }
}
