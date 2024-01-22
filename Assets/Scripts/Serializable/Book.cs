using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Book
{
    public string Title;
    public string Author;
    public string ISBN;
    public int totalAmount;
    public int borrowedAmount;
    public int availableAmount;
    
    public DateTime borrowedDate;
    public DateTime dueDate;
}
