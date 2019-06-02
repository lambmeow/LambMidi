using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sanford.Multimedia.Midi;

namespace Lambmeow.Midi
{
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
            _activeNotes = new MidiNote[0];
            _activeControl = new MidiNote[0];
            isActive = true;
        }

        private void _device_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            switch (e.Message.Command)
            {
                //regular note
                case ChannelCommand.NoteOn:

                    var index = FindNote(_activeNotes, e.Message.Data1);

                    if (index == -1 && e.Message.Data2 != 0)
                    {
                        var note = new MidiNote(e.Message.Data1, e.Message.Data2, e.Message.Command);
                        _activeNotes = AddToList(_activeNotes, note);
                        _activeNotes[_activeNotes.Length - 1].First = true;
                        break;
                    }
                    if (e.Message.Data2 == 0)
                        _activeNotes[index].Last = true;
                    break;
                //use existing note id and change it to the new value
                /*Note: Launchpad pro(and other types of polypressure supported devices) has a special poly pressure mode that is more sensitive, 
                DO NOT CHOOSE THAT OPTION! it uses data 1 for pressure values instead of data 2 and completely fucks up the program structure. 
                Use at your own risk.*/
                case ChannelCommand.PolyPressure:
                    var ind = FindNote(_activeNotes, e.Message.Data1);
                    //Something went wrong and needs to create a new midinote entry even though poly pressure values are preceded by NoteOn values
                    //this should never be executed but you never know!
                    if (ind == -1 && e.Message.Data2 != 0)
                    {
                        var note = new MidiNote(e.Message.Data1, e.Message.Data2, e.Message.Command);
                        _activeNotes = AddToList(_activeNotes, note);
                        _activeNotes[_activeNotes.Length - 1].First = true;
                        break;
                    }
                    _activeNotes[ind].Value = e.Message.Data2;
                    break;
                //used for sliders, knobs, and, in some cases, notes
                case ChannelCommand.Controller:
                    var place = FindNote(_activeControl, e.Message.Data1);
                    if (place == -1)
                    {
                        var note = new MidiNote(e.Message.Data1, e.Message.Data2, e.Message.Command);
                        _activeControl = AddToList(_activeControl, note);
                        _activeControl[_activeControl.Length - 1].First = true;
                        break;
                    }
                    _activeControl[place].Value = e.Message.Data2;
                    break;

            }
        }
        public void Deactivate()
        {
            _device.StopRecording();
            isActive = false;
        }
        public bool GetMidiButton(int noteID)
        {
            for (int i = 0; i < _activeNotes.Length; i++)
            {
                if (_activeNotes[i].ID == noteID)
                    return true;
            }
            return false;
        }
        public int GetMidiValue(int noteID)
        {
            for (int i = 0; i < _activeNotes.Length; i++)
            {
                if (_activeNotes[i].ID == noteID)
                    return _activeNotes[i].Value;

            }
            return 0;
        }
        public MidiMessage GetMidiData(int noteID)
        {
            for (int i = 0; i < _activeNotes.Length; i++)
            {
                if (_activeNotes[i].ID == noteID)
                {
                    return _activeNotes[i].getData();
                }
            }
            return new MidiMessage();
        }
        static MidiNote[] AddToList(MidiNote[] list, MidiNote entry)
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
