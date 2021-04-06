using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NestedButton : MonoBehaviour
{
    Canvas canvas;
    Button selfButton;
    NestedButton[] nestedButtonsInChildren;
    bool open = false;

    [SerializeField]
    Vector2 offset;
    [SerializeField]
    List<Button> subButtons = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        selfButton = gameObject.GetComponent<Button>();
        selfButton.onClick.AddListener(ToggleSubMenu);
        canvas = transform.root.gameObject.GetComponent<Canvas>();
        offset *= canvas.transform.localScale;
        subButtons.Reverse();
        for (int i = 0; i < subButtons.Count; i++)
        {
            subButtons[i].transform.position = new Vector3(transform.position.x + offset.x, Mathf.Clamp(transform.position.y - (offset.y * (subButtons.Count - 1) / 2), offset.y / 2, Mathf.Infinity) + offset.y * i, 0);
        }
    }

    void ToggleSubMenu()
    {
        if (!open)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.parent.GetChild(i).TryGetComponent<NestedButton>(out NestedButton nB);
                if (nB != null)
                {
                    nB.CloseSubMenu();
                }
            }
            open = true;
            for (int i = 0; i < subButtons.Count; i++)
            {
                subButtons[i].gameObject.SetActive(true);
            }
        }
        else
        {
            nestedButtonsInChildren = GetComponentsInChildren<NestedButton>();
            foreach (NestedButton i in nestedButtonsInChildren)
            {
                i.CloseSubMenu();
            }
        }
    }

    void CloseSubMenu()
    {
        open = false;
        for (int i = 0; i < subButtons.Count; i++)
        {
            subButtons[i].gameObject.SetActive(false);
        }
    }
}
