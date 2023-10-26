using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUpgrade : MonoBehaviour
{
    public string upgradeName;
    public Sprite Icon;
    public Sprite InfoSprite;
    public Utility.UpgradeTypes type;
    public Amount buildTime;
    public Amount[] cost;

    public Effect effect;
}
