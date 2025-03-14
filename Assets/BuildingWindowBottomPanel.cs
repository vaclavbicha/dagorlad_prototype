using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindowBottomPanel : MonoBehaviour
{
    public BuildingColumn[] BuildingColumns;
    public BuildingColumn[] NormalBuildingColumns;
    public BuildingColumn[] UtilityColumns;
    public BuildingColumn DemolishColumn;
    public BuildingColumn UpgradeColumn;

    public enum BottomPanelMode {
        normalPanel,
        utilityPanel
    }

    public BottomPanelMode bottomPanelMode;

    public void SetupColumns() {
        foreach(BuildingColumn column in BuildingColumns) {
            column.Deactivate();
        }
    }

    public void SetUtilityColumnDemolishCost(BuildingColumn utilityColumn, Amount[] costs) {
        for (int i = 0; i < utilityColumn.transform.childCount; i++) {
            Transform child = utilityColumn.transform.GetChild(i);
            if (!child.name.Contains("Cost")) return;

            foreach (Button button in child.GetComponentsInChildren<Button>()) {
                bool canAfford = true;
                
                string resourceName = child.name.Replace("Cost_", "");

                if (resourceName == "Gold" || resourceName == "Wood") {
                    Utility.ResourceTypes resourceType = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                    
                    //int owned = Player.Instance.resources.Find(x => x.amount.type == resourceType).amount.value;
                    //Debug.Log("Owned: " + owned);
                    Amount costamount = Array.Find(costs, x => x.type == resourceType);

                    child.GetComponentInChildren<UpdateIconText>().UpdateText(costamount.value.ToString());

                    //if (costamount != null) canAfford = owned >= costamount.value;
                }

                button.interactable = canAfford;
            }
        }
    }

    public void SetUtilityColumnUpdateCost(BuildingColumn utilityColumn, Amount[] costs) {
        //for (int i = 0; i < costs.Length; i++) {
        //    utilityColumn.array[i].UpdateText(costs[i].GetValueText());
        //}
    }

}
