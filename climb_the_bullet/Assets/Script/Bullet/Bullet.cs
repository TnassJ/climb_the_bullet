using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MotionList
{
    linear,
    circular,
    spiral,
    normal, //☆202411追加　イージングを使わないショット
    gravity,
    Sunset,
}

public class Bullet : MonoBehaviour
{
    float angle = 0; // 角度
    float speed = 3; // 速度
    Vector3 velocity; // 移動量

    // 20241028追加要素円運動
    float radius = 3; // 半径
    float angleVel = 1; // 角速度
    Vector3 center; // 回転中心

    //public AudioClip BulletClip; // 敵を倒した時に再生する SE
    float timeCount = 0; // 経過時間
    public EaseList easing;

    //202410追加 富士山Sunset用
    public bool LotationSwitch = true;
    public bool LargerSwitch = false;//発射後にサイズ変更を行うか
    float timeCountB = 0; // 経過時間
    public float LargerInterval;//サイズ変更のインターバル
    public float MagnificationRate;//インターバルごとに適用する拡大率
    public float LargersizeMax;//最大サイズ
    public float TimePoint1;
    public float TimePoint2;
    float timeCountC = 0;
    public Rigidbody2D rigid2D; //☆202411 重力弾のため追加

    public MotionList motion;

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
        var audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>();
        //audioSource.PlayOneShot(BulletClip);
        // 5秒後に削除
        //Destroy(gameObject, 5.0f);

        //☆202411 重力弾のため追加…しようとしたがうまくいかないためインスペクタから適用
        //if (Gravity_on == true)
        //this.rigid2D = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        timeCountB += Time.deltaTime; //拡大弾用
        timeCountC += Time.deltaTime; //拡大弾用
        var easeMethod = GetEasingMethod(easing);
        var shotSpeed = easeMethod(timeCount) + 1.0f;

        //☆202411追加　イージングを使わないショット
        if (motion == MotionList.normal)
        {
            shotSpeed = speed; //イージングを使わないため置き直す

            velocity.Normalize(); //正則化
            velocity *= shotSpeed; // ←のためにshotSpeedは初期値1.0とする
            // 毎フレーム、弾を移動させる
            transform.position += velocity * Time.deltaTime;
            DeleteBullet();
        }

        //☆202411追加　Sunset用　normalと同じだがDeleteBulletの範囲を拡大
        if (motion == MotionList.Sunset)
        {
            velocity.Normalize(); //正則化
            velocity *= shotSpeed; // ←のためにshotSpeedは初期値1.0とする
            // 毎フレーム、弾を移動させる
            transform.position += velocity * Time.deltaTime;
            DeleteBullet_Sunset();
        }

        //☆202411追加　重力をかけるため、上方向には弾を消さない
        if (motion == MotionList.gravity)
        {
            shotSpeed = speed; //イージングを使わないため置き直す

            velocity.Normalize(); //正則化
            velocity *= shotSpeed; // ←のためにshotSpeedは初期値1.0とする
            // 毎フレーム、弾を移動させる
            transform.position += velocity * Time.deltaTime;
            DeleteBullet_gravity();
        }

        if (motion == MotionList.linear)
        {
            velocity.Normalize(); //正則化
            velocity *= shotSpeed; // ←のためにshotSpeedは初期値1.0とする
            // 毎フレーム、弾を移動させる
            transform.position += velocity * Time.deltaTime;
            DeleteBullet();
        }

        // 円運動
        else if (motion == MotionList.circular)
        {
            Debug.Log("回転中");
            // 回転の中心位置
            //Vector3 center = transform.position;
            Vector3 VectorZ = new Vector3(0, 0, 1);
            // 中心点centerの周りを、軸axisで、period周期で円運動
            transform.RotateAround(
                center,
                VectorZ,
                angleVel * Time.deltaTime
            );
        }

        // 螺旋運動
        else if (motion == MotionList.spiral)
        {
            shotSpeed = speed; //イージングを使わないため置き直す

            //Debug.Log("螺旋回転中");
            // 回転の中心位置
            //Vector3 center = transform.position;
            Vector3 VectorZ = new Vector3(0, 0, 1);
            // 中心点centerの周りを、軸axisで、period周期で円運動
            transform.RotateAround(
                center,
                VectorZ,
                angleVel * Time.deltaTime
            );
            // 回転軸の外側方向のベクトル
            var outDirection = transform.position - center;
            outDirection.Normalize(); //正則化
            velocity = outDirection * shotSpeed; // ←のためにshotSpeedは初期値1.0とする
            // 毎フレーム、弾を移動させる
            transform.position += velocity * Time.deltaTime;
            DeleteBullet();

        }

        if (LotationSwitch == false)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 向きを固定

        //富士山　Sunset3用 拡大時間の限定（TimeCountCがTimePoint2にいる間だけ）
        if(timeCountC <= TimePoint1 || timeCountC >= TimePoint1 + TimePoint2) return;
        if (LargerSwitch == true && timeCountB > LargerInterval && this.transform.lossyScale.x < LargersizeMax)
        {
            var xScale = this.transform.lossyScale.x;
            var yScale = this.transform.lossyScale.y;
            var zScale = this.transform.lossyScale.z;
            this.transform.localScale = new Vector3(xScale * MagnificationRate, yScale * MagnificationRate, zScale * MagnificationRate);
            timeCountB = 0;
        }

    }

    // !!追加!!
    // 角度と速度を設定する関数
    public void Init(float input_angle, float input_speed)
    {
        angle = input_angle;
        speed = input_speed;
        motion = MotionList.linear;
    }

    // 20241028追加要素円運動
    // 半径と角速度、中心を初期化
    public void circularInit(float input_radius, float input_angleVel, Vector3 input_center)
    {
        radius = input_radius;
        angleVel = input_angleVel;
        center = input_center;
        motion = MotionList.circular;
    }

    // 20241028追加要素螺旋運動
    // 半径と角速度、中心を初期化
    public void spiralInit(float input_speed, float input_radius, float input_angleVel, Vector3 input_center)
    {
        speed = input_speed;
        radius = input_radius;
        angleVel = input_angleVel;
        center = input_center;
        motion = MotionList.spiral;
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

    //☆202411追加
    public void DeleteBullet_gravity()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -6.0f | xp > 6.0f | yp < -6.0f )
        {
            Destroy(gameObject);
        }

    }

    //☆202411追加
    public void DeleteBullet_Sunset()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -7.0f | xp > 7.0f | yp < -7.0f | yp > 7.0f)
        {
            Destroy(gameObject);
        }

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
