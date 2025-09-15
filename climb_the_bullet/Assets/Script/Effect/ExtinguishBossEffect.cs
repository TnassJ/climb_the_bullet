using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguishBossEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void callEffect(Vector3 Position, ExtinguishEnemy ExtinguishPrefab)
    {
        // ボスなのでエフェクトを周囲に5つ出す
        StartCoroutine(ContinuousExtinguishEffect(Position, ExtinguishPrefab));
    }

    IEnumerator ContinuousExtinguishEffect(Vector3 Position, ExtinguishEnemy ExtinguishPrefab)
    {
        Debug.Log("effect1start");
        Instantiate(
            ExtinguishPrefab,
            Position + new Vector3(1, 1, 0),
            Quaternion.identity
        );
        Debug.Log("effect1end");
        yield return new WaitForSeconds(0.1f);
        Debug.Log("effect2");
        Instantiate(
            ExtinguishPrefab,
            Position + new Vector3(-0.7f, -0.8f, 0),
            Quaternion.identity
        );
        yield return new WaitForSeconds(0.1f);
        Debug.Log("effect3");
        Instantiate(
            ExtinguishPrefab,
            Position + new Vector3(-0.75f, 0.6f, 0),
            Quaternion.identity
        );
        yield return new WaitForSeconds(0.1f);
        Instantiate(
            ExtinguishPrefab,
            Position + new Vector3(0.5f, -1, 0),
            Quaternion.identity
        );
        yield return new WaitForSeconds(0.1f);
        Instantiate(
            ExtinguishPrefab,
            Position,
            Quaternion.identity
        );
        yield return new WaitForSeconds(0.1f);
        // 自身を消す
        Destroy(gameObject);
    }
}

