using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoFunctions;


public class Motor : MonoBehaviour
{
    public Rigidbody wheel;
    
    [SerializeField]
    private float maxMotorTorque = 80;

    private float motorTorque;

    private int motorSign = 1; //-1 or 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SetSpeed(0);
            return;
        }
        //Test:
        SetSpeed(1023);
        SetDirection(false);


    }

    void FixedUpdate()
    {
        //Updates direction and speed

        wheel.AddRelativeTorque(Vector3.right * motorTorque * motorSign, ForceMode.Force);
    }

    #region ControlMethods
    //SetDirection is what the H-bridge will interface with the motor with.
    public void SetDirection(bool isHigh)
    {
        //Default is driving forward, if "HIGH" is sent through this function, motor will reverse
        motorSign = isHigh ? -1 : 1;
    }
    //Conversion method, only used in case HIGH/LOW is used as an int instead of bool.
    public void SetDirection(int isHigh)
    {
        bool isHighBool = isHigh == 0 ? false : true;
        SetDirection(isHighBool);
    }

    public void SetSpeed(int analogValue)
    {
        Debug.Assert(analogValue < 1024 && analogValue >= 0);

        //Mapping of analog value to force in motor/wheel:
        motorTorque = Functions.map(analogValue, 0, 1023, 0, maxMotorTorque);
    }
    #endregion ControlMethods
}