// using System.Collections;
// using System.Collections.Generic;
// //using Frontier.Data;
// using UnityEngine;
// using UnityEngine.AddressableAssets;

// public class PreLoadShotParam
// {
//     public List<TODAData> TODAData;
//     public List<TENKOData> TENKOData;
//     public List<TENKUData> TENKUData;
//     public List<RIKUGOData> RIKUGOData;
//     public bool loadFinish = false;
//     const string BASE_ADDRESS = "ParameterSet/";

//     // 4式神用のパラメータをロードするスクリプト
//     async public void DataLoad()
//     {
//         // 計測開始
//         var t = Time.realtimeSinceStartup;

//         string TODAassetAddress = "TODAShotSetting.asset";
//         string TODAPath = BASE_ADDRESS + TODAassetAddress;
//         // アセットのパラメータ読み込み
//         var TODAsetting = await Addressables.LoadAssetAsync<TODASetting>(TODAPath).Task;
//         // 外部から読みとるデータ用意
//         TODAData = TODAsetting.DataList;
//         //Debug.Log("ShotSpeed" + setting.DataList[0].ShotSpeed);
//         Addressables.Release(TODAsetting); // リソースの解放

//         string TENKOassetAddress = "TENKOShotSetting.asset";
//         string TENKOPath = BASE_ADDRESS + TENKOassetAddress;
//         // アセットのパラメータ読み込み
//         var TENKOsetting = await Addressables.LoadAssetAsync<TENKOSetting>(TENKOPath).Task;
//         // 外部から読みとるデータ用意
//         TENKOData = TENKOsetting.DataList;
//         //Debug.Log("ShotSpeed" + setting.DataList[0].ShotSpeed);
//         Addressables.Release(TENKOsetting); // リソースの解放

//         string TENKUassetAddress = "TENKUShotSetting.asset";
//         string TENKUPath = BASE_ADDRESS + TENKUassetAddress;
//         // アセットのパラメータ読み込み
//         var TENKUsetting = await Addressables.LoadAssetAsync<TENKUSetting>(TENKUPath).Task;
//         // 外部から読みとるデータ用意
//         TENKUData = TENKUsetting.DataList;
//         //Debug.Log("ShotSpeed" + setting.DataList[0].ShotSpeed);
//         Addressables.Release(TENKUsetting); // リソースの解放

//         string RIKUGOassetAddress = "RIKUGOShotSetting.asset";
//         string RIKUGOPath = BASE_ADDRESS + RIKUGOassetAddress;
//         // アセットのパラメータ読み込み
//         var RIKUGOsetting = await Addressables.LoadAssetAsync<RIKUGOSetting>(RIKUGOPath).Task;
//         // 外部から読みとるデータ用意
//         RIKUGOData = RIKUGOsetting.DataList;
//         //Debug.Log("ShotSpeed" + setting.DataList[0].ShotSpeed);
//         Addressables.Release(RIKUGOsetting); // リソースの解放

//         Debug.Log($"処理時間：{Time.realtimeSinceStartup - t}");
//         loadFinish = true; // 処理の終わりを伝える変数
        


//     }

// }