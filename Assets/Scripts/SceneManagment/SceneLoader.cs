using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string m_SceneName;
    public void Execute()
    {
        SceneManager.LoadSceneAsync(m_SceneName);
        //Pantalla de Carga un don't destroy onLoad  en un canvas y activarlo al cargar
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
            Execute();
    }
}
