using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void Scene()
    {
        SceneManager.LoadScene("The_Viking_Village");
        //·½Ê½¶þ SceneManager.LoadScene(0);
    }
    public void QuitAll()
    {
        Application.Quit();
    }
}
