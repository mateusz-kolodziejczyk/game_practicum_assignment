using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsReturnMenuButton : MonoBehaviour
{
    [SerializeField]
    AudioClip clip;
    public void ReturnToMenu()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void Start()
    {
        var gameManager = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var returnToMenuButton = GetComponent<Button>();

        returnToMenuButton.onClick.AddListener(delegate () 
        {
            gameManager.GetComponent<AudioSource>().Play(); 
        });

        returnToMenuButton.onClick.AddListener(ReturnToMenu);
    }
}
