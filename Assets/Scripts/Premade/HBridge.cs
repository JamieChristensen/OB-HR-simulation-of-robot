using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HBridge : ArduinoObject
{
    override public int analogRead()
    {
        throw new NotImplementedException();
    }
    override public void analogWrite(int value)
    {
        throw new NotImplementedException();
    }
    override public bool digitalRead()
    {
        throw new NotImplementedException();
    }
    override public void digitalWrite(bool isHigh)
    {
        throw new NotImplementedException();
    }
}
