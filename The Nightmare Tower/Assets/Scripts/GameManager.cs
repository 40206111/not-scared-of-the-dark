using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TransitionController TransController;

    [SerializeField]
    TextBoxFiller TextFiller;

    public void QueueTransition(eTransitionEnums transitionEnum, float fadeTime = 1)
    {
        if (TransController != null)
        {
            TransController.AddTransition(transitionEnum, fadeTime);
        }
        else
        {
            Debug.LogWarning("Transition attempted to be added, but no transition controller to add it to");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            throw new System.Exception("GameManager Created when GameManager already exists");
        }
        DontDestroyOnLoad(gameObject);

        var renderTrans = GetComponent<RenderTransition>();
        if (renderTrans != null)
        {
            TransController = new TransitionController(renderTrans);
        }

    }
    private void Start()
    {
        if (DebugMenu.Instance != null)
        {
            DebugMenu.Instance.ConsoleCommand += GetDebugCommand;
        }
    }

    private void OnDestroy()
    {
        if (DebugMenu.Instance != null)
        {
            DebugMenu.Instance.ConsoleCommand -= GetDebugCommand;
        }
    }

    public void GetDebugCommand(string[] commands)
    {
        string keyword = commands[0].ToLower();

        if (keyword == "text")
        {
            if (commands.Length == 1)
            {
                TextFiller.PrintText("This is a test");
                return;
            }
            string print = "";
            for (int i = 1; i < commands.Length; i++)
            {
                if (i != 1)
                {
                    print += " ";
                }

                print += commands[i];
            }
            TextFiller.PrintText(print);
            return;
        }

        if (keyword == "trans")
        {
            if (commands.Length == 1)
            {
                var ran = UnityEngine.Random.Range(0, (int)eTransitionEnums.COUNT);
                QueueTransition((eTransitionEnums)ran);
                return;
            }

            if (Enum.TryParse("Active", out eTransitionEnums trans))
            {
                QueueTransition(trans);
            }
            return;
        }
    }
}
