using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ArduinoMain : MonoBehaviour
{
    //These references ensure arduino-like syntax, and rely on the proper components being assigned to this script in the inspector.
    #region PremadeReferences
    public ArduinoObject[] arduinoObjects;

    #endregion PremadeReferences

    //On included/premade Arduino functions:
    //delay(timeInMilliseconds) : use "yield return delay(timeInMilliseconds)", to get similar functionality as delay() in arduino would give you.

    //map() : works exactly as on Arduino, maps a long from one range to another. 
    //If you want to get an int or a float from the map()-function, you can cast the output like this: (int)map(s, a1, a2, b1, b2) or (float)map(s, a1, a2, b1, b2) 

    //millis() : returns the time as a ulong since the start of the scene (and therefore also the time since setup() was run) in milliseconds.

    //If you want to do something similar to serial.println(), use Debug.Log(). 

    IEnumerator setup()
    {





        //following region ensures delay-functionality for setup(). Do not delete, must always be last thing in setup.
        #region PremadeSetup
        yield return null;
        #endregion PremadeSetup
    }

    IEnumerator loop()
    {
        //Drives forwards on both wheels, assuming H-bridge pins assigned in arduinoObjects 0 through 3. (and forward pins are set to 1023).:
        
        int leftSensor = arduinoObjects[4].analogRead() / 4;
        int rightSensor = arduinoObjects[5].analogRead() / 4;
        int leftWrite = (int)ArduinoFunctions.Functions.map(leftSensor, 0, 255, 255, 0);
        int rightWrite = (int)ArduinoFunctions.Functions.map(rightSensor, 0, 255, 255, 0);
        arduinoObjects[0].analogWrite((int)(leftSensor > leftWrite ? leftSensor / 2 : 0));
        arduinoObjects[1].analogWrite(leftSensor <= leftWrite ? leftWrite / 1 : 0);
        arduinoObjects[2].analogWrite((int)(rightSensor > rightWrite ? rightSensor / 2 : 0));
        arduinoObjects[3].analogWrite(rightSensor <= rightWrite ? rightWrite / 1 : 0);
        






        //Following region is implemented as to allow "yield return delay()" to function the same way as one would expect it to on Arduino.
        //It should always be at the end of the loop()-function, and shouldn't be edited.
        #region DoNotDelete
        //Wait for one frame
        yield return new WaitForSeconds(0);
        //New loop():
        yield return loop();
        #endregion DoNotDelete 
    }



    #region PremadeDefinitions
    void Start()
    {
        Time.fixedDeltaTime = 0.005f; //4x physics steps of what unity normally does - to improve sensor-performance.
        StartCoroutine(setup());
        StartCoroutine(loop());
    }

    IEnumerator delay(int _durationInMilliseconds)
    {
        float durationInSeconds = ((float)_durationInMilliseconds * 0.001f);
        yield return new WaitForSeconds(durationInSeconds);
    }

    public long map(long s, long a1, long a2, long b1, long b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public ulong millis()
    {
        return (ulong)(Time.timeSinceLevelLoad * 1000f);
    }

    #endregion PremadeDefinitions
}