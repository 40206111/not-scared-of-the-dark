using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInInstructions : MonoBehaviour
{
    [SerializeField]
    eTransitionEnums Transition;
    [SerializeField]
    float FadeTime = 1;


    private void OnEnable()
    {
        GameManager.Instance.QueueTransition(Transition, FadeTime);
    }
}
