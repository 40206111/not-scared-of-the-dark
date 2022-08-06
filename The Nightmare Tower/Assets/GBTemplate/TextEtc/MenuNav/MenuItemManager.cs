using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItemManager : MonoBehaviour
{
    int _currentIndex = 0;
    [SerializeField]
    bool MenuLoops = true;
    [SerializeField]
    List<MenuItemBase> MenuItems = new List<MenuItemBase>();
    [SerializeField]
    RectTransform Arrow;
    Vector2 ArrowValues;

    // Start is called before the first frame update
    void Start()
    {
        ArrowValues = Arrow.anchoredPosition;
        if(MenuItems.Count > 0)
        {
            MenuItems[0].IsHighlighted = true;
            Arrow.SetParent(MenuItems[0].transform);
            Arrow.localPosition = ArrowValues;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxisRaw("Vertical") * (Input.GetButtonDown("Vertical") ? 1.0f : 0.0f);
        int change = 0;
        if(vert > 0)
        {
            change = -1;
        }
        else if(vert < 0)
        {
            change = 1;
        }

        if(change != 0)
        {
            MenuItems[CurrentIndex].IsHighlighted = false;
            CurrentIndex += change;
            MenuItems[CurrentIndex].IsHighlighted = true;
            Arrow.SetParent(MenuItems[CurrentIndex].transform);
            Arrow.localPosition = ArrowValues;
        }

        if (Input.GetButtonDown("AButton"))
        {
            MenuItems[CurrentIndex].PerformAction();
        }
    }

    private int CurrentIndex
    {
        get
        {
            return _currentIndex;
        }
        set
        {
            if (MenuLoops)
            {
                if (value < 0)
                {
                    value = MenuItems.Count - 1;
                }
                _currentIndex = value % MenuItems.Count;
            }
            else
            {
                _currentIndex = Mathf.Clamp(value, 0, MenuItems.Count - 1);
            }
        }
    }
}
