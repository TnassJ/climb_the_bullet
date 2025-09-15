using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EaseList
{
    // 緩急 弱 ↑
    // 一次関数
    Linear,

    // Sine
    InSine,
    OutSine,
    InOutSine,

    // 二次関数
    InQuad,
    OutQuad,
    InOutQuad,

    // 三次関数
    InCubic,
    OutCubic,
    InOutCubic,

    // 四次関数
    InQuart,
    OutQuart,
    InOutQuart,

    // 五次関数
    InQuint,
    OutQuint,
    InOutQuint,

    // 指数関数
    InExpo,
    OutExpo,
    InOutExpo,

    // 円形関数
    InCirc,
    OutCirc,
    InOutCirc,

    // 1度のみ振動
    InBack,
    OutBack,
    InOutBack,

    // 弾性
    InElastic,
    OutElastic,
    InOutElastic,

    // バウンド
    InBounce,
    OutBounce,
    InOutBounce
    // 緩急 強 ↓
}