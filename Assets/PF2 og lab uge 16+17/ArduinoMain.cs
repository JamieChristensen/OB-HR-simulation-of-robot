using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ArduinoMain : MonoBehaviour
{
    public Breadboard breadboard;
    public Servo servo;
    //On included/premade Arduino functions:
    //delay(timeInMilliseconds) : use "yield return delay(timeInMilliseconds)", to get similar functionality as delay() in arduino would give you.

    //map() : works exactly as on Arduino, maps a long from one range to another. 
    //If you want to get an int or a float from the map()-function, you can cast the output like this: (int)map(s, a1, a2, b1, b2) or (float)map(s, a1, a2, b1, b2) 

    //millis() : returns the time as a ulong since the start of the scene (and therefore also the time since setup() was run) in milliseconds.

    //If you want to do something similar to serial.println(), use Debug.Log(). 

    //analogWrite() and analogRead() works as they do in arduino - remember to give them correct input-values.
    //digitalRead() and digitalWrite() writes and returns bools. (High = true). 
    //LineSensors have both write-functions implemented, motors/hbridge have both read-functions implemented.
    //The console will display a "NotImplementedException" if you attempt to write to sensors or read from motors. 


    //Additions from 21-04-2020:

    //Distance sensor:
    //The Distance (ultrasonic) sensor is added, if you use "pulseIn()" on the pin it is assigned to, 
    //it will return the time it took sound to travel double the distance to the point of impact in microseconds (type: ulong).
    //This mimics roughly how the HC-SR04 sensor works. 
    //There is no max-range of the distance-sensor. If it doesn't hit anything, it returns a 0.

    //Servo:
    //if you add the servo-prefab to the scene, ArduinoMain will automatically find the servo object, essentially handling "servo.attach()" automatically. 
    //There can be only one servo controlled by this script.
    //servo.write() and servo.read() implemented, they function similar to a servomotor. 
    //The angles that servo.write() can handle are [0:179]. All inputs outside of this range, are clamped within the range.
    //servo.read() will return the last angle written to the servo-arm. 
    //In order to attach something to the servo, so that it rotates with the servo-arm, simply make the object you wish to rotate, a child of either: Servo-rotationCenter or Servo-arm. 
    //Make sure to take into account the position of the object relative to Servo-rotationCenter. The rotated object will rotate positively around the Y-axis (up) of the Servo-rotationCenter gameobject.




    IEnumerator setup()
    {
        //Your code goes here:

        //Example of delay:
        Debug.Log("pre-delay log");
        yield return delay(20); //2 second delay
        //Your code ends here -----

        //following region ensures delay-functionality for setup() & loop(). Do not delete, must always be last thing in setup.
        #region PremadeSetup
        yield return StartCoroutine(loop()); ;
        #endregion PremadeSetup
    }

    float servoAngle;
    ulong currentMillis;

    bool gotToPillar = false;
    bool passedPillar = false;

    int forwardDriveLeftPin = 0;
    int backwardDriveLeftPin = 1;
    int forwardDriveRightPin = 2;
    int backwardDriveRightPin = 3;

    public bool reachedEndOfLine = false;
    private bool turnedLeft;

    IEnumerator loop()
    {
        ulong time;
        float distance;
        int leftSensor = analogRead(4) / 4;
        int rightSensor = analogRead(5) / 4;

        //Delay for the sake of being able to detect the end of the line
        if (passedPillar)
        {
            yield return delay(10);
        }

        //If the end of the line is detected:
        if (analogRead(4) < 300 && analogRead(5) < 300)
        {
            reachedEndOfLine = true;
            analogWrite(0, 0);
            analogWrite(1, 0);
            analogWrite(2, 0);
            analogWrite(3, 0);
            yield return delay(500);

            //Until robot completes the level.:
            while (true)
            {
                int speed = 100;
                analogWrite(forwardDriveLeftPin, speed);
                analogWrite(forwardDriveRightPin, speed);
                time = pulseIn(6); //distanceSensor.
                distance = (time * 0.034f * 0.5f);

                if (distance < 14 && distance != 0)
                {
                    if (!turnedLeft)
                    {
                        //Rotate 90 approx. 90 degrees.
                        analogWrite(forwardDriveLeftPin, 0);
                        analogWrite(backwardDriveLeftPin, 20);
                        analogWrite(forwardDriveRightPin, 20);
                        turnedLeft = true;
                        yield return delay(2500);
                        analogWrite(backwardDriveLeftPin, 0);
                        analogWrite(backwardDriveRightPin, 0);
                        analogWrite(forwardDriveLeftPin, speed);
                        analogWrite(forwardDriveRightPin, speed);
                    }
                    else
                    {
                        analogWrite(forwardDriveRightPin, 0);
                        analogWrite(backwardDriveRightPin, 20);
                        analogWrite(forwardDriveLeftPin, 20);
                        yield return delay(2600);
                        analogWrite(backwardDriveLeftPin, 0);
                        analogWrite(backwardDriveRightPin, 0);
                        analogWrite(forwardDriveRightPin, speed);
                        analogWrite(forwardDriveLeftPin, speed);

                    }
                }
                yield return delay(1);
            }
        }

        //This is the line-followingbehaviour that the robot does both before and after passing the pillar.
        if (!reachedEndOfLine)
        {
            int leftWrite = (int)ArduinoFunctions.Functions.map(leftSensor, 0, 255, 255, 0);
            int rightWrite = (int)ArduinoFunctions.Functions.map(rightSensor, 0, 255, 255, 0);
            analogWrite(0, (int)(leftSensor > leftWrite ? leftSensor / 4 : 0));
            analogWrite(1, leftSensor <= leftWrite ? leftWrite / 2 : 0);
            analogWrite(2, (int)(rightSensor > rightWrite ? rightSensor / 4 : 0));
            analogWrite(3, rightSensor <= rightWrite ? rightWrite / 2 : 0);
        }

        if (!passedPillar)
        {
            servoAngle = 45;
            servo.write((int)servoAngle);
        }
        else
        {
            servo.write(90); //Position the distance-sensor/ultrasonic sensor for detecting walls right in front of the robot.
        }


        time = pulseIn(6); //Value out of ultrasonic sensor.
        distance = (time * 0.034f * 0.5f); //Time to distance conversion.

        //Lengthy sequence of turns and driving to get around the pillar:
        if (distance != 0 && distance < 14 && !gotToPillar)
        {
            //Prepare motorvalues for driving around the pillar:
            gotToPillar = true;


            //Set the backwards direction PWN-signal on H-bridge to 0.
            analogWrite(1, 0);
            analogWrite(3, 0);
            analogWrite(forwardDriveLeftPin, 40);
            analogWrite(forwardDriveRightPin, 20);
            yield return delay(1500);

            analogWrite(forwardDriveLeftPin, 30);
            analogWrite(forwardDriveRightPin, 30);
            yield return delay(1500);

            //Rotate 90 approx. 90 degrees.
            analogWrite(forwardDriveLeftPin, 0);
            analogWrite(backwardDriveLeftPin, 20);
            analogWrite(forwardDriveRightPin, 20);
            yield return delay(2600);

            analogWrite(backwardDriveLeftPin, 0);
            analogWrite(forwardDriveLeftPin, 90);
            analogWrite(forwardDriveRightPin, 90);
            yield return delay(1700);

            //Rotate 
            analogWrite(forwardDriveLeftPin, 0);
            analogWrite(backwardDriveLeftPin, 40);
            analogWrite(forwardDriveRightPin, 40);
            yield return delay(1050);

            analogWrite(forwardDriveLeftPin, 0);
            analogWrite(backwardDriveLeftPin, 0);
            analogWrite(forwardDriveRightPin, 0);
            analogWrite(backwardDriveRightPin, 0);
            
            //When passed the pillar, drive forwards until the robot finds the line again:
            while (!passedPillar)
            {
                analogWrite(forwardDriveRightPin, 90);
                analogWrite(forwardDriveLeftPin, 90);
                if (analogRead(5) < 100)
                {
                    passedPillar = true;
                }
                yield return delay(1); //To stay in loop.
            }
            passedPillar = true;
        }


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
        servo = FindObjectOfType<Servo>();
        if (servo == null)
        {
            Debug.Log("No servo found in the scene. Consider assigning it to 'ArduinoMain.cs' manually.");
        }
        Time.fixedDeltaTime = 0.005f; //4x physics steps of what unity normally does - to improve sensor-performance.
        StartCoroutine(setup());


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

    public ulong abs(long x)
    {
        return (ulong)Mathf.Abs(x);
    }

    public long constrain(long x, long a, long b)
    {
        return (x < a ? a : (x > b ? b : x));
    }


    #endregion PremadeDefinitions

    #region InterfacingWithBreadboard
    public int analogRead(int pin)
    {
        return breadboard.analogRead(pin);
    }
    public void analogWrite(int pin, int value)
    {
        breadboard.analogWrite(pin, value);
    }
    public bool digitalRead(int pin)
    {
        return breadboard.digitalRead(pin);
    }
    public void digitalWrite(int pin, bool isHigh)
    {
        breadboard.digitalWrite(pin, isHigh);
    }

    public ulong pulseIn(int pin)
    {
        return breadboard.pulseIn(pin);
    }
    #endregion InterfacingWithBreadboard
}