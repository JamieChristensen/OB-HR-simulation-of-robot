using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoMain : MonoBehaviour
{
    public ArduinoObject[] arduinoObjects;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Drives forwards on both wheels, assuming H-bridge pins assigned in arduinoObjects 0 through 3. (and forward pins are set to 1023).:
        int leftSensor = arduinoObjects[4].analogRead() /4;
        int rightSensor = arduinoObjects[5].analogRead() /4;

        int leftWrite = (int)ArduinoFunctions.Functions.map(leftSensor, 0, 255, 255, 0);
        int rightWrite = (int)ArduinoFunctions.Functions.map(rightSensor, 0, 255, 255, 0);

        arduinoObjects[0].analogWrite(leftSensor > leftWrite ? leftSensor/2 : 0);
        arduinoObjects[1].analogWrite(leftSensor < leftWrite ? leftWrite/1 : 0);

        arduinoObjects[2].analogWrite(rightSensor > rightWrite ? rightSensor/2: 0);
        arduinoObjects[3].analogWrite(rightSensor <= rightWrite ? rightWrite/1: 0);
    }
}
