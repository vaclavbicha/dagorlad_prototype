using Microsoft.Unity.VisualStudio.Editor;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnitStruct : MonoBehaviour {
    public Utility.UnitTypes UnitType;
    public Utility.UnitClass UnitClass;

    public Sprite UnitSprite;
    public Sprite IconSprite;
    public Sprite InfoSprite;

    public Amount SuppplyCost;
    public Amount GoldCost;
    public Amount WoodCost;
    public Amount TimeCost;

    override public bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }
        UnitStruct other = (UnitStruct)obj;
        
        return (UnitType == other.UnitType
             && UnitClass == other.UnitClass
             && UnitSprite == other.UnitSprite
             && IconSprite == other.IconSprite
             );
    }
}

