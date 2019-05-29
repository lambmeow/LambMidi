using Sanford.Multimedia.Midi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiDevice : MonoBehaviour
{
    #region Variables

    #region Input
    [SerializeField] bool _hasInputValues;
    int[] _inputIDs;
    InputDevice[] _iDevices;
    [SerializeField] string[] _iNames;
    protected int _iCurrentID;
    #endregion

    #region Output
    [SerializeField] bool _hasOutputValues;
    int[] _outputIDs;
    [SerializeField] string[] _oNames;
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
    /// Returns true if MidiDevice stores any input device names/IDs
    /// </summary>
    public bool HasInputValues { get => _hasInputValues; }
    public string[] InputNames { get => _iNames; }
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
    /// <summary>
    /// All the output device names used by the MidiDevice
    /// </summary>
    public string[] OutputNames { get => _oNames; }
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
        if (HasInputValues)
        {
            _inputIDs = new int[_iNames.Length];

            for (int i = 0; i < _iNames.Length; i++)
            {
                var id = MidiWatcher.GetInputDeviceID(_iNames[i]);
                //nothing
                if (id.Length == 0)
                {
                    throw new System.Exception("A name in a MidiDevice does not exist or is not typed correctly");
                }
                _inputIDs = id;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("no input device values found or disabled, ignoring.");
#endif
        }
        if (HasOutputValues)
        {

        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("no output devices found or disabled, ignoring.");
#endif
        }
    }


    
    #endregion
}
