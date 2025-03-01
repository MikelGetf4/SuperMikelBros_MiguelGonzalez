using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public void CargarNivel1()
    {
        SceneManager.LoadScene("Level 1-1");
    }
}
