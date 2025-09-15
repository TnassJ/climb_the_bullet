using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class LoadConfig : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    //[SerializeField] AudioSource bgmAudioSource;
    //[SerializeField] AudioSource seAudioSource;

    float bgmValue = 0f;
    float seValue = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //bgmAudioSource = GameObject.Find("AudioSource(BGM)").GetComponent<AudioSource>(); // SE�ǂݍ���
        //seAudioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE�ǂݍ���


        bgmValue = PlayerPrefs.GetFloat("bgmValue", 0f); //�������ɃZ�b�g����ꏊ�A�������͂Ȃ��������̐��l
        //float decibel = 20f * Mathf.Log10(bgmValue);
        //decibel = Mathf.Clamp(decibel, -80f, 0f);
        audioMixer.SetFloat("BGM", bgmValue);

        seValue = PlayerPrefs.GetFloat("seValue", 0f); //�������ɃZ�b�g����ꏊ�A�������͂Ȃ��������̐��l
        //float decibel_se = 20f * Mathf.Log10(seValue);
        //decibel_se = Mathf.Clamp(decibel_se, -80f, 0f);
        audioMixer.SetFloat("SE", seValue);
        Debug.Log("SEValue = " + seValue);

        float seValue2 = PlayerPrefs.GetFloat("seSliderPosition", 0f); //�������ɃZ�b�g����ꏊ�A�������͂Ȃ��������̐��l
        Debug.Log("SEValue2 = " + seValue2);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
