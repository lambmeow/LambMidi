using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
public static class MidiWatcher
{
    static MidiDevice[] devices;
    
    /// <summary>
    /// Returns an array of ID of input devices containing a name
    /// </summary>
    public static int[] GetInputDeviceID(string name)
    {
        var result = new int[0];
        for (int i = 0; i < InputDevice.DeviceCount; i++)
        {
            if (InputDevice.GetDeviceCapabilities(i).name.Contains(name))
            {
                var temp = new int[result.Length + 1];
                for (int j = 0; j < result.Length; j++)
                {
                    temp[j] = result[j];
                }
                temp[result.Length] = i;
                result = temp;
            }
            
                
        }

        return result;
    }
    /// <summary>
    /// Returns an ID of the input device with the same name (NAME MUST BE EXACT)
    /// </summary>
    public static int GetOutputDeviceID(string name)
    {
        for (int i = 0; i < OutputDevice.DeviceCount; i++)
        {
            if (name == OutputDevice.GetDeviceCapabilities(i).name)
                return i;
        }

        return -1;
    }
}
