using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class ProcessManager : MonoBehaviour
{
    //チュートリアルフラグ管理変数
    public bool Stage0A = true;
    public bool StageWay0 = false;
    public bool Stage0B = false;
    public bool StageBoss0 = false;
    public bool Stage0C = false;
    public bool StageStop = false;
    //フローチャート参照
    public Fungus.Flowchart flowChart;
    // ポーズ対象
    //public List<ProcessManager> targets = new List<ProcessManager>();

 
    // Start is called before the first frame update
    void Start()
    {
        //flowChart = GetComponent<Flowchart>();
        Stage0A = true;
        flowChart.SetBooleanVariable("Stage0A", Stage0A);
        
    }

    // Update is called once per frame
    void Update()
    {
        Stage0A = flowChart.GetBooleanVariable("Stage0A");
        StageWay0 = flowChart.GetBooleanVariable("StageWay0");
        if ((StageWay0 == false) && (StageBoss0 == false))
        {
            //StageStop = true;
            EnemyMove.EnemyPouse = true;
            TargetPauser.Pause ();
        }
        if (StageWay0 == true)
        {
            TargetPauser.Resume();
            EnemyMove.EnemyPouse = false;
            
        }

    }
}
