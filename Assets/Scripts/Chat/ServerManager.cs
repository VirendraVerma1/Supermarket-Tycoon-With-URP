using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerManager : MonoBehaviour
{
    string CreateAccountURL = "https://kreasarapps.000webhostapp.com/SupermarketTycoon/createaccount.php";
    string LogInAccountURL = "https://kreasarapps.000webhostapp.com/SupermarketTycoon/login.php";
    string GlobalChatSendURL = "https://kreasarapps.000webhostapp.com/SupermarketTycoon/sendglobalmessage.php";
    string GlobalChatRecieveURL = "https://kreasarapps.000webhostapp.com/SupermarketTycoon/receiveglobalmessage.php";

    public Text ResultText;

    [Header("All Pannels")]
    public GameObject CreateAccountPannel;
    public GameObject LoginPannel;
    public GameObject GlobalChatPannel;

    void Start()
    {
        CreateAccountPannel.SetActive(false);
        LoginPannel.SetActive(false);
        GlobalChatPannel.SetActive(false);

        if (saveload.accountID == " ")
        {
            ActivatePanel(LoginPannel.name);
        }

        ResultText.gameObject.SetActive(false);
        //print(saveload.accountID);
       // print(saveload.playerName);

        StartCoroutine(RecieveGlobalMessage());
    }

    #region OnCreateAccount
    [Header("CreateAccount")]
    public InputField EmailCreateAccountText;
    public InputField PasswordCreateAccountText;
    public InputField NameCreateAccountText;

    public void OnCreateAccountButtonIsPressed()
    {
        string email = EmailCreateAccountText.text;
        string password = PasswordCreateAccountText.text;
        string name = NameCreateAccountText.text;
        saveload.playerName = name;
        StartCoroutine(CreateAccountSend(email, password, name));
    }

    IEnumerator CreateAccountSend(string email,string password,string name)
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "Connecting...";
        ResultText.color = Color.green;

        WWWForm form = new WWWForm();
        
        form.AddField("username", name);
        form.AddField("email", email);
        form.AddField("password", password);

        WWW www = new WWW(CreateAccountURL, form);
        yield return www;

        

        if (www.text != "" || www.text != " " || www.text != null || !www.text.Contains("<br>"))
        {
            if (!www.text.Contains("<br>"))
            {
                if (www.text == "Account Already Exist")
                {
                    ResultText.text = "Account Already Exist";
                    ResultText.color = Color.red;
                }
                else if (www.text == "Connection Failed")
                {
                    ResultText.text = "Failed to connect server";
                    ResultText.color = Color.red;
                }
                else if (www.text.Contains("id:"))
                {
                    CreateAccountPannel.SetActive(false);
                    saveload.accountID = GetDataValue(www.text, "id:");
                    saveload.Save();
                    ResultText.text = "Connected";
                    ResultText.color = Color.green;
                }
                else
                {
                    ResultText.text = "Failed to connect server";
                    ResultText.color = Color.red;
                }
            }
            else
            {
                ResultText.text = "Server is in maintainence";
                ResultText.color = Color.red;
            }
        }
        else
        {
            ResultText.text = "Check your connection";
            ResultText.color = Color.red;
        }

        StartCoroutine(HideResultText());
    }

