using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by name.
    /// </summary>
    /// <param name="sceneName">Scene name to load.</param>
    public void LoadSceneByName(string sceneName)
    {
        Sceneloader.s_Instance.LoadScene(sceneName);
    }
}
