using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public string PlayerName;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
