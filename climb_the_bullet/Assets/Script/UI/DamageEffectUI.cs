using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffectUI : MonoBehaviour
{
    float EffectTimeCount = 0; // 経過時間
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EffectTimeCount += Time.deltaTime;
        if (EffectTimeCount <= 1.1f) return;
        Destroy(gameObject);
    }
}
