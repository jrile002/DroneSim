/*
 * Drone Simulation Script
 * CS 179J - Spring 2019
 * Joshua Riley
 * Adriel Bustamente
 * Colton Vosburg
 * Jonathan Woolf
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovementScript : MonoBehaviour
{
    
	Rigidbody drone;
	// Total upward force from propellers
	public float lift;
	// Drone movement speeds
	public float forwardMovementAmount = 100.0f;
	public float sideMovementAmount = 100.0f;
	// Associated movement tilts
	private float tiltAmountForward = 0;
	private float tiltAmountSideways;
	private float tiltVelocityForward;
	private float tiltAmountVelocity;
	// Drone rotation
	private float desiredYRotation;
	[HideInInspector] public float currentYRotation;
	private float rotateAmountByKeys = 2.5f;
	private float rotationYVelocity;
	// For speed clamping
	private Vector3 velocitySmoothDamp;
	
	// Called only once, after all objects are initialized
	void Awake() {
		drone = GetComponent<Rigidbody>();
	}
	
	// Called every fixed frame-rate frame; Default time between calls: 0.02 seconds
	void FixedUpdate(){
		MovementUpDown();
		MovementLeftRight();
		MovementForwardBackward();
		Rotation();
		ClampingSpeedValues();
		LiftCompensation();
		
		drone.AddRelativeForce(Vector3.up * lift);
		drone.rotation = Quaternion.Euler(
			new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways)
		);
	}
	
	// Simulates upward and downward movement
	void MovementUpDown(){		
		if (Input.GetKey(KeyCode.I)){
			lift = 300;
		}
		else if (Input.GetKey(KeyCode.K)){
			lift = 50;
		}
		else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)){
			// Hovers when stationary
			lift = 98.1f;
		}
		
	}
	
	// Simulates forward and backward movement
	void MovementForwardBackward(){
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f){
			drone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * forwardMovementAmount);
			tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);
			
		}
		else {
			tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 0, ref tiltVelocityForward, 0.1f);
		}
		
	}
	
	// Simulates left and right movement
	void MovementLeftRight(){
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f){
			drone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMovementAmount);
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.1f);
		}
		else {
			tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
		}
	}
	
	// Simulates left and right rotational movement
	void Rotation(){
		if(Input.GetKey(KeyCode.J)){
			desiredYRotation -= rotateAmountByKeys;
		}
		if (Input.GetKey(KeyCode.L)){
			desiredYRotation += rotateAmountByKeys;
		}
		
		currentYRotation = Mathf.SmoothDamp(currentYRotation, desiredYRotation, ref rotationYVelocity, 0.25f);
	} 
	
	// Clamps movement speeds
	void ClampingSpeedValues(){
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f){
			drone.velocity = Vector3.ClampMagnitude(drone.velocity, Mathf.Lerp(drone.velocity.magnitude, 30.0f, Time.deltaTime * 5f));
		}
		else {
			drone.velocity = Vector3.SmoothDamp(drone.velocity, Vector3.zero, ref velocitySmoothDamp, 0.95f);
		}
	}
	
	// Compensate the lift for when the drone is tilted
	void LiftCompensation(){
		if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f){
			if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L)){
				drone.velocity = new Vector3(drone.velocity.x, Mathf.Lerp(drone.velocity.y, 0, Time.deltaTime*5), drone.velocity.z);
				lift = 135;
			}
			else if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L)){
				drone.velocity = new Vector3(drone.velocity.x, Mathf.Lerp(drone.velocity.y, 0, Time.deltaTime*5), drone.velocity.z);
				lift = 150;
			}
		}
		if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f){
			if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L)){
				drone.velocity = new Vector3(drone.velocity.x, Mathf.Lerp(drone.velocity.y, 0, Time.deltaTime*5), drone.velocity.z);
				lift = 140;
			}
			else if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L)){
				drone.velocity = new Vector3(drone.velocity.x, Mathf.Lerp(drone.velocity.y, 0, Time.deltaTime*5), drone.velocity.z);
				lift = 145;
			}
		}
	}
	
}
