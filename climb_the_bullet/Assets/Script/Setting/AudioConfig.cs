using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioConfig : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;

    void Awake()
    {
        float bgmValue = PlayerPrefs.GetFloat("bgmSliderPosition", 1.0f); //�������ɃZ�b�g����ꏊ�A�������͂Ȃ��������̐��l
        float seValue = PlayerPrefs.GetFloat("seSliderPosition", 1.0f); //�������ɃZ�b�g����ꏊ�A�������͂Ȃ��������̐��l

        //�V�[���ǂݍ��ݎ��A���݂̉��ʐݒ���X���C�_�[�ɔ��f
        bgmSlider.value = bgmValue;
        seSlider.value = seValue;
    }

    // Start is called before the first frame update
    void Start()
    {


        //�X���C�_�[��G�����特�ʂ��ω�����
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);
            Debug.Log("Value = " + value);

            //�ω�����̂�-80�`0�܂ł̊�
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f, 0f);
            audioMixer.SetFloat("BGM", decibel);

            //���̃V�[���Œl���Ăяo�����߂ɕۑ����Ă���
            PlayerPrefs.SetFloat("bgmValue", decibel); //�������ɃZ�b�g����ꏊ�A�������ɃZ�b�g���鐔�l
            //�X���C�_�[�̈ʒu���ۑ�����
            PlayerPrefs.SetFloat("bgmSliderPosition", value); //�������ɃZ�b�g����ꏊ�A�������ɃZ�b�g���鐔�l
            PlayerPrefs.Save();//�f�[�^�ɃZ�b�g������ۑ�����
        });

        //�X���C�_�[��G�����特�ʂ��ω�����
        seSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);

            //�ω�����̂�-80�`0�܂ł̊�
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f, 0f);
            audioMixer.SetFloat("SE", decibel);

            //���̃V�[���Œl���Ăяo�����߂ɕۑ����Ă���
            PlayerPrefs.SetFloat("seValue", decibel); //�������ɃZ�b�g����ꏊ�A�������ɃZ�b�g���鐔�l
            //�X���C�_�[�̈ʒu���ۑ�����
            PlayerPrefs.SetFloat("seSliderPosition", value); //�������ɃZ�b�g����ꏊ�A�������ɃZ�b�g���鐔�l
            PlayerPrefs.Save();//�f�[�^�ɃZ�b�g������ۑ�����
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            seAudioSource.Play();
        }
    }
}
