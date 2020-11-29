using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    GameObject optionsMenu;
    void Start()
    {
        optionsMenu = GameObject.FindWithTag("OptionsMenu");
        optionsMenu.SetActive(false);
        var button = GetComponent<Button>();
        button.onClick.AddListener(LoadOptionsMenu);
    }

    private void LoadOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

}
