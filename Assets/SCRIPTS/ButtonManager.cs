using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    GameObject last = null;
    public GameObject panel_defence;
    public GameObject panel_attack;
    public GameObject panel_resource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void defense()
    {
        Debug.Log("defense");
        panel_defence.SetActive(true);
        panel_attack.SetActive(false);
        panel_resource.SetActive(false);
        moveButton();
    }

    public void attack()
    {
        Debug.Log("attack");
        panel_attack.SetActive(true);
        panel_defence.SetActive(false);
        panel_resource.SetActive(false);
        moveButton();
    }

    public void resource()
    {
        Debug.Log("resource");
        panel_resource.SetActive(true);
        panel_attack.SetActive(false);
        panel_defence.SetActive(false);
        moveButton();
    }

    void moveButton()
    {
        if (last != null) last.GetComponent<RectTransform>().anchoredPosition += Vector2.left * 75;
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition += Vector2.right * 75;
        last = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
