using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager instance;

    private CredentialsManager credentialsManager;
    private LibraryManager libraryManager;

    [Header("Main Menu Panel")]
    public Button loginPanelButton;
    public Button registerPanelButton;
    public GameObject mainMenuPanel;

    [Header("Register Panel")]
    public GameObject registerPanel;
    public Button registerPanelBackButton;
    public Button attemptRegisterButton;
    public TextMeshProUGUI registerSuccessfulText;

    [Header("Login Panel")]
    public GameObject loginPanel;
    public Button attemptLoginButton;
    public Button loginPanelBackButton;

    [Header("Library Panel")]
    public GameObject libraryMenuPanel;
    public Button libraryMenuDonateBookButton;
    public Button LibraryMenuReturnBookButton;
    public Button libraryMenuBorrowBookButton;
    public TextMeshProUGUI userNameText;

    [Header("Donate Panel")]
    public GameObject donateBookPanel;
    public Button donateBookPanelBackButton;
    public Button donateBookButton;

    [Header("Borrow Panel")]
    public GameObject searchBookPanel;
    public Button searchBookPanelBackButton;

    [Header("Return Panel")]
    public GameObject returnBookPanel;
    public Button returnBookPanelBackButton;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        credentialsManager = CredentialsManager.instance;
        libraryManager = LibraryManager.instance;

        credentialsManager.OnRegisterSuccessful += CredentialsManager_OnRegisterSuccessful;
        credentialsManager.OnLoginSuccessful += CredentialsManager_OnLoginSuccessful;

        loginPanelButton.onClick.AddListener (OpenLoginPanel);
        loginPanelBackButton.onClick.AddListener (CloseLoginPanel);
        registerPanelButton.onClick.AddListener (OpenRegisterPanel);
        registerPanelBackButton.onClick.AddListener (CloseRegisterPanel);
        attemptRegisterButton.onClick.AddListener (credentialsManager.RegisterCredentials);
        attemptLoginButton.onClick.AddListener (credentialsManager.LoginCredentials);

        libraryMenuDonateBookButton.onClick.AddListener(OpenDonateBookPanel);
        LibraryMenuReturnBookButton.onClick.AddListener(OpenReturnBookPanel);
        libraryMenuBorrowBookButton.onClick.AddListener(OpenBorrowBookPanel);

        donateBookPanelBackButton.onClick.AddListener(CloseDonateBookPanel);
        returnBookPanelBackButton.onClick.AddListener(CloseReturnBookPanel);
        searchBookPanelBackButton.onClick.AddListener(CloseSearchBookPanel);

        donateBookButton.onClick.AddListener(libraryManager.DonateBook);
        
    }

    private void CredentialsManager_OnLoginSuccessful(object sender, CredentialsManager.OnLoginSuccessfulEventArgs e)
    {
        mainMenuPanel.SetActive (false);
        loginPanel.SetActive (false);
        libraryMenuPanel.SetActive (true);
        userNameText.text = e.loggedInUser.UserName;
    }

    private void CredentialsManager_OnRegisterSuccessful(object sender, EventArgs e)
    {
        registerSuccessfulText.gameObject.SetActive(true);
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenRegisterPanel()
    {
        registerPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenDonateBookPanel()
    {
        donateBookPanel.SetActive(true);
        libraryMenuPanel.SetActive(false);
    }

    public void CloseDonateBookPanel()
    {
        donateBookPanel.SetActive(false);
        libraryMenuPanel.SetActive(true);
    }

    public void OpenReturnBookPanel()
    {
        returnBookPanel.SetActive(true);
        libraryMenuPanel.SetActive(false);
        libraryManager.ShowReturnableBooks();
    }

    public void CloseReturnBookPanel()
    {
        returnBookPanel.SetActive(false);
        libraryMenuPanel.SetActive(true);
    }


    public void OpenBorrowBookPanel()
    {
        
        searchBookPanel.SetActive(true);
        libraryMenuPanel.SetActive(false);
        libraryManager.ShowAllBooks();
    }

    public void CloseSearchBookPanel()
    {
        searchBookPanel.SetActive(false);
        libraryMenuPanel.SetActive(true);
    }
}
