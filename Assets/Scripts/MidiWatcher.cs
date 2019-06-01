using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

namespace Lambmeow.Midi
{
    public class MidiWatcher {
        MidiHardwareDevice[] devices;
        static MidiWatcher _instance;
        public static MidiWatcher Instance { get { if (_instance == null) _instance = new MidiWatcher(); return _instance; } }
        public bool Active { get; private set; }
        /// <summary>
        /// Returns an array of ID of input devices containing a tag
        /// </summary>
        public static int[] GetInputDeviceID(string tags)
        {
            var result = new int[0];
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                if (InputDevice.GetDeviceCapabilities(i).name.Contains(tags))
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
        public static void Activate()
        {
            
            Instance.devices = GameObject.FindObjectsOfType<MidiHardwareDevice>();
            if (Instance.Active || Instance.devices.Length == 0)
                return;
            foreach (var devices in Instance.devices)
            {
                devices.Activate();
            }
            Instance.Active = true;
        }

        public static void Deactivate()
        {
            if (!Instance.Active)
                return;
            foreach (var dev in Instance.devices)
            {
                dev.Deactivate();
            }
            Instance.Active = false;
        }
        /// <summary>
        /// Returns an ID of the input device with the same name (NAME MUST BE EXACT)
        /// </summary>
        public static int[] GetOutputDeviceID(string tags)
        {
            var result = new int[0];
            for (int i = 0; i < OutputDevice.DeviceCount; i++)
            {
                if (OutputDevice.GetDeviceCapabilities(i).name.Contains(tags))
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
    }
}

