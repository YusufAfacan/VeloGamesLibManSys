using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class LibraryManager : MonoBehaviour
{
    public List<Book> books;
    public List<BookUI> showedAllBooks;
    public List<BookUI> showedBorrowedBooks;

    public UserCredential currentUser;

    public static LibraryManager instance;
    private CredentialsManager credentialsManager;

    public TextMeshProUGUI donatedBookTitle;
    public TextMeshProUGUI donatedBookAuthor;
    public TextMeshProUGUI donatedBookISBN;

    public GameObject bookUiPrefab;
    public GameObject libraryContentParent;
    public GameObject returnableContentParent;

    public TMP_InputField searchBookInput;

    private const string COPYAVAILABLE = " Copy available";
    private const string NOCOPYAVAILABLE = " No Copy available";
    private const string YOUHAVETHISBOOK = " You have this book";
    private const string DAYSREMAINING = " Days remaining";
    private const string DAYSPAST = " Days past";
    private const string NEXTRETURNIN = "Next return in ";
    private const string DAYS = " Days";


    private void Awake()
    {
        instance = this;

        LoadLibrary();
    }

    private void Start()
    {
        credentialsManager = CredentialsManager.instance;
        credentialsManager.OnLoginSuccessful += CredentialsManager_OnLoginSuccessful;

        searchBookInput.onValueChanged.AddListener(delegate { SearchBook(); });

    }

    private void CredentialsManager_OnLoginSuccessful(object sender, CredentialsManager.OnLoginSuccessfulEventArgs e)
    {
        currentUser = e.loggedInUser;
    }

    public void LoadLibrary()
    {
        string path = Application.persistentDataPath + "/library.dat";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            books = formatter.Deserialize(stream) as List<Book>;
            stream.Close();
        }
        else
        {
            books = new List<Book>();
        }
    }

    private void SaveLibrary()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/library.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, books);
        stream.Close();
    }

    private void OnApplicationQuit()
    {
        SaveLibrary();
    }

    public void DonateBook()
    {
        Book donatedBook = new()
        {
            Title = donatedBookTitle.text,
            Author = donatedBookAuthor.text,
            ISBN = donatedBookISBN.text
        };

        Book donatedCopy = books.Find(x => x.ISBN == donatedBook.ISBN);

        if (donatedCopy != null)
        {
            donatedCopy.availableAmount++;
            donatedCopy.totalAmount++;
        }
        else
        {
            books.Add(donatedBook);
            donatedBook.availableAmount++;
            donatedBook.totalAmount++;
        }

        
        
    }

    public void ShowAllBooks()
    {
        books = books.OrderBy(x => x.Title).ToList();

        for (int i = 0; i < books.Count; i++)
        {
            for (int j = 0; j < showedAllBooks.Count; j++)
            {
                if (books[i].ISBN == showedAllBooks[j].ISBNText.text)
                {
                    return;
                }
            }

            GameObject instBook = Instantiate(bookUiPrefab, libraryContentParent.transform, false);
            BookUI showedBookUI = instBook.GetComponent<BookUI>();
            showedBookUI.titleText.text = books[i].Title;
            showedBookUI.authorText.text = books[i].Author;
            showedBookUI.ISBNText.text = books[i].ISBN;
            showedBookUI.borrowButton.onClick.AddListener(() => BorrowBook(showedBookUI));
            Book borrowedCopy = currentUser.borrowedBooks.Find(x => x.ISBN == showedBookUI.ISBNText.text);

            if(borrowedCopy != null)
            {
                showedBookUI.nextReturnDaysText.text = YOUHAVETHISBOOK;
                showedBookUI.nextReturnDaysText.gameObject.SetActive(true);
                showedBookUI.borrowButton.gameObject.SetActive(false);

                if (books[i].availableAmount <= 0)
                {
                    showedBookUI.availableAmountText.text = NOCOPYAVAILABLE;
                    showedBookUI.availableAmountText.gameObject.SetActive(true);
                }
                else
                {
                    showedBookUI.availableAmountText.text = books[i].availableAmount.ToString() + COPYAVAILABLE;
                    showedBookUI.availableAmountText.gameObject.SetActive(true);
                }
            }
            else
            {

                if (books[i].availableAmount <= 0)
                {
                    showedBookUI.availableAmountText.text = NOCOPYAVAILABLE;
                    showedBookUI.availableAmountText.gameObject.SetActive(true);
                    showedBookUI.borrowButton.gameObject.SetActive(false);

                    int closestRemainingDay = 15;

                    for (int j = 0; j < credentialsManager.credentials.Count; j++)
                    {
                        Book CopyInAnotherUser = credentialsManager.credentials[j].borrowedBooks.Find(x => x.ISBN == showedBookUI.ISBNText.text);
                        
                        if (CopyInAnotherUser != null)
                        {
                            int remainingDay;

                            remainingDay = (int)(CopyInAnotherUser.dueDate - DateTime.Now).TotalDays;

                            if (remainingDay <= closestRemainingDay)
                            {
                                closestRemainingDay = remainingDay;
                            }
                        } 
                    }

                    showedBookUI.nextReturnDaysText.gameObject.SetActive(true);
                    showedBookUI.nextReturnDaysText.text = NEXTRETURNIN + closestRemainingDay + DAYS;


                }
                else
                {
                    showedBookUI.availableAmountText.text = books[i].availableAmount.ToString() + COPYAVAILABLE;
                    showedBookUI.nextReturnDaysText.gameObject.SetActive(false);
                    showedBookUI.borrowButton.gameObject.SetActive(true);
                }
            }

            showedAllBooks.Add(showedBookUI);
        }
    }

    public void SearchBook()
    {
        string searchText = searchBookInput.text;
        int searchTextLength = searchText.Length;

        int searchIndex = 0;

        for (int i = 0; i < showedAllBooks.Count; i++)
        {
            searchIndex++;

            string showedBookTitle = showedAllBooks[i].titleText.text;
            string showedBookAuthor = showedAllBooks[i].authorText.text;

            if (showedBookTitle.Length >= searchTextLength || showedBookAuthor.Length >= searchTextLength)
            {
                if (searchText.ToLower() == showedBookTitle[..searchTextLength].ToLower()
                || searchText.ToLower() == showedBookAuthor[..searchTextLength].ToLower())
                {
                    showedAllBooks[i].gameObject.SetActive(true);
                }
                else
                {
                    showedAllBooks[i].gameObject.SetActive(false);
                }
            }

        }
    }
    public void BorrowBook(BookUI borrowedBook)
    {
        for (int i = 0; i < books.Count; i++)
        {
            if (borrowedBook.ISBNText.text == books[i].ISBN)
            {
                books[i].borrowedAmount++;
                books[i].borrowedDate = DateTime.Now;
                books[i].dueDate = DateTime.Now.AddDays(15);
                currentUser.borrowedBooks.Add(books[i]);

                borrowedBook.nextReturnDaysText.text = YOUHAVETHISBOOK;
                borrowedBook.nextReturnDaysText.gameObject.SetActive(true);
                borrowedBook.borrowButton.gameObject.SetActive(false);
                books[i].availableAmount--;

                if (books[i].availableAmount <= 0)
                {
                    borrowedBook.availableAmountText.text = NOCOPYAVAILABLE;

                }
                else
                {
                    borrowedBook.availableAmountText.text = books[i].availableAmount.ToString() + COPYAVAILABLE;
                }

            }
        }
    }

    public void ShowReturnableBooks()
    {
        List<Book> borrowedBooks = currentUser.borrowedBooks;

        borrowedBooks = borrowedBooks.OrderBy(x => x.Title).ToList();

        for (int i = 0; i < borrowedBooks.Count; i++)
        {
            for (int j = 0; j < showedBorrowedBooks.Count; j++)
            {
                if (borrowedBooks[i].ISBN == showedBorrowedBooks[j].ISBNText.text)
                {
                    return;
                }
            }

            GameObject instBook = Instantiate(bookUiPrefab, returnableContentParent.transform, false);
            BookUI showedBookUI = instBook.GetComponent<BookUI>();
            showedBookUI.titleText.text = borrowedBooks[i].Title;
            showedBookUI.authorText.text = borrowedBooks[i].Author;
            showedBookUI.ISBNText.text = borrowedBooks[i].ISBN;
            showedBookUI.availableAmountText.gameObject.SetActive(false);
            showedBookUI.remainingDaysText.gameObject.SetActive(true);

            int days = (int)(borrowedBooks[i].dueDate - DateTime.Now).TotalDays;

            if (days >= 0)
            {
                showedBookUI.remainingDaysText.text = days.ToString() + DAYSREMAINING;
            }
            else
            {
                days = -days;
                showedBookUI.remainingDaysText.text = days.ToString() + DAYSPAST;
            }

            showedBookUI.borrowButton.gameObject.SetActive(false);
            showedBookUI.returnButton.gameObject.SetActive(true);
            showedBookUI.returnButton.onClick.AddListener(() => ReturnBook(showedBookUI));
            showedBorrowedBooks.Add(showedBookUI);
        }
    }

    public void ReturnBook(BookUI showedBookUI)
    {
        Book bookToReturn = currentUser.borrowedBooks.Find(x => x.ISBN == showedBookUI.ISBNText.text);

        for (int i = 0; i < books.Count; i++)
        {
            if (bookToReturn.ISBN == books[i].ISBN)
            {
                books[i].borrowedAmount--;
                books[i].availableAmount++;


                currentUser.borrowedBooks.Remove(bookToReturn);

                BookUI returnedBook = showedAllBooks.Find(x => x.ISBNText.text == bookToReturn.ISBN);

                returnedBook.availableAmountText.text = books[i].availableAmount.ToString() + COPYAVAILABLE;
                returnedBook.nextReturnDaysText.gameObject.SetActive(false);
                returnedBook.borrowButton.gameObject.SetActive(true);

                BookUI showedBurrowedBookUI = showedBorrowedBooks.Find(x => x.ISBNText == showedBookUI.ISBNText);

                showedBorrowedBooks.Remove(showedBurrowedBookUI);

                Destroy(showedBookUI.gameObject);


            }
        }

        
    }
}
