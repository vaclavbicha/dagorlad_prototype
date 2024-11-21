using System;
using UnityEngine;

public class BuildingWindowBottomPanel : MonoBehaviour
{
    public BuildingColumn[] BuildingColumns;
    public BuildingColumn[] NormalBuildingColumns;
    public BuildingColumn[] UtilityColumns;

    public enum BottomPanelMode {
        normalPanel,
        utilityPanel
    }

    public void SetupColumns() {
        BuildingColumns = transform.GetComponentsInChildren<BuildingColumn>(true);
        NormalBuildingColumns = Array.FindAll(BuildingColumns, BuildingColumn => BuildingColumn.buildingColumnMode == BuildingColumn.BuildingColumnMode.normal);
        UtilityColumns = Array.FindAll(BuildingColumns, BuildingColumn => BuildingColumn.buildingColumnMode == BuildingColumn.BuildingColumnMode.utility);
    }

}
