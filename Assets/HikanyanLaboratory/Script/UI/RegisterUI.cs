using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] UIManager uiManager;
    [SerializeField] Button registerButton;
    [SerializeField] Button backToLoginButton;
    [SerializeField] Button closeButton;

    void Start()
    {
        registerButton.onClick.AddListener(OnRegisterClicked);
        backToLoginButton.onClick.AddListener(OnBackToLoginClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    public void OnRegisterClicked()
    {
        string email = emailInput.text.Trim();
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("すべての項目を入力してください");
            return;
        }

        PlayFabAuthService.Instance.Email = email;
        PlayFabAuthService.Instance.Username = username;
        PlayFabAuthService.Instance.Password = password;
        PlayFabAuthService.Instance.Authenticate(Authtypes.RegisterPlayFabAccount);
    }

    public void OnBackToLoginClicked()
    {
        uiManager.ShowLogin();
    }

    public void OnCloseClicked()
    {
        uiManager.HideAll();
    }
}