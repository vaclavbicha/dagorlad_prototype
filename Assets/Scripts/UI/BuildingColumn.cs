using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColumn : MonoBehaviour
{
    public UpdateIconText[] array;
    public void OnBuy(string arg)
    {
        UIManager.Instance.OnBuyBuilding(arg);
    }
}
