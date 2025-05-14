using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

public class PlayFabController : MonoBehaviour
{
    void Start()
    {
        PlayFabAuthService.Instance.Authenticate(Authtypes.Silent);
    }

    void OnEnable()
    {
        PlayFabAuthService.OnLoginSuccess += PlayFabLogin_OnLoginSuccess;
    }

    private void PlayFabLogin_OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success!");
        GetUserData(); // ← ログイン成功後に呼び出す
    }

    private void OnDisable()
    {
        PlayFabAuthService.OnLoginSuccess -= PlayFabLogin_OnLoginSuccess;
    }

    List<UserQuestData> UserQuestDatas { get; set; }

    void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => {
            if (result.Data != null && result.Data.ContainsKey("Quests"))
            {
                try
                {
                    // JSONデータの読み込み（例: [{"Id":1,"Score":100}, {"Id":2,"Score":200}]）
                    string json = result.Data["Quests"].Value;
                    UserQuestDatas = PlayFabSimpleJson.DeserializeObject<List<UserQuestData>>(json);

                    Debug.Log("UserQuestDatas取得成功：" + UserQuestDatas.Count + "件");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("デシリアライズ失敗: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("QuestsキーがUserDataに存在しません。");
            }
        }, error => {
            Debug.LogError("UserData取得失敗: " + error.GenerateErrorReport());
        });
    }

    [System.Serializable]
    public class UserQuestData
    {
        public int Id;
        public int Score;
    }
}