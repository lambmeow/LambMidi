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
        public int FirstTime,LastTime;
        
        public MidiMessage(int data1, int data2, ChannelCommand message, int firsttime,int lasttime)
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
        public int Last, First;
        public int Value;
        public ChannelCommand Command;
        public int ID { get => _ID; }
        
        
        public MidiNote(int id, int value,ChannelCommand command)
        {
            _ID = id;
            Value = value;
            Command = command;
        }
        public MidiMessage getData() => new MidiMessage(_ID,Value,Command,First,Last);
        
    }
}