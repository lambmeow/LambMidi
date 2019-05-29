using Sanford.Multimedia.Midi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lambmeow.Midi
{
    public class MidiDevice : MonoBehaviour
    {
        #region Variables

        #region Input
        [SerializeField] bool _hasInputValues;
        int[] _inputIDs;
        InputDevice[] _iDevices;
        protected int _iCurrentID;
        #endregion

        #region Output
        [SerializeField] bool _hasOutputValues;
        int[] _outputIDs;
        OutputDevice[] _oDevices;
        [SerializeField] int _minValue, _maxValue;
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
        protected virtual void OnMidiPress() { }

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
                _iDevices = new InputDevice[_inputIDs.Length];
                for (int i = 0; i < _inputIDs.Length; i++)
                {
                    _iDevices[i] = new InputDevice(_inputIDs[i]);
                    _iDevices[i].ChannelMessageReceived += InputMessageReceived;
                    _iDevices[i].StartRecording();
                    
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

        private void InputMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            //TODO: add a struct to send data of the message(including the MidiDevice info like the InputDevice id and name of MidiDevice)
        }

        public void Deactivate()
        {
            if (!_active)
                return;
            //Better safe than sorry!
            for (int i = 0; i < _iDevices.Length; i++)
            {
                _iDevices[i].StopRecording();
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

        }
        #endregion
    }
}
