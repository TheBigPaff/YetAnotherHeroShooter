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
    private GameObject errorMessageGameObject;

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
        
        if(www.downloadHandler.text[0] == '0')
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

            errorMessageGameObject.SetActive(true);
            errorMessageGameObject.GetComponentInChildren<TMP_Text>().text = "User login failed. Error #" + www.downloadHandler.text;
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= 1 && passwordField.text.Length >= 1);
    }

    public void ResetErrorMessage()
    {
        errorMessageGameObject.SetActive(false);
    }

}
