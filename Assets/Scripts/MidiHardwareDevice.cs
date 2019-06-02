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
        [SerializeField] bool _startOnAwake;
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
#if UNITY_EDITOR
                    Debug.Log(string.Format("No output devices found under the name {0}, ignoring input values for this device.", _deviceName));
#endif
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
                Debug.Log(string.Format("input devices on {0} disabled, ignoring.", _deviceName));
#endif
            }
            if (HasOutputValues)
            {
                var id = MidiWatcher.GetOutputDeviceID(_deviceName);
                //nothing
                if (id.Length == 0)
                {
#if UNITY_EDITOR
                    Debug.Log(string.Format("No output devices found under the name {0}, ignoring input values for this device.", _deviceName));
#endif
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
                Debug.Log(string.Format("output devices on {0} disabled, ignoring.", _deviceName));
#endif

            }
            OnActivate();
            _active = true;
        }
        public void Deactivate()
        {
            if (!_active)
                return;
            if (HasInputValues)
            {
                //Better safe than sorry!
                for (int i = 0; i < _iDevices.Length; i++)
                {
                    _iDevices[i].Deactivate();
                    _iDevices[i].Dispose();
                    _iDevices[i] = null;
                }
            }
            if (HasOutputValues)
            {
                for (int i = 0; i < _oDevices.Length; i++)
                {
                    _oDevices[i].Dispose();
                    _oDevices[i] = null;
                }
            }
            
            _iDevices = null;
            _oDevices = null;
            _inputIDs = null;
            _outputIDs = null;
            _active = false;

        }
       
        
        void Awake()
        {
            if (_startOnAwake) Activate();
        }
        void OnDisable()
        {
            Deactivate();
        }
        
        #endregion

    }

    
}

