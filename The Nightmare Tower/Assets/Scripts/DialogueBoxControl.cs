using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxControl : MonoBehaviour
{
    private static DialogueBoxControl _Instance;
    public static DialogueBoxControl Instance { get { return _Instance; } }
    private void Awake()
    {
        if (_Instance != null && _Instance.gameObject != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;

        Filler = GetComponentInChildren<TextBoxFiller>();
        Filler.gameObject.SetActive(false);
        IsShowing = Filler.gameObject.activeInHierarchy;
    }


    public bool RequiresInput = true;
    TextBoxFiller Filler;
    bool IsShowing = false;

    Queue<DialogueDetails> TheQueue = new Queue<DialogueDetails>();

    public int QueueCount => TheQueue.Count;

    public void ResetSpeed()
    {
        Filler.ResetSpeed();
    }

    public void SetTimePerChar(float time)
    {
        Filler.SetTimePerChar(time);
    }

    public void Display(bool active)
    {
        Filler.ClearText();
        Filler.gameObject.SetActive(active);
        IsShowing = Filler.gameObject.activeInHierarchy;
        //if (Main.ActiveCamera != null)
        //{
        //    GetComponent<Canvas>().worldCamera = CameraFollow.ActiveCamera;
        //}
    }

    public void QueueDialogue(string text, float timePerChar = -1.0f, bool closeAfterText = false)
    {
        var dialogue = new DialogueDetails(text, timePerChar, closeAfterText);
        TheQueue.Enqueue(dialogue);

        if (!IsShowing)
        {
            var next = TheQueue.Dequeue();
            PrintTextWithCharacter(next.Text, next.TimePerChar, next.CloseAferText);
        }
    }


    public void PrintTextWithCharacter(string text, float timePerChar = -1.0f, bool closeAfterText = false)
    {
        StartCoroutine(PrintTextAndWait(text, timePerChar, closeAfterText));
    }

    public void PrintText(string text, float timePerChar = -1.0f, bool closeAfterText = false, bool waitInputStayOpen = false)
    {
        StartCoroutine(PrintTextAndWait(text, timePerChar, closeAfterText, waitInputStayOpen));
    }

    public IEnumerator PrintTextAndWait(string text, float timePerChar = -1.0f, bool closeAfterText = false, bool waitInputStayOpen = false)
    {
        if (!IsShowing)
        {
            Display(true);
        }
        if (timePerChar != -1.0f)
        {
            SetTimePerChar(timePerChar);
        }
        yield return StartCoroutine(Filler.PrintTextAndWait(text));
        if (closeAfterText)
        {
            yield return StartCoroutine(Filler.WaitForUser());
            Display(false);
        }
        else if (waitInputStayOpen)
        {
            yield return StartCoroutine(Filler.WaitForUser());
        }
        if (timePerChar != -1.0f)
        {
            ResetSpeed();
        }

        if (TheQueue.Count != 0)
        {
            var next = TheQueue.Dequeue();
            PrintTextWithCharacter(next.Text, next.TimePerChar, next.CloseAferText);
        }

    }
}
