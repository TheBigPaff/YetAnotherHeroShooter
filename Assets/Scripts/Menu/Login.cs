using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private GameObject loginMenu;
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject feedbackMessageGO;

    [SerializeField]
    private TMP_InputField nameField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    public Button submitButton;

    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    IEnumerator LoginPlayer()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/YetAnotherMobileShooter/login.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            feedbackMessageGO.SetActive(true);
            feedbackMessageGO.GetComponentInChildren<TMP_Text>().text = "The server is not reachable";
        }
        else if (www.downloadHandler.text[0] == '0')
        {
            DBManager.username = nameField.text;
            transform.parent.GetComponent<MainMenu>().startMenuPlayerDisplay.text = "User: " + DBManager.username;
            transform.parent.GetComponent<MainMenu>().mainMenuPlayerDisplay.text = "User: " + DBManager.username;

            ResetErrorMessage();
            loginMenu.SetActive(false);
            mainMenu.SetActive(true);
            nameField.text = "";
            passwordField.text = "";
        }
        else
        {
            Debug.Log("User login failed. Error #" + www.downloadHandler.text);

            feedbackMessageGO.SetActive(true);
            feedbackMessageGO.GetComponentInChildren<TMP_Text>().text = "User login failed. Error #" + www.downloadHandler.text;
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 1 && passwordField.text.Length >= 1);
    }

    public void ResetErrorMessage()
    {
        feedbackMessageGO.SetActive(false);
    }

}
