using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Registration : MonoBehaviour
{
    [SerializeField]
    private GameObject registerMenu;
    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject feedbackMessageGO;

    [SerializeField]
    private TMP_InputField nameField;
    [SerializeField]
    private TMP_InputField emailField;
    [SerializeField]
    private TMP_InputField passwordField;
    [SerializeField]
    public Button submitButton;

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/YetAnotherMobileShooter/register.php", form);
        yield return www.SendWebRequest();

        if(www.downloadHandler.text == "0")
        {
            Debug.Log("User created successfully.");

            DBManager.username = nameField.text;
            transform.parent.GetComponent<MainMenu>().startMenuPlayerDisplay.text = "User: " + DBManager.username;
            transform.parent.GetComponent<MainMenu>().mainMenuPlayerDisplay.text = "User: " + DBManager.username;

            ResetErrorMessage();
            registerMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            Debug.Log("User creation failed. Error #" + www.downloadHandler.text);

            feedbackMessageGO.SetActive(true);
            feedbackMessageGO.GetComponentInChildren<TMP_Text>().text = "User creation failed. Error #" + www.downloadHandler.text;
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 1 && passwordField.text.Length >= 1 && emailField.text.Length > 6);
    }
    public void ResetErrorMessage()
    {
        feedbackMessageGO.SetActive(false);
    }
}
