using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDetails
{
    public string Text;
    public float TimePerChar;
    public bool CloseAferText;

    public DialogueDetails(string text, float timePerChar = -1.0f, bool closeAfterText = false)
    {
        Text = text;
        TimePerChar = timePerChar;
        CloseAferText = closeAfterText;
    }

}
