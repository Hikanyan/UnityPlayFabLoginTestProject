using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailOrUsernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] UIManager uiManager;
    [SerializeField] Button loginButton;
    [SerializeField] Button registerButton;
    [SerializeField] Button closeButton;
    [SerializeField] TMP_Text warningText;

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
        PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
        PlayFabAuthService.OnPlayFabError += OnLoginError;
        warningText.text = "";
    }

    void OnDestroy()
    {
        PlayFabAuthService.OnLoginSuccess -= OnLoginSuccess;
        PlayFabAuthService.OnPlayFabError -= OnLoginError;
    }

    public void OnLoginClicked()
    {
        string id = emailOrUsernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            warningText.text = "メールアドレスとパスワードを入力してください";
            return;
        }

        warningText.text = ""; // 入力成功したので警告を消す

        PlayFabAuthService.Instance.Email = id; // ユーザー名でもOK
        PlayFabAuthService.Instance.Password = password;
        PlayFabAuthService.Instance.Authenticate(Authtypes.EmailAndPassword);
    }

    public void OnRegisterButtonClicked()
    {
        uiManager.ShowRegister();
    }

    public void OnCloseClicked()
    {
        uiManager.HideAll();
    }

    void OnLoginError(PlayFabError error)
    {
        // 典型的な「存在しない or パスワードミス」エラーコードを判定
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
            case PlayFabErrorCode.InvalidEmailOrPassword:
            case PlayFabErrorCode.InvalidUsernameOrPassword:
            case PlayFabErrorCode.AccountNotFound:
                warningText.text = "アカウントが見つかりません。メールアドレスやパスワードを確認してください。";
                break;

            default:
                warningText.text = $"ログイン失敗: {error.ErrorMessage}";
                break;
        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("メールログイン成功！PlayFabId: " + result.PlayFabId);
    }
}