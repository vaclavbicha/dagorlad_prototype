using UnityEngine;

public class SupplyResource : MonoBehaviour {
    [SerializeField]
    int maxAmount;

    [SerializeField]
    int currentAmount;

    [SerializeField]
    UpdateIconText display;

    public delegate void UpdatedEventDelegate(int value);
    public event UpdatedEventDelegate On_AmountUpdate;

    public void SetStartingValue(int maxValue) {
        maxAmount = maxValue;
        currentAmount = 0;
        AmountUpdateWithText();
    }

    public int GetCurrentValue() {
        return currentAmount;
    }

    public int GetMaxValue() {
        return maxAmount;
    }

    public void SetCurrentValue(int value) {
        currentAmount = value;
        On_AmountUpdate?.Invoke(value);
        AmountUpdateWithText();
    }

    public void SetMaxValue(int value) {
        maxAmount = value;
        On_AmountUpdate?.Invoke(value);
        AmountUpdateWithText();
    }

    public void AddCurrentValue(int value) {
        SetCurrentValue(currentAmount + value);
    }

    public void DeductCurrentValue(int value) {
        SetCurrentValue(currentAmount - value);
    }

    public void AddMaxValue(int value) {
        SetMaxValue(maxAmount + value);
    }

    public void DeductMaxValue(int value) {
        SetMaxValue(maxAmount - value);
    }

    public void AmountUpdateWithText() {
        display.UpdateText(currentAmount.ToString() + "/" + maxAmount);
    }
}
