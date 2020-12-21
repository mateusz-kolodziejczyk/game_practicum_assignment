using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClickNoise : MonoBehaviour
{
    [SerializeField]
    AudioClip clickNoise;

    public void PlayNoise()
    {
        AudioSource.PlayClipAtPoint(clickNoise, gameObject.transform.position);
    }
}
