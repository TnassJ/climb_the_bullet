using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField]
    private Material _transitionIn;
    //public AudioClip TransitionClip; //  トランジション SE

    void Start()
    {
        // SE を再生する
        //var audioSource = FindObjectOfType<AudioSource>();
        //audioSource.PlayOneShot( TransitionClip );
        StartCoroutine( BeginTransition() );
    }

    IEnumerator BeginTransition()
    {
        yield return Animate( _transitionIn, 1 );
    }

    /// <summary>
    /// time秒かけてトランジションを行う
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Animate(Material material, float time)
    {
        GetComponent<Image>().material = material;
        float current = 0;
        while (current < time) {
            material.SetFloat( "_Alpha", current / time );
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        material.SetFloat( "_Alpha", 1 );
    }
}
