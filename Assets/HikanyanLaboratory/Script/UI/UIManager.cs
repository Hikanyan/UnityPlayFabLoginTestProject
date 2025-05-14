using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel;

    void Start()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }

    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void HideAll()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
    }
}