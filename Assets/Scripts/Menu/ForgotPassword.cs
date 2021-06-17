using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ForgotPassword : MonoBehaviour
{
    [SerializeField]
    private GameObject passwordMenu;
    [SerializeField]
    private GameObject loginMenu;

    [SerializeField]
    private GameObject feedbackMessageGO;

    [SerializeField]
    private TMP_InputField emailField;
    [SerializeField]
    public Button submitButton;
    IEnumerator SendEmail()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/YetAnotherMobileShooter/forgot_password.php", form);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text[0] == '0')
        {
            feedbackMessageGO.SetActive(true);
            feedbackMessageGO.GetComponentInChildren<TMP_Text>().text = "An email was sent to your inbox with instructions for changing your password";
        }
        else
        {
            Debug.Log("Send email failed. Error #" + www.downloadHandler.text);

            feedbackMessageGO.SetActive(true);
            feedbackMessageGO.GetComponentInChildren<TMP_Text>().text = "Send email failed. Error #" + www.downloadHandler.text;
        }
    }

    public void VerifyInputs()
    {
        submitButton.interactable = (emailField.text.Length >= 6);
    }

    public void ResetErrorMessage()
    {
        feedbackMessageGO.SetActive(false);
    }

    public void SendEmailButton()
    {
        StartCoroutine(SendEmail());
    }

    public void GoBackToLogin()
    {
        ResetErrorMessage();
        passwordMenu.SetActive(false);
        loginMenu.SetActive(true);
    }
}
