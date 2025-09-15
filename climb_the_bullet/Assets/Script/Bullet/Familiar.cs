using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Familiar : MonoBehaviour
{
    float angle = 0; // 角度
    public float speed = 3.0f; // 速度
    Vector3 velocity; // 移動量
    //public AudioClip BulletClip; // 敵を倒した時に再生する SE
    float timeCount = 0; // 経過時間
    public EaseList easing;

    //public BossMove BossMove;//☆追加 BossMove
    public AudioClip bulletClip; // 弾のSE
    AudioSource audioSource;
    float shotAngle = 0; // 発射角度
    [SerializeField] Bullet FamiliarBullet; // 弾のプレハブ
    public float shotInterval = 0.3f; // 発射間隔

    //☆追加20241104
    public bool VortexBullet_familiar_On = false;
    public bool ToMeBullet_familiar_On = false;
    float timeCountB = 0; // 経過時間


    void Start()
    {

        // X方向の移動量を設定する
        velocity.x = speed * Mathf.Cos(angle * Mathf.Deg2Rad);

        // Y方向の移動量を設定する
        velocity.y = speed * Mathf.Sin(angle * Mathf.Deg2Rad);

        // 弾の向きを設定する
        float zAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.Euler(0, 0, zAngle);
        // SE を再生する
        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>();
        //audioSource.volume = 0.1f;
        // 5秒後に削除
        //Destroy(gameObject, 5.0f);
    }
    void Update()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        //var easeMethod = GetEasingMethod(easing);
        var shotSpeed =speed;

        velocity.Normalize(); //正則化
        velocity *= shotSpeed; // ←のためにshotSpeedは初期値1.0とする
        // 毎フレーム、弾を移動させる
        transform.position += velocity * Time.deltaTime;
        DeleteBullet();

        transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 向きを固定

        //以下、召喚に応じた弾幕を展開
        if (VortexBullet_familiar_On == true)
        {
            VortexBullet_familiar();
        }
        if (ToMeBullet_familiar_On == true)
        {
            //1.2秒間連射
            timeCountB += Time.deltaTime;
            if (timeCountB > 1.2f) return;
            ToMeBullet_familiar();
        }
    }

    // !!追加!!
    // 角度と速度を設定する関数
    public void Init(float input_angle, float input_speed)
    {
        angle = input_angle;
        speed = input_speed;
    }

    public void DeleteBullet()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -6.0f | xp > 6.0f | yp < -6.0f | yp > 6.0f)
        {
            Destroy(gameObject);
        }

    }

    void VortexBullet_familiar()
    {

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle -= 43; // 発射角度を10度ずらす
        // インスタンス生成
        // 発射する弾を生成する
        var shot = Instantiate(FamiliarBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, 3);
        shot.LotationSwitch = false; //弾の向きを固定
        audioSource.PlayOneShot(bulletClip, 0.05f); // SE再生
    }

    void ToMeBullet_familiar()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか 数字が小さいほど連射
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        // インスタンス生成
        // 発射する弾を生成する Instantiate( 生成するオブジェクト,  場所, 回転 );
        var shot = Instantiate(FamiliarBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, 5);
        audioSource.PlayOneShot(bulletClip, 0.05f); // SE再生

    }

    // 2点間の角度を取得
    float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    // Func<in T, out TResult>(T arg);
    // イージング関数を返す関数
    public static Func<float, float> GetEasingMethod(EaseList ease)
    {
        switch (ease)
        {
            case EaseList.Linear:
                return Linear;

            case EaseList.InSine:
                return EaseInSine;
            case EaseList.OutSine:
                return EaseOutSine;
            case EaseList.InOutSine:
                return EaseInOutSine;

            case EaseList.InQuad:
                return EaseInQuad;
            case EaseList.OutQuad:
                return EaseOutQuad;
            case EaseList.InOutQuad:
                return EaseInOutQuad;

            case EaseList.InCubic:
                return EaseInCubic;
            case EaseList.OutCubic:
                return EasesOutCubic;
            case EaseList.InOutCubic:
                return EasesInOutCubic;

            case EaseList.InQuart:
                return EaseInQuart;
            case EaseList.OutQuart:
                return EaseOutQuart;
            case EaseList.InOutQuart:
                return EaseInOutQuart;

            case EaseList.InQuint:
                return EaseInQuint;
            case EaseList.OutQuint:
                return EaseOutQuint;
            case EaseList.InOutQuint:
                return EaseInOutQuint;

            case EaseList.InExpo:
                return EaseInExpo;
            case EaseList.OutExpo:
                return EaseOutExpo;
            case EaseList.InOutExpo:
                return EaseInOutExpo;

            case EaseList.InCirc:
                return EaseInCirc;
            case EaseList.OutCirc:
                return EaseOutCirc;
            case EaseList.InOutCirc:
                return EaseInOutCirc;

            case EaseList.InBack:
                return EaseInBack;
            case EaseList.OutBack:
                return EaseOutBack;
            case EaseList.InOutBack:
                return EaseInOutBack;

            case EaseList.InElastic:
                return EaseInElastic;
            case EaseList.OutElastic:
                return EaseOutElastic;
            case EaseList.InOutElastic:
                return EaseInOutElastic;

            case EaseList.InBounce:
                return EaseInBounce;
            case EaseList.OutBounce:
                return EaseOutBounce;
            case EaseList.InOutBounce:
                return EaseInOutBounce;

            default:
                return Linear;
        }
    }

    public static float Linear(float x)
    {
        return x;
    }

    public static float EaseInSine(float x)
    {
        return 1.0f - Mathf.Cos((x * Mathf.PI) / 2.0f);
    }

    public static float EaseOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2.0f);
    }

    public static float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
    }

    public static float EaseInQuad(float x)
    {
        return x * x;
    }

    public static float EaseOutQuad(float x)
    {
        return 1.0f - (1.0f - x) * (1.0f - x);
    }

    public static float EaseInOutQuad(float x)
    {
        return x < 0.5f ? (2.0f * x * x) : (1.0f - Mathf.Pow(-2.0f * x + 2.0f, 2.0f) / 2.0f);
    }

    public static float EaseInCubic(float x)
    {
        return x * x * x;
    }

    public static float EasesOutCubic(float x)
    {
        return 1.0f - Mathf.Pow(1.0f - x, 3.0f);
    }

    public static float EasesInOutCubic(float x)
    {
        return x < 0.5f ? (4.0f * x * x * x) : (1.0f - Mathf.Pow(-2.0f * x + 2.0f, 3.0f) / 2.0f);
    }

    public static float EaseInQuart(float x)
    {
        return x * x * x * x;
    }

    public static float EaseOutQuart(float x)
    {
        return 1.0f - Mathf.Pow(1.0f - x, 4.0f);
    }

    public static float EaseInOutQuart(float x)
    {
        return x < 0.5f ? (8.0f * x * x * x * x) : (1.0f - Mathf.Pow(-2.0f * x + 2.0f, 4.0f) / 2.0f);
    }

    public static float EaseInQuint(float x)
    {
        return x * x * x * x * x;
    }

    public static float EaseOutQuint(float x)
    {
        return 1.0f - Mathf.Pow(1.0f - x, 5.0f);
    }

    public static float EaseInOutQuint(float x)
    {
        return x < 0.5f ? (16.0f * x * x * x * x * x) : (1.0f - Mathf.Pow(-2.0f * x + 2.0f, 5.0f) / 2.0f);
    }

    public static float EaseInExpo(float x)
    {
        return x == 0.0f ? (0.0f) : Mathf.Pow(2.0f, 10.0f * x - 10.0f);
    }

    public static float EaseOutExpo(float x)
    {
        return x == 1.0f ? 1.0f : 1.0f - Mathf.Pow(2.0f, -10.0f * x);
    }

    public static float EaseInOutExpo(float x)
    {
        return x == 0.0f ? 0.0f
        : x == 1.0f ? 1.0f
        : x < 0.5f ? Mathf.Pow(2.0f, 20.0f * x - 10.0f) / 2.0f
        : (2.0f - Mathf.Pow(2.0f, -20.0f * x + 10.0f)) / 2.0f;
    }

    public static float EaseInCirc(float x)
    {
        return 1.0f - Mathf.Sqrt(1.0f - Mathf.Pow(x, 2.0f));
    }

    public static float EaseOutCirc(float x)
    {
        return Mathf.Sqrt(1.0f - Mathf.Pow(x - 1.0f, 2.0f));
    }

    public static float EaseInOutCirc(float x)
    {
        return x < 0.5f
        ? (1 - Mathf.Sqrt(1.0f - Mathf.Pow(2.0f * x, 2.0f))) / 2.0f
        : (Mathf.Sqrt(1.0f - Mathf.Pow(-2.0f * x + 2.0f, 2.0f)) + 1.0f) / 2.0f;
    }

    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1.0f;
        return c3 * x * x * x - c1 * x * x;
    }

    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1.0f;
        return 1.0f + c3 * Mathf.Pow(x - 1.0f, 3.0f) + c1 * Mathf.Pow(x - 1.0f, 2.0f);
    }

    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return x < 0.5f
        ? (Mathf.Pow(2.0f * x, 2.0f) * ((c2 + 1.0f) * 2.0f * x - c2)) / 2.0f
        : (Mathf.Pow(2.0f * x - 2.0f, 2.0f) * ((c2 + 1.0f) * (x * 2.0f - 2.0f) + c2) + 2.0f) / 2.0f;
    }

    public static float EaseInElastic(float x)
    {
        float c4 = (2.0f * Mathf.PI) / 3.0f;
        return x == 0.0f
        ? 0.0f
        : x == 1.0f
        ? 1.0f
        : -Mathf.Pow(2.0f, 10.0f * x - 10.0f) * Mathf.Sin((x * 10.0f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float x)
    {
        float c4 = (2.0f * Mathf.PI) / 3.0f;
        return x == 0.0f
        ? 0.0f
        : x == 1.0f
        ? 1.0f
        : Mathf.Pow(2.0f, -10.0f * x) * Mathf.Sin((x * 10.0f - 0.75f) * c4) + 1.0f;
    }

    public static float EaseInOutElastic(float x)
    {
        float c5 = (2.0f * Mathf.PI) / 4.5f;
        return x == 0.0f
        ? 0.0f
        : x == 1.0f
        ? 1.0f
        : x < 0.5f
        ? -(Mathf.Pow(2.0f, 20.0f * x - 10.0f) * Mathf.Sin((20.0f * x - 11.125f) * c5)) / 2.0f
        : (Mathf.Pow(2.0f, -20.0f * x + 10.0f) * Mathf.Sin((20.0f * x - 11.125f) * c5)) / 2.0f + 1.0f;
    }

    public static float EaseInBounce(float x)
    {
        return 1.0f - EaseOutBounce(1.0f - x);
    }

    public static float EaseOutBounce(float x)
    {
        float a = 7.5625f;
        float b = 2.75f;
        if (x < 1.0f / b)
        {
            return a * x * x;
        }
        else if (x < 2.0f / b)
        {
            float c = (x - 1.5f / b);
            return a * c * c + 0.75f;
        }
        else if (x < 2.5 / b)
        {
            float c = (x - 2.25f / b);
            return a * c * c + 0.9375f;
        }
        else
        {
            float c = (x - 2.625f / b);
            return a * c * c + 0.984375f;
        }
    }

    public static float EaseInOutBounce(float x)
    {
        return x < 0.5f
        ? (1.0f - EaseOutBounce(1.0f - 2.0f * x)) / 2.0f
        : (1.0f + EaseOutBounce(2.0f * x - 1.0f)) / 2.0f;
    }

}
