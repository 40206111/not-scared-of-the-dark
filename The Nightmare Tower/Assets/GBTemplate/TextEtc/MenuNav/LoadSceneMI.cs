using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneMI : MenuItemBase
{
    [SerializeField]
    string SceneName;
    public override void PerformAction()
    {
        SceneManager.LoadScene(SceneName);
    }
}
