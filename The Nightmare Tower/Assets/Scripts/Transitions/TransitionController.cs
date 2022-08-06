using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController
{
    Queue<eTransitionEnums> Transistions = new Queue<eTransitionEnums>();
    Queue<float> TransTimes = new Queue<float>();
    RenderTransition RenderTrans;


    public TransitionController(RenderTransition renderTransition)
    {
        RenderTrans = renderTransition;
    }


    public void AddTransition(eTransitionEnums transition, float time = 1)
    {
        Transistions.Enqueue(transition);
        TransTimes.Enqueue(time);
    }

    public void RunTransition(bool fadeIn, RenderTransition.FinishedTransition method = null, bool asToggle = false)
    {
        eTransitionEnums trans = eTransitionEnums.None;
        float transTime = 1;

        if (Transistions.Count != 0)
        {
            trans = Transistions.Dequeue();
            transTime = TransTimes.Dequeue();
        }

        if (trans == eTransitionEnums.None)
        {
            transTime = 0.01f;
        }
        RenderTrans.RunTransition(trans, transTime, fadeIn, asToggle, method);

    }

}
