using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsButton : MonoBehaviour
{
    GameObject credits;
    void Start()
    {
        var gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        credits = GameObject.FindWithTag("Credits");
        credits.SetActive(false);
        var button = GetComponent<Button>();
        button.onClick.AddListener(LoadCredits);
        button.onClick.AddListener(delegate ()
        {
            gameManagement.GetComponent<AudioSource>().Play();
        });

    }

    private void LoadCredits()
    {
        credits.SetActive(true);
    }

}
