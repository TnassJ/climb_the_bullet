using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererBlinker : MonoBehaviour
{
    // 点滅させる対象
    //[SerializeField] private SpriteRenderer _target;
    SpriteRenderer _target; // プレイヤーの色相等を変えるのに必要

    // 点滅周期[s]
    [SerializeField] private float _cycle = 0.1f;

    [SerializeField] private float MaxDensity = 1.0f;
    [SerializeField] private float MinDensity = 0.7f;


    public bool _isBlinking = false;
    private float _defaultAlpha;
    private double _time;

    /// <summary>
    /// 点滅を開始する
    /// </summary>
    public void BeginBlink()
    {
        
        // 点滅中は何もしない
        if (_isBlinking) return;

        _isBlinking = true;

        // 時間を開始時点に戻す
        _time = 0;
    }

    /// <summary>
    /// 点滅を終了する
    /// </summary>
    public void EndBlink()
    {
        _isBlinking = false;

        // 初期状態のアルファ値に戻す
        SetAlpha(_defaultAlpha);
    }

    // Start is called before the first frame update
    void Start()
    {
        _target = GetComponent<SpriteRenderer>();
        _defaultAlpha = _target.color.a;
        Debug.Log("_defaultAlpha"+_defaultAlpha);

    }

    // Update is called once per frame
    void Update()
    {
        // 点滅していないときは何もしない
        if (!_isBlinking) return;

        // 内部時刻を経過させる
        _time += Time.deltaTime;

        // 周期cycleで繰り返す値の取得
        // 0～cycleの範囲の値が得られる
        var repeatValue = Mathf.Repeat((float)_time, _cycle);

        // 内部時刻timeにおける明滅状態を反映
        // Imageのアルファ値を変更している
        SetAlpha(repeatValue >= _cycle * 0.5f ? MaxDensity : MinDensity);
    }

    // Imageのアルファ値を変更するメソッド
    private void SetAlpha(float alpha)
    {
        var color = _target.color;
        color.a = alpha;
        _target.color = color;
    }
}
