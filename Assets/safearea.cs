using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class safearea : MonoBehaviour
{
    bool hasSpanwed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = Screen.safeArea.ToString();
        if (!hasSpanwed)
        {
            hasSpanwed = true;
            var canvas = GetComponentInParent<Canvas>();
            var x = Instantiate(new GameObject(), canvas.transform);
            x.AddComponent<Image>().color = new Color32(255,255,255,125);
            x.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
            x.transform.SetAsFirstSibling();
        }
    }
}
