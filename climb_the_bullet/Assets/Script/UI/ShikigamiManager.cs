using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShikigamiManager : MonoBehaviour
{
    public int SwitchCount = 0;
    int SikigamiMode;
    int CurrentSikigamiMode;
    public GameObject TODAIcon;
    public GameObject TENKOIcon;
    public GameObject TENKUIcon;
    public GameObject RIKUGOIcon;

    public AudioClip AudioClip; // SE
    Renderer TODARenderer;
    Renderer TENKORenderer;
    Renderer TENKURenderer;
    Renderer RIKUGORenderer;

    public bool tutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        GetSikigamiMode(); //式神モード取得
        CurrentSikigamiMode = SikigamiMode; // 現在の式神モードを控える→式神の変更を検出する
        // 4アイコンの色相を変えたいため、レンダー情報取得
        TODARenderer = TODAIcon.GetComponent<Renderer>();
        TENKORenderer = TENKOIcon.GetComponent<Renderer>();
        TENKURenderer = TENKUIcon.GetComponent<Renderer>();
        RIKUGORenderer = RIKUGOIcon.GetComponent<Renderer>();
        IconReset();
        IconChange();
    }

    void GetSikigamiMode()
    {
        // プレイヤーの情報から現在の式神のモード番号を取得
        //var PlayerObject = GameObject.Find("Player");
        //var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
        if (tutorial)
        {
            SikigamiMode = TutorialShootingPlayer.SikigamiMode;
        }
        else SikigamiMode = ShootingPlayer.SikigamiMode;
    }
    void IconReset()
    {
        // アイコンを全て初期状態に（グレー、大きさ0.9）
        TODARenderer.material.color = Color.gray;
        TENKORenderer.material.color = Color.gray;
        TENKURenderer.material.color = Color.gray;
        RIKUGORenderer.material.color = Color.gray;
        TODAIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        TENKOIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        TENKUIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        RIKUGOIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        GetSikigamiMode();
        //Debug.Log("CurrentSikigamiMode" + CurrentSikigamiMode);
        //Debug.Log("SikigamiMode" + SikigamiMode);
        // 式神変更を検出、変更なしなら以下の処理スキップ
        if (CurrentSikigamiMode == SikigamiMode) return;

        IconReset();
        IconChange();
        CurrentSikigamiMode = SikigamiMode;

    }

    void IconChange()
    {
        if (SikigamiMode == 0)
        {
            TODARenderer.material.color = Color.white;
            TODAIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        else if (SikigamiMode == 1)
        {
            TENKORenderer.material.color = Color.white;
            TENKOIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        else if (SikigamiMode == 2)
        {
            TENKURenderer.material.color = Color.white;
            TENKUIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        else if (SikigamiMode == 3)
        {
            RIKUGORenderer.material.color = Color.white;
            RIKUGOIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }


}
