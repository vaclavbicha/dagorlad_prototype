using System.Collections;
using UnityEngine;

public class Resource : MonoBehaviour {
    [SerializeField]
    Amount amount;
    public Sprite icon;
    public int currentProduction;

    public UpdateIconText display;

    public delegate void UpdatedEventDelegate(int value);
    public event UpdatedEventDelegate On_AmountUpdate;

    public void SetStartingValue(int value) {
        amount.value = value;
        AmountUpdateWithText();
    }

    public int GetValue() {
        return amount.value;
    }

    public void SetValue(int value) {
        amount.value = value;
        AmountUpdateWithText();
        On_AmountUpdate?.Invoke(value);
    }

    public void AddValue(int value) {
        SetValue(amount.value + value);
    }

    public void DeductValue(int value) {
        SetValue(amount.value - value);
    }

    public void SetCurrentProduction(int value) {
        currentProduction = value;

        if (currentProduction > 0) {
            StartCoroutine(DistributeResource());
        } else {
            StopCoroutine(DistributeResource());
        }
    }

    public void AddCurrentProduction(int value) {
        SetCurrentProduction(currentProduction + value);
    }

    public void DeductCurrentProduction(int value) {
        SetCurrentProduction(currentProduction - value);
    }

    IEnumerator DistributeResource() {
        float initialSceneStartTime = ResourceManager.Instance.GetGameTime();
        yield return new WaitUntil(() => ResourceManager.Instance.GetGameTime() != initialSceneStartTime);

        AddValue(currentProduction);
        StartCoroutine(DistributeResource());
    }

    public void AmountUpdateWithText()
    {
        if (amount.type == Utility.ResourceTypes.Supply) {
            display.UpdateText(ResourceManager.Instance.currentSupply.ToString() + "/" + amount.value);
        } else if (amount.type == Utility.ResourceTypes.Gold || amount.type == Utility.ResourceTypes.Wood) {
            display.UpdateText(amount.value.ToString() + "<color=#C7D3EF>" + "+" + currentProduction.ToString() + "</color>");
        }
    }
}
