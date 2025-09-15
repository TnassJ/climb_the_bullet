using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    List<int> score = new List<int>();//�X�R�A�����L���O�����X�g��
    List<string> scoreYMD = new List<string>();//�X�R�A�̓��t�����X�g��

    public TextMeshProUGUI scoreText0;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText1;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText2;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText3;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText4;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText5;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText6;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText7;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText8;// �X�R�A�̃e�L�X�g
    public TextMeshProUGUI scoreText9;// �X�R�A�̃e�L�X�g

    public TextMeshProUGUI DateText0;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText1;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText2;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText3;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText4;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText5;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText6;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText7;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText8;// ���t�̃e�L�X�g
    public TextMeshProUGUI DateText9;// ���t�̃e�L�X�g


    // Start is called before the first frame update
    void Start()
    {
        //scoreText[i]th�ɁA�Ή�����e�L�X�g�I�u�W�F�N�g�i�ρj���i�[
        //for (int i = 0; i < 10; i++) {

        //    scoreText{ i} = GameObject.Find($"Score{i+1}th");
        //}
        for (int i = 0; i < 10; i++)
        {

            //�������Ƀ��X�g�ɓ����X�R�A�A�������̓X�R�A���Ȃ��ꍇ�ɓ���鐔�l
            score.Add(PlayerPrefs.GetInt($"SCORE[{i}]", 0));
            scoreYMD.Add(PlayerPrefs.GetString($"SCOREYMD[{i}]", "YYYY/MM/DDFFF"));
        }
        //�����̃X�R�A�̒l�����ׂ�score���X�g�ɑ}���Ascore[0]���n�C�X�R�A
        //for (int i = 0; i < 10; i++)
        //{
        //�������Ƀ��X�g�ɓ����X�R�A�A�������̓X�R�A���Ȃ��ꍇ�ɓ���鐔�l
        //score.Add(PlayerPrefs.GetInt($"SCORE[{i}]", 0));

        scoreText0.text = score[0].ToString().PadLeft(8, '0');
        scoreText1.text = score[1].ToString().PadLeft(8, '0');
        scoreText2.text = score[2].ToString().PadLeft(8, '0');
        scoreText3.text = score[3].ToString().PadLeft(8, '0');
        scoreText4.text = score[4].ToString().PadLeft(8, '0');
        scoreText5.text = score[5].ToString().PadLeft(8, '0');
        scoreText6.text = score[6].ToString().PadLeft(8, '0');
        scoreText7.text = score[7].ToString().PadLeft(8, '0');
        scoreText8.text = score[8].ToString().PadLeft(8, '0');
        scoreText9.text = score[9].ToString().PadLeft(8, '0');

        DateText0.text = ResultDataTrim(scoreYMD[0].ToString());
        DateText1.text = ResultDataTrim(scoreYMD[1].ToString());
        DateText2.text = ResultDataTrim(scoreYMD[2].ToString());
        DateText3.text = ResultDataTrim(scoreYMD[3].ToString());
        DateText4.text = ResultDataTrim(scoreYMD[4].ToString());
        DateText5.text = ResultDataTrim(scoreYMD[5].ToString());
        DateText6.text = ResultDataTrim(scoreYMD[6].ToString());
        DateText7.text = ResultDataTrim(scoreYMD[7].ToString());
        DateText8.text = ResultDataTrim(scoreYMD[8].ToString());
        DateText9.text = ResultDataTrim(scoreYMD[9].ToString());
    }

    // スコアの日付の秒以降のデータを切り取る
    private string ResultDataTrim(string scoreText)
    {
        return scoreText.Substring(0, scoreText.Length - 3);
    }
}
