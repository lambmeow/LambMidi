using Sanford.Multimedia.Midi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lambmeow.Midi
{
    public class MidiHardwareDevice : MonoBehaviour
    {
        #region Variables

        #region Input
        [SerializeField] bool _hasInputValues;
        int[] _inputIDs;
        MidiHandle[] _iDevices;
        protected int _iCurrentID;
        
        #endregion

        #region Output
        [SerializeField] bool _hasOutputValues;
        int[] _outputIDs;
        OutputDevice[] _oDevices;
        //[SerializeField] int _minValue, _maxValue;
        protected int _oCurrentID;
        #endregion

        #region Other
        bool _active = false;
        [SerializeField] string _deviceName;
        #endregion
        #endregion

        #region Accessors
        

        #region Input
        /// <summary>
        /// All input device IDs used by this MidiDevice
        /// </summary>
        public int[] InputIDs { get => _inputIDs; }
        /// <summary>
        /// Returns true if MidiDevice stores any input device tags/IDs
        /// </summary>
        public bool HasInputValues { get => _hasInputValues; }

        #endregion

        #region Output
        /// <summary>
        /// All output device ids used by the MidiDevice
        /// </summary>
        public int[] OutputIDs { get => _outputIDs; }
        /// <summary>
        /// returns true if MidiDevice stores any output device names/IDs
        /// </summary>
        public bool HasOutputValues { get => _hasOutputValues; }


        #endregion

        #region Other
        /// <summary>
        /// Returns true if either Input or output values are being used by the devices specified
        /// </summary>
        public bool IsActive { get => _active; }
        /// <summary>
        /// The name of the MidiDevice 
        /// </summary>
        public string DeviceName { get => _deviceName; }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Executed when this MidiDevice activates (usually when MidiWatcher activates) 
        /// </summary>
        protected virtual void OnActivate() { }

        /// <summary>
        /// Executed when this MidiDevice deactivates (MidiWatcher Deactivates, Hardware gets disconnected)
        /// </summary>
        protected virtual void OnDeactivate() { }

        /// <summary>
        /// Executed when a midi device gets pressed (input only)
        /// </summary>
        protected virtual void OnMidiPress(ChannelMessageEventArgs e) { }

        /// <summary>
        /// Executed when 
        /// </summary>
        protected virtual void OnSysexMessage(SysExMessageEventArgs e) { }

        /// <summary>
        /// Executed when a message is sent to a midi device (output only)
        /// </summary>
        protected virtual void OnMessageSent() { }

        public void Activate()
        {
            if (_active)
                return;
            if (HasInputValues)
            {
                var id = MidiWatcher.GetInputDeviceID(_deviceName);
                //nothing
                if (id.Length == 0)
                {
                    throw new System.Exception("A name in a MidiDevice does not contain input id values or is not typed correctly");
                }
                _inputIDs = id;
                _iDevices = new MidiHandle[id.Length];
                for (int i = 0; i < _inputIDs.Length; i++)
                {
                    _iDevices[i] = new MidiHandle(_inputIDs[i]);
                   
                    _iDevices[i].Activate();
                    
                }
                _iCurrentID = 0;
                
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("no input device values found or disabled, ignoring.");
#endif
            }
            if (HasOutputValues)
            {
                var id = MidiWatcher.GetOutputDeviceID(_deviceName);
                //nothing
                if (id.Length == 0)
                {
                    throw new System.Exception("the name in a MidiDevice does not contain output id values or is not typed correctly");
                }
                _outputIDs = id;
                _oDevices = new OutputDevice[_outputIDs.Length];
                for (int i = 0; i < _inputIDs.Length; i++)
                {
                    _oDevices[i] = new OutputDevice(_inputIDs[i]);
                }
                _oCurrentID = 0;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("no output devices found or disabled, ignoring.");
#endif
            }
            OnActivate();
            _active = true;
        }
        /// <summary>
        /// Returns a new MidiDevice (only used for creating )
        /// </summary>
        /// <param name="id">hardware device id</param>
        /// <param name="device">true for input, false for output</param>
        /// <returns></returns>
        public static MidiHardwareDevice CreateMidiDevice(int id, bool device)
        {
            return null;
        }

        public void Deactivate()
        {
            if (!_active)
                return;
            //Better safe than sorry!
            for (int i = 0; i < _iDevices.Length; i++)
            {
                _iDevices[i].Deactivate();
                _iDevices[i].Dispose();
                _iDevices[i] = null;
            }
            for (int i = 0; i < _oDevices.Length; i++)
            {
                _oDevices[i].Dispose();
                _oDevices[i] = null;
            }
            
            _iDevices = null;
            _oDevices = null;
            _inputIDs = null;
            _outputIDs = null;
            _active = false;

        }
        #endregion
    }
    public class MidiHandle : IDisposable
    {
        InputDevice _device;
        MidiNote[] _activeNotes, _activeControl;

        public bool isActive { get; private set; }

        public MidiHandle(int id)
        {
            _device = new InputDevice(id);
        }

        ~MidiHandle()
        {
            Dispose();
        }
        public void Dispose()
        {
            Deactivate();
            _device.Dispose();
        }
        public void Activate()
        {
            _device.ChannelMessageReceived += _device_ChannelMessageReceived;
            _device.StartRecording();
            isActive = true;
        }

        private void _device_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            
            switch (e.Message.Command)
            {
                //regular note
                case ChannelCommand.NoteOn:

                    var index = FindNote(_activeNotes, e.Message.Data1);
                    if ( index == -1 && e.Message.Data2 != 0)
                    {
                        var note = new MidiNote(e.Message.Data1, e.Message.Data2, e.Message.Command);
                        AddToList(_activeNotes, note);
                        _activeNotes[_activeNotes.Length - 1].First = true;
                        break;
                    }
                    if (e.Message.Data2 == 0)
                        _activeNotes[index].Last = true;
                    break;
            }
        }

        public void Deactivate()
        {
            _device.StopRecording();
            isActive = false;
        }
        
        static MidiNote[] AddToList(MidiNote[] list,MidiNote entry)
        {
            var res = new MidiNote[list.Length + 1];
            for (int i = 0; i < list.Length; i++)
            {
                res[i] = list[i];
            }
            res[list.Length] = entry;
            return res;

           
        }
        static int FindNote(MidiNote[] list, int id)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].ID == id)
                    return i;
            }
            return -1;
        }
    }
}

