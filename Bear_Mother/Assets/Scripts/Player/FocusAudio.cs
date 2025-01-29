using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusAudio : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter _emitter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _emitter.EventInstance.setParameterByName("Focus", 1);
        } 
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _emitter.EventInstance.setParameterByName("Focus", 0);
        }
    }
}
