using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHoverSound : MonoBehaviour {
    [SerializeField] private SoundMenu soundMenu;

    public void PlayHover() { if(soundMenu) soundMenu.PlayHoverButton(); }
}
