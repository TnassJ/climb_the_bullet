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
        float bgmValue = PlayerPrefs.GetFloat("bgmSliderPosition", 1.0f); //第一引数にセットする場所、第二引数はなかった時の数値
        float seValue = PlayerPrefs.GetFloat("seSliderPosition", 1.0f); //第一引数にセットする場所、第二引数はなかった時の数値

        //シーン読み込み時、現在の音量設定をスライダーに反映
        bgmSlider.value = bgmValue;
        seSlider.value = seValue;
    }

    // Start is called before the first frame update
    void Start()
    {


        //スライダーを触ったら音量が変化する
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);
            Debug.Log("Value = " + value);

            //変化するのは-80～0までの間
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f, 0f);
            audioMixer.SetFloat("BGM", decibel);

            //他のシーンで値を呼び出すために保存しておく
            PlayerPrefs.SetFloat("bgmValue", decibel); //第一引数にセットする場所、第二引数にセットする数値
            //スライダーの位置も保存する
            PlayerPrefs.SetFloat("bgmSliderPosition", value); //第一引数にセットする場所、第二引数にセットする数値
            PlayerPrefs.Save();//データにセットしたら保存する
        });

        //スライダーを触ったら音量が変化する
        seSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);

            //変化するのは-80～0までの間
            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f, 0f);
            audioMixer.SetFloat("SE", decibel);

            //他のシーンで値を呼び出すために保存しておく
            PlayerPrefs.SetFloat("seValue", decibel); //第一引数にセットする場所、第二引数にセットする数値
            //スライダーの位置も保存する
            PlayerPrefs.SetFloat("seSliderPosition", value); //第一引数にセットする場所、第二引数にセットする数値
            PlayerPrefs.Save();//データにセットしたら保存する
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
