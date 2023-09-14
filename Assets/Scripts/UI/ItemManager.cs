using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void SelectLocation(string arg)
    {
        UIManager.Instance.OnSelectLocation(arg);
    }
}
