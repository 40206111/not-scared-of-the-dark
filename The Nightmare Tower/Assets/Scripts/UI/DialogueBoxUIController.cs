using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxUIController : MonoBehaviour
{
    int Current = 0;
    int Max = 4;

    [SerializeField]
    IconController Icons;
    [SerializeField]
    IconController Highlight;
    [SerializeField]
    float SecsBetweenMoves;

    float WaitTime;

    bool Active = true;

    void Update()
    {
        if (!Active)
        {
            return;
        }
        var axis = Input.GetAxisRaw("Horizontal");

        if (axis == 0)
        {
            WaitTime = 0;
        }

        if (WaitTime > 0)
        {
            WaitTime -= Time.deltaTime;
            return;
        }

        int next = Current;
        if (axis > 0)
        {
            next++;
            next = next >= Max ? 0 : next;
        }
        if (axis < 0)
        {
            next--;
            next = next < 0 ? Max - 1 : next;
        }

        if (next != Current)
        {
            Highlight.SetIconLocation(Current, next);
            Current = next;
            WaitTime = SecsBetweenMoves;
        }
    }
}
