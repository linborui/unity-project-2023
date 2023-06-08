using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    public string sceneName;
    public SceneFade sceneFade;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            sceneFade.PanelFadeIn();
            Invoke("LoadScene", sceneFade.fadeTime);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}