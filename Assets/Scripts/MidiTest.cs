using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
public class MidiTest : MonoBehaviour
{
    InputDevice device;
    // Start is called before the first frame update
    void Start()
    {
        device = new InputDevice(1);
        device.ChannelMessageReceived += Device_ChannelMessageReceived;
        device.StartRecording();
        device.SysExMessageReceived += Device_SysExMessageReceived;
        print(InputDevice.GetDeviceCapabilities(0).name);   
    }

    private void Device_SysExMessageReceived(object sender, SysExMessageEventArgs e)
    {
        print(e.Message.GetBytes()[7]);
    }

    private void Device_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
    {
        
       print(e.Message.Command + " " + e.Message.Data1 + " " + e.Message.Data2);
    }
    private void OnDisable()
    {
        device.Dispose();
    }
    // Update is called once per frame

}
