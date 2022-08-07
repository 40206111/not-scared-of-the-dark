using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemConfig
{
    public float NumberOfItems;
    public Image[] Images = new Image[4];
}

public class IconController : MonoBehaviour
{
    [SerializeField] List<Image> Icons;

    public void SetConfiguration(ItemConfig config)
    {
        for (int i = 0; i < Icons.Count; i++)
        {
            if (config.NumberOfItems - 1 <= i)
            {
                Icons[i].gameObject.SetActive(false);
            }
            else
            {
                Icons[i].gameObject.SetActive(true);
                if (config.Images[i] == null)
                {
                    Icons[i].enabled = false;
                }
                else
                {
                    Icons[i].enabled = true;
                    Icons[i].sprite = config.Images[i].sprite;
                }
            }
        }
    }

    public void SetIconLocation(int old, int number)
    {
        Icons[old].enabled = false;
        Icons[number].enabled = true;
    }

}
