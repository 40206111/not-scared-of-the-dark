using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTransition : MonoBehaviour
{
    Vector2[] ColourCoords = new Vector2[] { new Vector2(0.2f, 0.0f), new Vector2(0.4f, 0.0f), new Vector2(0.6f, 0.0f), new Vector2(0.8f, 0.0f) };
    string OutColourStringBase = "_OutCol";
    string TransProgString = "_TransProgress";

    [SerializeField]
    float FadeTime = 1.0f;
    [SerializeField]
    float ShaderFadeTime = 1.0f;

    [SerializeField]
    Material PaletteMat;

    [SerializeField]
    List<Texture> TransTextures;

    bool Running;

    public delegate void FinishedTransition();
    public static FinishedTransition FinishedTransitionDel;

    private void Awake()
    {
        PaletteMat.SetFloat(TransProgString, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            StartCoroutine(FadeTransition());
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            ResetColourCoords();
        }

    }

    IEnumerator RunTransition(Texture transTex, float fadeTime, bool fadeIn, bool asToggle)
    {
        while (Running)
        {
            yield return null;
        }

        Running = true;
        PaletteMat.SetTexture("_TransitionTexture", transTex);
        ShaderFadeTime = fadeTime;

        var progress = PaletteMat.GetFloat(TransProgString);
        float startTime = Time.time;

        if (asToggle ||
            (!(fadeIn && progress <= 0 ||
            !fadeIn && progress >= 1)))
        {
            int end = fadeIn ? 0 : 1;
            end = asToggle ? 1 - (int)progress : end;
            int start = 1 - end;
            while ((end == 1 && progress < 1) || (end == 0 && progress > 0))
            {
                progress = start + (((Time.time - startTime) / ShaderFadeTime) * (end - start)); //Mathf.Abs(((Time.time * (1.0f / ShaderFadeTime)) % 4.0f) - 2.0f) - 1.0f;
                progress = Mathf.Clamp(progress, 0.0f, 1.0f);
                PaletteMat.SetFloat(TransProgString, progress);
                yield return null;
            }
            PaletteMat.SetFloat(TransProgString, end);
        }

        Running = false;
        if (FinishedTransitionDel != null)
        {
            FinishedTransitionDel.Invoke();
        }
    }

    public void RunTransition(eTransitionEnums transition, float transTime, bool fadeIn, bool asToggle, FinishedTransition method = null)
    {
        FinishedTransitionDel += method;
        var transTex = TransTextures[(int)transition];
        StartCoroutine(RunTransition(transTex, transTime, fadeIn, asToggle));
    }

    public void ResetColourCoords()
    {
        for (int i = 0; i < ColourCoords.Length; ++i)
        {
            SetOutColours(i + 1, ColourCoords[i]);
        }
    }

    IEnumerator FadeTransition()
    {
        float elapsed = 0.0f;
        int colours = 4;
        float divTime = FadeTime / colours;
        int currentColour = 0;

        while (elapsed < FadeTime || currentColour < ColourCoords.Length)
        {

            int colCheck = (int)(elapsed / divTime);
            if (colCheck > currentColour)
            {
                currentColour = colCheck;
                for (int i = 0; /*i <= currentColour &&*/ i < ColourCoords.Length; ++i)
                {
                    int fColour = Mathf.Clamp(i + currentColour, 0, 3);
                    SetOutColours(i + 1, ColourCoords[fColour]);
                }
            }


            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SetOutColours(int colIndex, Vector2 to)
    {
        PaletteMat.SetVector(OutColourStringBase + colIndex, to);
    }
}
