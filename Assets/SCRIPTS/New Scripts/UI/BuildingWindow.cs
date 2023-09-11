using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWindow : MonoBehaviour
{
    public void ActivateWindow(int location_id, string window_type)
    {
        gameObject.SetActive(true);
    }
}
