using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class WheelControl : MonoBehaviour
{
    public Transform carTransform;
    public Rigidbody carRigidbody;
    public GameObject tyre;

    public bool steering;

    [Header("Acceleration Control")]
    public float accelSpeed;
    public float brakeForce;
    public float carTopSpeed;
    public AnimationCurve powerCurve;

    [Header ("Suspension Control")]
    public float restDistance;
    public float springStrength;
    public float springDamper;

    [Header("Steering Control")]
    [Range(0.0f, 1.0f)]
    public float gripFactor;
    [Range(0.0f, 1.0f)]
    public float tireMass;
    public float steeringSpeed;
    public float steerLerp;
    public float maxSteeringAngle = 50;
    public float minSteeringAngle = -50;
    public AnimationCurve steerCurve;
    public AnimationCurve stiffnessCurve;

    public TMP_Text steerAngleText;

    private float currentHorizontalInput;
    private float gasInput;
    private float brakeInput;
    private float speedInKmh;

    bool grounded;
    RaycastHit hit;
    void FixedUpdate()
    {
        ShootRaycast();

        gasInput = Input.GetAxis("Accelerate") * accelSpeed;
        brakeInput = Input.GetAxis("Brake") * brakeForce;

        float steeringInput = Input.GetAxisRaw("Horizontal");                                                   //get horizontal axi 0-1

        //float normalizedInput = Mathf.Clamp(steeringInput, -1f, 1f);

        float curveEval = steerCurve.Evaluate(Mathf.Abs(steeringInput));                                        //ie. if steering is 0.5 - curveEval = 0.6
        //consider remapping curve bc of -1
        //float remapCurveEval = (curveEval - 0.5f) * 2;

        //steering stiffness on high speed
        //get the magnitude of the car velocity: carRigidbody.velocity.magnitude
        //normalize it to 0-1f
        //setup a curve where x = normalizeCarVelocity and y = appliedStiffness
        //this results in a stifness factor that gets multiplied onto the steering speed.
        float carVel = carRigidbody.velocity.magnitude;
        float normalizedCarVel = Mathf.Clamp01(carVel / carTopSpeed);
        //Debug.Log("normalizedCarVel: " + normalizedCarVel);
        float appliedStiffness = stiffnessCurve.Evaluate(Mathf.Abs(normalizedCarVel));
        Debug.Log("appliedStiffness: " + appliedStiffness);

        currentHorizontalInput = Mathf.Lerp(currentHorizontalInput, steeringInput, steerLerp * Time.deltaTime); //currentHorizontalInput will reach steering Input
                                   //0.6                25     = 15   *     0.6     = 9     
        float steerRot = currentHorizontalInput * steeringSpeed * appliedStiffness;                       //suppose chi = 0.6 and steerSpd = 25. 
                
        steerRot = Mathf.Clamp(steerRot, minSteeringAngle, maxSteeringAngle);


        transform.localRotation = Quaternion.Euler(0f, steerRot, 0f);                                   //apply value to localRotation

        steerAngleText.text = "angle: " + (Mathf.Round(steerRot * 1000f) / 1000f);
        //Debug.Log("curve eval: " + curveEval);
        Debug.Log("steer rot: " + steerRot);
        //Debug.Log("raw input: " + steeringInput);
    }

    void ShootRaycast()
    {
        if (Physics.Raycast(transform.position, -transform.up, out hit, restDistance))          //if ray touches the ground 
        {
            grounded = true;
            Suspension(); //seems ok
            Acceleration(); //clear
            Steering(); //broke -- would ramp up like crazy
            //Debug.Log("grounded: " + grounded);
        }
        else                                                                                    //if not touching
        {
            grounded = false;
        }
    }
    void Acceleration()
    {
        //world-space direction of the acceleration OR braking force.
        Vector3 accelDir = transform.forward;

        //acceleration torque
        if (gasInput > 0.0f)
        {
            //getting current forward speed of the car (in the driven direction)
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

            //normalize car speed                        0 - 100+         ???
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

            //how much to add?
            //available torque                                       
            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * gasInput; //look into their power curve stuff

            //apply force                    forward       
            carRigidbody.AddForceAtPosition(accelDir * availableTorque, transform.position);

            Debug.DrawRay(transform.position, transform.forward * availableTorque, Color.blue);

            //debugging speed values
            speedInKmh = normalizedSpeed * carTopSpeed * 3.6f;

            //Debug.Log("car speed: " + speedInKmh.ToString("F2") + "km/h");
            //Debug.Log("available torque: " + availableTorque);
            //Debug.Log("car speed normalized: " + normalizedSpeed);

        }

        if (brakeInput > 0.0f)
        {
            //reverse or slow
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

            float brakePressure = brakeInput;

            carRigidbody.AddForceAtPosition(-accelDir * brakePressure, transform.position); //apply brakes

        }

        if (Input.GetKeyDown(KeyCode.Space))
        { 
            //when held down begin slowing down
        }

        Debug.DrawRay(transform.position, transform.forward * 1, Color.blue);
    }
    void Suspension()
    {
        //spring force worldspace
        Vector3 springDir = transform.up; //applies upwards when hitting the ground

        //tire worldspace velocity
        Vector3 tireWorldVelocity = carRigidbody.GetPointVelocity(transform.position);

        //calculate offset from the raycast
        float offset = restDistance - hit.distance;

        //calculate velocity along the spring direction
        float velocity = Vector3.Dot(springDir, tireWorldVelocity);

        //calculate the magnitude of the dampened spring force
        float force = (offset * springStrength) - (velocity * springDamper);

        tyre.transform.position = new Vector3(transform.position.x, transform.position.y + (offset), transform.position.z);

        //apply the force at the location of the tire and in the suspension's direction
        carRigidbody.AddForceAtPosition(springDir * force, transform.position);

        Debug.DrawRay(transform.position, transform.up * (velocity), Color.green);          //draw force enacting upwards
        Debug.DrawRay(transform.position, transform.up * (offset), Color.yellow);        //ray pointing downwards
    }
    void Steering()
    {
        //spring force worldspace
        Vector3 steeringDir = transform.right;

        //suspension worldspace velocity
        Vector3 tireWorldVelocity = carRigidbody.GetPointVelocity(transform.position);

        //whats the tire velocity in steering direction?
        //steeringDir is a unit vector, this returns a magnitude of tireWorldVelocity projected onto steering direction
        float steeringVel = Vector3.Dot(steeringDir, tireWorldVelocity);

        //we want to look for the negative steeringVel * gripFactor || we want the force to push us to the direction
        //gripFactor is in range 0-1, 0 = no grip, 1 = grippy
        float desiredVelChange = -steeringVel * gripFactor;

        //velocity change will become acceleration (acceleration = change in velocity / time)
        //this value produces acceleration that changes the velocity by desiredVelocityChange in 1 physics step
        float desiredAcceleration = desiredVelChange / Time.fixedDeltaTime;

        //Force = mass * acceleration, multiply by the mass of the tire and apply as a force
        carRigidbody.AddForceAtPosition(steeringDir * tireMass * desiredAcceleration, transform.position);


        //force lines
        Debug.DrawRay(transform.position, transform.right * restDistance, Color.red);          //draw force enacting upwards
        Debug.DrawRay(transform.position, transform.right * (steeringVel), Color.magenta);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(carTransform.position, carTransform.forward * 5, Color.red);
    }

}
