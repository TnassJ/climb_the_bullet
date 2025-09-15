using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguishEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 演出が完了したら削除する
        var particleSystem = GetComponent<ParticleSystem>();
        Destroy( gameObject, particleSystem.main.duration );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
