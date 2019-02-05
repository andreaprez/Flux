using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenu : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressedButton;
    [SerializeField] private AudioClip hoverButton;

    public void PlayPressedButton() { audioSource.PlayOneShot(pressedButton); }
    public void PlayHoverButton() { audioSource.PlayOneShot(hoverButton); }
}
