using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void goToScene(string scenename)
    {
        scenename = _sceneName;
        SceneManager.LoadScene(scenename);
        
    }
}