#endregion


    #region OnLogInAccount

    [Header("LogInAccount")]
    public InputField EmailLogInAccountText;
    public InputField PasswordLogInAccountText;

    public void OnLogInAccountButtonIsPressed()
    {
        string email = EmailLogInAccountText.text;
        string password = PasswordLogInAccountText.text;
        StartCoroutine(LogInAccountSend(email, password));
    }

    IEnumerator LogInAccountSend(string email, string password)
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "Connecting...";
        ResultText.color = Color.green;

        WWWForm form = new WWWForm();

        form.AddField("email", email);
        form.AddField("password", password);

        WWW www = new WWW(LogInAccountURL, form);
        yield return www;
        
        if (www.text != "" || www.text != " " || www.text != null || !www.text.Contains("<br>"))
        {
            if (!www.text.Contains("<br>"))
            {
                if (www.text == "Incorrect password")
                {
                    ResultText.text = "Incorrect password";
                    ResultText.color = Color.red;
                }
                else if (www.text == "server error")
                {
                    ResultText.text = "Failed to connect server";
                    ResultText.color = Color.red;
                }
                else if (www.text.Contains("id:"))
                {
                    LoginPannel.SetActive(false);
                    saveload.accountID = GetDataValue(www.text, "id:");
                    saveload.playerName = GetDataValue(www.text, "username:");
                    saveload.Save();
                    ResultText.text = "Connected";
                    ResultText.color = Color.green;
                    print(www.text);
                }
                else
                {
                    ResultText.text = "Failed to connect server";
                    ResultText.color = Color.red;
                }
            }
            else
            {
                ResultText.text = "Server is in maintainence";
                ResultText.color = Color.red;
            }
        }
        else
        {
            ResultText.text = "Check your connection";
            ResultText.color = Color.red;
        }

        StartCoroutine(HideResultText());
    }

    

    #endregion

    #region globalChat

    [Header("Global Chat")]
    public InputField ChatText;

    public GameObject LeftChatText;
    public GameObject RightChatText;
    public Transform ChatInstantiatePosition;

    public void OnSendMessageButtonPressed()
    {
        string chatText = ChatText.text;

        if (chatText != "" || chatText != " " || chatText != null)
        {
            StartCoroutine(SendChat(chatText));
        }
    }

    IEnumerator SendChat(string chat)
    {
        ResultText.gameObject.SetActive(true);
        ResultText.text = "";

        WWWForm form = new WWWForm();

        form.AddField("id", saveload.accountID);
        form.AddField("chat", chat);
        form.AddField("name", saveload.playerName);

        WWW www = new WWW(GlobalChatSendURL, form);
        yield return www;

        if (www.text != "" || www.text != " " || www.text != null || !www.text.Contains("<br>"))
        {
            if (!www.text.Contains("<br>"))
            {
                if (www.text == "Connection Failed")
                {
                    ResultText.text = "Failed to connect server";
                    ResultText.color = Color.red;
                    
                }
                else if (www.text.Contains("DateTime")) 
                {
                    //instantiate right chat
                    GameObject go = Instantiate(RightChatText);
                    go.transform.SetParent(ChatInstantiatePosition);
                    go.transform.localScale = Vector3.one;
                    go.transform.GetComponent<Text>().text = chat;
                    go.transform.Find("Name").GetComponent<Text>().text = saveload.playerName;

                    saveload.globalDateTime = GetDataValue(www.text, "DateTime:");
                    ChatText.text = "";
                }
                else
                {
                    ResultText.text = "Server is in maintainence";
                    ResultText.color = Color.red;
                    
                   
                }

            }
            else
            {
                ResultText.text = "Server is in maintainence";
                ResultText.color = Color.red;
                
            }
        }
        else
        {
            ResultText.text = "Check your connection";
            ResultText.color = Color.red;
            
        }

        ResultText.gameObject.SetActive(false);
    }

    int n = 1;

    IEnumerator RecieveGlobalMessage()
    {
        while (n > 0)
        {
            yield return new WaitForSeconds(0.3f);
            WWWForm form = new WWWForm();

            form.AddField("datetime", saveload.globalDateTime);

            WWW www = new WWW(GlobalChatRecieveURL, form);
            yield return www;
            //print(www.text);
            if (www.text != "" || www.text != null || !www.text.Contains("<br>"))
            {
                if (!www.text.Contains("<br>"))
                {
                    if (www.text.Contains("Id:"))
                    {
                        //have chat

                        string data = www.text;
                        string[] items = data.Split(';');
                        int len = items.Length;

                        for (int i = 0; i < len - 1; i++)
                        {
                            GameObject go = Instantiate(LeftChatText);
                            go.transform.SetParent(ChatInstantiatePosition);
                            go.transform.localScale = Vector3.one;
                            go.transform.GetComponent<Text>().text = GetDataValue(items[i], "Chat:");
                            go.transform.Find("Name").GetComponent<Text>().text = GetDataValue(items[i], "Name:");
                            saveload.globalDateTime = GetDataValue(items[i], "DateTime:");
                        }

                    }
                    else if (www.text == " ")
                    {
                        //dont have any chat
                    }
                }
            }
        }
    }

    


    #endregion

    public void OnClosaeGlobalChatButtonPressed()
    {
        GlobalChatPannel.SetActive(false);
    }

    public void ToOpeCreateAccountButtonPressed()
    {
        ActivatePanel(CreateAccountPannel.name);
    }

    public void ToOpeLogInAccountButtonPressed()
    {
        ActivatePanel(LoginPannel.name);
    }


    public void OnGlobalChatButtonPressed()
    {
        ActivatePanel(GlobalChatPannel.name);
    }

    public void ActivatePanel(string panelToBeActivated)
    {
        CreateAccountPannel.SetActive(panelToBeActivated.Equals(CreateAccountPannel.name));
        LoginPannel.SetActive(panelToBeActivated.Equals(LoginPannel.name));
        GlobalChatPannel.SetActive(panelToBeActivated.Equals(GlobalChatPannel.name));
        
    }
    public static string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
            value = value.Remove(value.IndexOf("|"));
        return value;
    }
    IEnumerator HideResultText()
    {
        yield return new WaitForSeconds(2);
        ResultText.gameObject.SetActive(false);
    }
}
