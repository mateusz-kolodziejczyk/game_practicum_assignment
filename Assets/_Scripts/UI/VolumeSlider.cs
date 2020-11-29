using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    GameManagement gameManagement;
    void Start()
    {
        gameManagement = GameObject.FindWithTag("GameManagement").GetComponent<GameManagement>();
        var volumeSlider = GetComponent<Slider>();
        volumeSlider.onValueChanged.AddListener(gameManagement.Changevolume);
    }
}
