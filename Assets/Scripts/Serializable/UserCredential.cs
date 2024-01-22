using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserCredential
{
    public string UserName;
    public string Password;

    public List<Book> borrowedBooks = new();
    
}
