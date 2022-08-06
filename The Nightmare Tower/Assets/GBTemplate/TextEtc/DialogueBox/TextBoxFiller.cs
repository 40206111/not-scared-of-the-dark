using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxFiller : MonoBehaviour
{
    [SerializeField]
    private Text Text;
    [SerializeField]
    private Animator IconAnimator;

    [SerializeField]
    private float DefaultTimePerChar = 0.05f;
    [HideInInspector]
    public float TimePerChar = 0.0f;

    private bool Filling = false;

    private void Awake()
    {
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        TimePerChar = DefaultTimePerChar;
    }

    public void SetTimePerChar(float time)
    {
        TimePerChar = time;
    }

    public void PrintText(string text)
    {
        StartCoroutine(TextScroll(text));
    }

    public IEnumerator PrintTextAndWait(string text)
    {
        yield return StartCoroutine(TextScroll(text));
    }

    private IEnumerator TextScroll(string text)
    {
        // If already filling cancel and do this new text
        if (Filling)
        {
            Filling = false;
            yield return null;
        }

        // Filling commence
        Filling = true;

        CharacterInfo info;
        char[] chars = text.ToCharArray();
        int rowWidth = (int)Text.rectTransform.rect.width;
        List<int> rowStartIndexes = new List<int> { 0 };
        int lastWhitespace = -1;
        int currentWidth = 0;

        for (int i = 0; i < chars.Length; ++i)
        {
            char testChar = chars[i];
            Text.font.GetCharacterInfo(testChar, out info);

            /*
             * Get char width
             * if(' ')
             *      whitespace = i
             * if += char > text width 
             *      rowStartI = whitespace +1
             *      i = whitespace+1
             * else
             *      width += char
             */

            int charWidth = info.advance;
            if (testChar.Equals(' '))
            {
                lastWhitespace = i;
            }
            if (currentWidth + charWidth > rowWidth)
            {
                int newRowIndex = lastWhitespace + 1;
                if (newRowIndex == rowStartIndexes[rowStartIndexes.Count - 1])
                {
                    newRowIndex = i;
                    lastWhitespace = i-1; // Pretend current char is whitespace for the new line
                }
                if (newRowIndex < chars.Length)
                {
                    rowStartIndexes.Add(newRowIndex);
                    i = newRowIndex - 1; // minus one because for loop ++i
                }
                currentWidth = 0;
            }
            else
            {
                currentWidth += charWidth;
            }
        }

        // Create line list
        int lineCount = (int)(Text.rectTransform.rect.height / (((float)Text.fontSize) * Text.lineSpacing));
        List<string> lines = new List<string>();
        for (int i = 0; i < lineCount; ++i)
        {
            lines.Add("");
        }
        // Tracking variables
        int textProgress = 0;
        int currentFillLine = 0;
        int nextRowIndex = 1;
        // Timing variables
        float elapsedTime = 0.0f;
        int elapsedProgress = -1;
        int lastElapsedProgress = -1;
        // Print whole dialogue
        while (textProgress < chars.Length)
        {
            // Fill lines, wait for input to replace bottom line
            while (currentFillLine < lines.Count)
            {
                lastElapsedProgress = elapsedProgress;
                while (elapsedProgress == lastElapsedProgress)
                {
                    yield return null;
                    elapsedTime += Time.deltaTime;
                    elapsedProgress = Mathf.Clamp((int)(elapsedTime / TimePerChar), 0, chars.Length - 1);
                    if (ContinueInputPressed())
                    {
                        int current = elapsedProgress;
                        if (nextRowIndex < rowStartIndexes.Count)
                        {
                            elapsedProgress = rowStartIndexes[nextRowIndex] - 1;
                        }
                        else
                        {
                            elapsedProgress = chars.Length - 1;
                        }
                        elapsedTime += (elapsedProgress - current) * TimePerChar;
                        yield return null;
                    }
                }

                while (textProgress <= elapsedProgress && currentFillLine < lines.Count)
                {
                    // Add character to line
                    lines[currentFillLine] += chars[textProgress];

                    // Advance viewed character
                    textProgress++;
                    // If within character limits, there are more rows to print, and current progress is the start of the next line:
                    //      do next line stuff
                    if (textProgress < chars.Length && nextRowIndex < rowStartIndexes.Count && textProgress == rowStartIndexes[nextRowIndex])
                    {
                        lines[currentFillLine] += '\n';
                        lines[currentFillLine].TrimEnd(' ');
                        currentFillLine++;
                        nextRowIndex++;
                    }
                }
                elapsedProgress = textProgress - 1;

                // Put the lines into the text box
                string outString = "";
                for (int i = 0; i < lines.Count; ++i)
                {
                    outString += lines[i];
                }
                Text.text = outString;

                // Break out of writing lines to wait for input to continue
                if (textProgress >= chars.Length)
                {
                    break;
                }
            }

            yield return StartCoroutine(WaitForUser());

            // Move all lines up 1, make the bottom one blank
            if (textProgress < chars.Length)
            {
                for (int i = 0; i < lines.Count - 1; ++i)
                {
                    lines[i] = lines[i + 1];
                }
                lines[lines.Count - 1] = "";
                currentFillLine--;
            }
            else
            {
                for (int i = 0; i < lines.Count; ++i)
                {
                    lines[i] = "";
                }
            }
        }

        // Clean up
        Filling = false;
        yield return null;
    }

    public void ClearText()
    {
        Text.text = "";
    }

    public IEnumerator WaitForUser()
    {
        ActivateButtonPromptIcon();
        while (!ContinueInputPressed())
        {
            yield return null;
        }
        HidePromptIcon();
    }

    public void ActivateButtonPromptIcon()
    {
        IconAnimator.SetBool("Button", true);
    }
    public void HidePromptIcon()
    {
        IconAnimator.SetBool("Button", false);
    }

    private bool ContinueInputPressed()
    {
        return Input.GetButtonDown("AButton");
    }
}
