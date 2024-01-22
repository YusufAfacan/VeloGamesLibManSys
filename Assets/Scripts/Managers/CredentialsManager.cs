using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class CredentialsManager : MonoBehaviour
{
    public List<UserCredential> credentials;

    public TextMeshProUGUI registerUserNameInputField;
    public TextMeshProUGUI registerPasswordInputField;

    public TextMeshProUGUI loginUserNameInputField;
    public TextMeshProUGUI loginPasswordInputField;

    
    public static CredentialsManager instance;

    public event EventHandler OnRegisterSuccessful;
    public event EventHandler<OnLoginSuccessfulEventArgs> OnLoginSuccessful;
    public class OnLoginSuccessfulEventArgs : EventArgs
    {
        public UserCredential loggedInUser;
    }

    private void Awake()
    {
        instance = this;

        LoadCredentials();
    }

    private void LoadCredentials()
    {
        string path = Application.persistentDataPath + "/credentials.dat";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            credentials = formatter.Deserialize(stream) as List<UserCredential>;
            stream.Close();
        }
        else
        {
            credentials = new List<UserCredential>();
        }
    }

    private void SaveCredentials()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/credentials.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, credentials);
        stream.Close();
    }

    private void OnApplicationQuit()
    {
        SaveCredentials();
    }

    public void RegisterCredentials()
    {
        UserCredential newRegister = new UserCredential();
        newRegister.UserName = registerUserNameInputField.text;
        newRegister.Password = registerPasswordInputField.text;
        credentials.Add(newRegister);

        OnRegisterSuccessful?.Invoke(this, EventArgs.Empty);
    }

    public void LoginCredentials()
    {
        for (int i = 0; i < credentials.Count; i++)
        {
            if (loginUserNameInputField.text == credentials[i].UserName)
            {
                if (credentials[i].Password == loginPasswordInputField.text)
                {
                    OnLoginSuccessful?.Invoke(this, new OnLoginSuccessfulEventArgs { loggedInUser = credentials[i] });
                }
            } 
        }
    }
}
