using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColumn : MonoBehaviour
{
    public UpdateIconText[] array;
    public string currentBuildingName = "default";
    public void OnBuy()
    {
        UIManager.Instance.OnBuyBuilding(currentBuildingName);
    }
}
