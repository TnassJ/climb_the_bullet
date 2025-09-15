using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierParticleEffect : MonoBehaviour
{
   



    // Update is called once per frame
    void Update()
    {
        var PlayerObject = GameObject.Find("Player");
        this.transform.position = PlayerObject.transform.position;
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("衝突");
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Debug.Log("弾消滅");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("衝突");
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
            Debug.Log("弾消滅");
        }
    }
}
