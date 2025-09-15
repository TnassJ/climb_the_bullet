using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HPBarManager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GaugeReduction(float HPmax, float HPnow, float reducationValue)
    {
        var valueFrom = HPnow / HPmax;
        var valueTo = (HPnow - reducationValue) / HPmax;

        // óŒÉQÅ[ÉWå∏è≠
        this.GetComponentInChildren<Image>().fillAmount = valueTo;

        //if (redGaugeTween != null)
        //{
        //    redGaugeTween.Kill();
        //}

        //// ê‘ÉQÅ[ÉWå∏è≠
        //redGaugeTween = DOTween.To(
        //    () => valueFrom,
        //    x => {
        //        RedGauge.fillAmount = x;
        //    },
        //    valueTo,
        //    time
        //);
    }

}
