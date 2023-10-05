using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    public Transform carTransform;
    public Rigidbody carRigidbody;
    public GameObject tyre;

    public bool steering;

    [Header("Acceleration Control")]
    public float accelSpeed;
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
    public float maxSteeringAngle = 50;
    public float minSteeringAngle = -50;

    public TMP_Text steerAngleText;

    private float steeringRotation;
    private float gasInput;

    bool grounded;
    RaycastHit hit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void FixedUpdate()
    {
        ShootRaycast();

        steeringRotation = Input.GetAxis("Horizontal") * (steeringSpeed);
        gasInput = Input.GetAxis("Vertical") * accelSpeed;
        
        float steerRot = Mathf.Clamp(steeringRotation, minSteeringAngle, maxSteeringAngle);

        //transform.Rotate(0, (steeringRotation), 0);
        transform.localRotation = Quaternion.Euler(0, (steerRot), 0);

        steerAngleText.text = "angle: " + (steerRot);

    }

    void ShootRaycast()
    {
        if (Physics.Raycast(transform.position, -transform.up, out hit, restDistance))          //if ray touches the ground 
        {
            grounded = true;
            Suspension(); //seems ok
            Acceleration(); //clear
            Steering(); //broke -- would ramp up like crazy
            Debug.Log("grounded: " + grounded);
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
            //current forward speed of the car (in the driven direction)
            float carSpeed = Vector3.Dot(carTransform.forward, carRigidbody.velocity);

            //normalize car speed
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

            //available torque
            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * gasInput; //look into their power curve stuff

            carRigidbody.AddForceAtPosition(accelDir * availableTorque, transform.position);

            Debug.DrawRay(transform.position, transform.forward * normalizedSpeed, Color.blue);

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
