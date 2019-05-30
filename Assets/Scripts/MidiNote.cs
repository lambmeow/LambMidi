using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;
namespace Lambmeow.Midi
{
    public struct MidiMessage
    {
        public int Data1, Data2;
        public ChannelCommand MessageType;
        public bool FirstTime,LastTime;
        
        public MidiMessage(int data1, int data2, ChannelCommand message, bool firsttime,bool lasttime)
        {
            Data1 = data1;
            Data2 = data2;
            MessageType = message;
            FirstTime = firsttime;
            LastTime = lasttime;
        }
    }
    public class MidiNote
    {
        int _ID;
        public bool Last, First;
        int _value;
        public ChannelCommand Command;
        public int ID { get => _ID; }
        public int Value { get => _value; }
        
        public MidiNote(int id, int value,ChannelCommand command)
        {
            _ID = id;
            _value = value;
            Command = command;
        }
    }
}