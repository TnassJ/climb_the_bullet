using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCutin : MonoBehaviour
{
    float DisplayTime = 15.0f;
    float timeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeCount += Time.deltaTime;

        if (timeCount > DisplayTime)
        {
            Destroy(gameObject);
        }
    }
}
