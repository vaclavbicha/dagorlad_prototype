using UnityEngine;

public class TimeResource : MonoBehaviour {
    [SerializeField]
    Amount amount;

    [SerializeField]
    UpdateIconText display;

    public int GetValue() {
        return amount.value;
    }

    public void SetValue(int value) {
        amount.value = value;
        AmountUpdateWithText();
    }

    public void AmountUpdateWithText() {
        display.UpdateText(amount.GetValueText());
    }
}
