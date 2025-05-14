using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLoginFlow : MonoBehaviour
{
    [Header("初期プレイヤー名（新規ユーザー用）")] [SerializeField]
    string defaultPlayerName = "NewPlayer";

    [SerializeField] private UIManager uiManager;

    void Start()
    {
        // ゲームスタート → サイレントログイン
        SilentLogin();
    }

    void SilentLogin()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetUserAccountInfo = true
                }
            },
            result =>
            {
                Debug.Log("サイレントログイン成功");

                // ユーザーデータを確認して存在しなければログイン画面へ
                CheckUserDataAfterLogin(result.NewlyCreated);
            },
            error =>
            {
                Debug.LogError("サイレントログイン失敗: " + error.GenerateErrorReport());
                uiManager.ShowLogin(); // 通常ログイン画面を表示
            });
    }


    void CheckUserDataAfterLogin(bool newlyCreated)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
            {
                bool hasValidData = result.Data != null && result.Data.ContainsKey("Quests");

                if (newlyCreated || !hasValidData)
                {
                    Debug.LogWarning("UserDataが存在しないか、新規ユーザー → ログイン画面を表示");
                    uiManager.ShowLogin();
                }
                else
                {
                    Debug.Log("UserDataあり → メインメニューへ");
                    ShowMainMenu();
                }
            },
            error =>
            {
                Debug.LogError("UserData確認失敗 → 念のためログイン画面を表示: " + error.GenerateErrorReport());
                uiManager.ShowLogin();
            });
    }


    void ShowMainMenu()
    {
        Debug.Log("メインメニュー表示（UIへ接続してください）");

        // 他の認証方法と連携済みか確認（例としてCustomIDのみかどうか）
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
            {
                var accountInfo = result.AccountInfo;
                bool isLinked = accountInfo.FacebookInfo != null || accountInfo.GoogleInfo != null ||
                                accountInfo.Username != null;

                if (!isLinked)
                {
                    Debug.Log("追加認証未連携 → 登録・連携を促す");
                    PromptAccountLinkOptions();
                }
                else
                {
                    Debug.Log("他の認証手段と連携済み → 完全に認証OK");
                    FullyAuthenticated();
                }
            },
            error => { Debug.LogError("認証状況確認失敗: " + error.GenerateErrorReport()); });
    }

    void PromptAccountLinkOptions()
    {
        // ユーザー名・パスワード登録や、Facebook/Google連携を促すUIを表示
        Debug.Log("登録・連携UIを表示（実装してください）");
    }

    void FullyAuthenticated()
    {
        Debug.Log("完全に認証済み！ゲームに進行できます");
        // ゲーム開始処理へ
    }

    // 任意: パスワード登録
    public void RegisterUsernamePassword(string email, string password)
    {
        PlayFabClientAPI.AddUsernamePassword(new AddUsernamePasswordRequest
            {
                Email = email,
                Password = password,
                Username = Guid.NewGuid().ToString() // 仮
            },
            result => Debug.Log("ユーザー名とパスワードの登録成功"),
            error => Debug.LogError("登録失敗: " + error.GenerateErrorReport()));
    }

    // 任意: Facebook連携
    public void LinkFacebook(string accessToken)
    {
        PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest
            {
                AccessToken = accessToken,
                ForceLink = true
            },
            result =>
            {
                Debug.Log("Facebook連携成功");
                FullyAuthenticated();
            },
            error => Debug.LogError("Facebook連携失敗: " + error.GenerateErrorReport()));
    }

    // 任意: Google連携
    public void LinkGoogle(string serverAuthCode)
    {
        PlayFabClientAPI.LinkGoogleAccount(new LinkGoogleAccountRequest
            {
                ServerAuthCode = serverAuthCode,
                ForceLink = true
            },
            result =>
            {
                Debug.Log("Google連携成功");
                FullyAuthenticated();
            },
            error => Debug.LogError("Google連携失敗: " + error.GenerateErrorReport()));
    }
}