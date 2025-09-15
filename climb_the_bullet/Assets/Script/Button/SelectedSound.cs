using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.

public class SelectedSound : MonoBehaviour, ISelectHandler
{
    public AudioClip ButtonSelected; // SE
    [SerializeField] AudioSource seAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        seAudioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SEì«Ç›çûÇ›

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        //var audioSource = FindObjectOfType<AudioSource>();
        seAudioSource.PlayOneShot(ButtonSelected);
        //Debug.Log(this.gameObject.name + " was selected");
    }
}
