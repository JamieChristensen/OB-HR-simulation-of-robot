using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoMain : MonoBehaviour
{
    public ArduinoObject[] arduinoObjects;

    

    // Start is called before the first frame update
    void Start()
    {
        //Drives forwards on both wheels, assuming H-bridge pins assigned in arduinoObjects 0 through 3. (and forward pins are set to 1023).:
        arduinoObjects[0].analogWrite(1023);
        arduinoObjects[1].analogWrite(0);
        arduinoObjects[2].analogWrite(1023);
        arduinoObjects[3].analogWrite(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
