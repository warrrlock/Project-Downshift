using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
public class CarParent : MonoBehaviour
{
	public Vector3 _localCenterOfMass;
	public Rigidbody carRb;
	public WheelControl frontRight, frontLeft, rearRight, rearLeft;
	public TMP_Text driftOffsetText;
	public TMP_Text carSpeedText;

	private float driftOffset;

	private void Awake()
	{
		SetCenterOfMass();
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

    private void FixedUpdate()
    {
		driftOffset = Vector3.Dot(carRb.GetPointVelocity(transform.position).normalized, carRb.transform.right.normalized);

		//text displays
		carSpeedText.text = "Speed: " + (carRb.velocity.magnitude * 3.6f).ToString("F2") + "km/h";
		driftOffsetText.text = "Drift Offset: " + (Mathf.Round(driftOffset * 1000f) / 1000f);
		//Debug.Log("speed(carparent): " + carRb.velocity.magnitude);
    }

    void SetCenterOfMass()
	{
		carRb.centerOfMass = _localCenterOfMass;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 worldCenterOfMass = transform.TransformPoint(_localCenterOfMass);
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.TransformPoint(_localCenterOfMass), 0.1f);
		Gizmos.DrawLine(worldCenterOfMass + Vector3.up, worldCenterOfMass - Vector3.up);
		Gizmos.DrawLine(worldCenterOfMass + Vector3.forward, worldCenterOfMass - Vector3.forward);
		Gizmos.DrawLine(worldCenterOfMass + Vector3.right, worldCenterOfMass - Vector3.right);
	}
}
