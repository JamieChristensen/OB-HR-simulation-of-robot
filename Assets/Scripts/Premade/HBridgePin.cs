using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HBridgePin : ArduinoObject
{
    public enum PinType
    {
        DriveForward, DriveBackward
    };

    [SerializeField]
    private PinType pinType;

    [SerializeField]
    private HBridge hBridge; //assign in inspector - otherwise it finds it through its parent.

    void Start()
    {
        if (hBridge == null)
        {
            hBridge = transform.GetComponentInParent<HBridge>();
        }
    }

    override public int analogRead()
    {
        throw new NotImplementedException();
    }
    override public void analogWrite(int value)
    {
        if (value < 0 || value > 1023)
        {
            Debug.LogWarning("analogWrite used with values outside range of [0;1023], value was clamped within this range.");
        }
        int val = Mathf.Clamp(value, 0, 1023);
        hBridge.SetMotorSpeedAndDirection(pinType, val);
    } //from 0 to 1023
    override public bool digitalRead()
    {
        throw new NotImplementedException();
    }
    override public void digitalWrite(bool isHigh)
    {
        throw new NotImplementedException();
    }


}
