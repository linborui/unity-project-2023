using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    public List<ItemDescription> ItemList;
}

[System.Serializable]
public class ItemDescription
{
    public string ItemName;
    public Image ItemImage;
    public string[] ItemDescriptionLines = new string[4];
}


