using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLinePausePlay : MonoBehaviour
{
    PlayableDirector playableDirector;
    public bool pause;
    // Start is called before the first frame update
    void Start()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    public void ListenForNextAction()
    {
        pause = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            pause = false;
            playableDirector.Play();
        }
        //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
    }
}
