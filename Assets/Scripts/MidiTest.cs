using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
public class MidiTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(InputDevice.GetDeviceCapabilities(0).name);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
