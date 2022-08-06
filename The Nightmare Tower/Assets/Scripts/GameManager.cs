using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var ran = Random.Range(0, (int)eTransitionEnums.COUNT);

            QueueTransition((eTransitionEnums)ran);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            TextFiller.PrintText("Hello Friends");
        }
    }
}
