#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public static DebugMenu Instance;

    [SerializeField]
    InputField InputBox;
    EventSystem myEventSystem;
    bool Open;

    public Action<string[]> ConsoleCommand;

    float LastGameSpeed;
    float GameSpeed = 1;

    //History
    List<string> LastText = new List<string>();
    int LastTextIndex = 0;
    bool History;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            throw new System.Exception("DebugMenu Created when DebugMenu already exists");
        }
        DontDestroyOnLoad(gameObject);
        myEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    void Start()
    {
        InputBox.gameObject.SetActive(false);
        ConsoleCommand += SetGameSpeed;
        ConsoleCommand += RunOrStop;
    }

    private void OnEnable()
    {
        StartCoroutine(SelectConsole());
    }

    private void OnDestroy()
    {
        ConsoleCommand -= SetGameSpeed;
    }

    void ToggleConsole()
    {
        Open = !Open;
        InputBox.gameObject.SetActive(Open);
        float tempSpeed = GameSpeed;
        GameSpeed = Open ? LastGameSpeed : 0;
        LastGameSpeed = tempSpeed;
    }

    void AddLastText(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            LastText.Add(text);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }

        if (Time.timeScale != GameSpeed)
        {
            Time.timeScale = GameSpeed;
        }

        if (!Open)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log($"reading {InputBox.text} from console");
            AddLastText(InputBox.text);
            if (History)
            {
                LastText.RemoveAt(LastTextIndex);
            }
            var parts = InputBox.text.Split(' ');
            if (parts.Length > 0)
            {
                ConsoleCommand.Invoke(parts);
            }
            InputBox.text = "";
            StartCoroutine(SelectConsole(toggle: false));
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(SelectConsole(toggle: true));
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!History)
            {
                LastTextIndex = LastText.Count;
                if (LastText.Count > 0)
                {
                    AddLastText(InputBox.text);
                }
                History = true;
            }

            LastTextIndex--;
            if (LastTextIndex < 0 && LastText.Count > 0)
            {
                LastTextIndex = LastText.Count - 1;
            }

            if (LastTextIndex >= 0)
            {
                InputBox.text = LastText[LastTextIndex];
            }

        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!History)
            {
                LastTextIndex = 0;
                if (LastText.Count > 0)
                {
                    AddLastText(InputBox.text);
                }
                History = true;
            }

            LastTextIndex++;
            if (LastTextIndex >= LastText.Count && LastText.Count > 0)
            {
                LastTextIndex = 0;
            }

            if (LastTextIndex < LastText.Count)
            {
                InputBox.text = LastText[LastTextIndex];
            }

        }

        if (Input.anyKeyDown && (!Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow)))
        {
            History = false;
        }
    }

    IEnumerator SelectConsole(bool toggle = false)
    {
        bool select = !(toggle && myEventSystem.currentSelectedGameObject == InputBox.gameObject);
        myEventSystem.SetSelectedGameObject(null);
        while (!InputBox.gameObject.activeSelf)
        {
            yield return null;
        }
        if (select)
        {
            InputBox.Select();
        }
    }

    void SetGameSpeed(string[] input)
    {
        string keyword = input[0].ToLower();

        if (keyword != "gamespeed")
        {
            return;
        }

        if (input.Length != 2)
        {
            Debug.Log("Please just enter gamespeed then value, nothing else");
            return;
        }

        LastGameSpeed = float.Parse(input[1]);
        if (GameSpeed != 0)
        {
            GameSpeed = LastGameSpeed;
            Debug.Log($"Setting Game Speed to {GameSpeed}");
        }
        else
        {
            Debug.Log($"Setting Game Speed to {GameSpeed}, unpause to see");
        }
    }

    void RunOrStop(string[] input)
    {
        string keyword = input[0].ToLower();
        
        if (keyword == "toggle")
        {
            if (input.Length == 2)
            {
                if (input[1].ToLower() == "pause")
                {
                    GameSpeed = GameSpeed == 0 ? LastGameSpeed : 0;
                    Debug.Log("Toggling pause");
                }
            }
        }
        else if (keyword == "pause")
        {
            GameSpeed = 0;
            Debug.Log("paused");
        }
        else if (keyword == "run" || keyword == "play" || keyword == "unpause")
        {
            GameSpeed = LastGameSpeed;
            Debug.Log($"running at speed {GameSpeed}");
        }
    }
}
#endif