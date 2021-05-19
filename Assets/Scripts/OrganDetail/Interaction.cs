using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
	private Camera camera;
	private GameObject gyroscope;

	private float perspectiveZoomSpeed = 0.05f;        
	private float orthoZoomSpeed = 0.05f;
	
	private float eigen = 0.005f;

	Touch touchZero;
	Touch touchOne;
	Vector2 touchZeroPrevPos;
	Vector2 touchOnePrevPos;
	float prevTouchDeltaMag;
	float touchDeltaMag;
	float deltaMagnitudeDiff;

	Touch touch;
	float horizontal;
	float vertical;

	private float Azi = 0f;
	private float Ele = 0f;
	float sensitivity = 5f;

	Gesture currentGes;

	enum Gesture {
		None, // 
		Stationary,  // 
		Press,  //
		Swipe  //
	}
	
	Vector2 oriPosition;

	void Start()
	{
		gyroscope = GameObject.Find("Gyroscope");
		camera = Camera.main;
	}
	
	void Update()
	{
		currentGes = Gesture.None;
		if (Input.touchCount < 1) {
			return;
		}
		Touch touch = Input.GetTouch(0);
		
		if (Input.touchCount == 1 && 
			(Helper.GetObjectOnTouchByTag(touch.position, ObjectTag.mainView) != null)) 
		{
			HandleSingleTouch(touch);
		}
		else if(Input.touches.Length == 2)
		{
			HandleMultiTouch(touch);
		}
	}

	private void HandleSingleTouch(Touch touch)
	{
		switch (touch.phase) 
		{
			case TouchPhase.Began:
				oriPosition = touch.position;
				break;
			case TouchPhase.Moved:
				Vector2 delta = touch.position - oriPosition;
				if (delta.magnitude > sensitivity)
					Move(touch, delta);
				break;
			case TouchPhase.Stationary:
				currentGes = Gesture.Stationary;
            	break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				currentGes = Gesture.None;
				break;
		}
	}

	public void HandleSingleTouchj(Touch touch)
	{
		switch(touch.phase)
		{
			case TouchPhase.Began:
				oriPosition = touch.position; 
				break; 
			
			case TouchPhase.Moved: 
				Vector2 delta = touch.position - oriPosition; 
				if(delta.magnitude > sensitivity)
					Move(touch, delta); 
				break; 

			case TouchPhase.Stationary: 
				currentGes = Gesture.Stationary; 
				break; 

			case TouchPhase.Ended: 
				// Kết thúc ở đây 
			case TouchPhase.Canceled: 
				currentGes = Gesture.None; 
				break; 
		}
	}
	

	// If there are two touches on the device 
	private void HandleMultiTouch(Touch touch)
	{
		// Store both touches 
		// Touch touchZero = Input.GetTouch(0); 
		// Touch touchOne = Input.GetTouch(1);
		touchZero = Input.GetTouch(0);
		touchOne = Input.GetTouch(1);

		// Find the position in the previous frame of each touch
		// Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; 
		// Vector2 touchOnePrevPos= touchOne.position - touchOne.deltaPosition; 
		touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		// Find the magnitude of the vector between the touch in each frame 
		prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

		// Find the difference in the distances between each frame.
		deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			
		// If the camera is orthographic...
		if (camera.orthographic)
		{
			// ... change the orthographic size based on the change in distance between the touches.
			camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			// Make sure the orthographic size never drops below zero.
			camera.orthographicSize = Mathf.Max(camera.orthographicSize, 10f);
		}
		else
		{
			// Otherwise change the field of view based on the change in distance between the touches.
			camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

			// Clamp the field of view to make sure it's between 10 and 90.
			camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10f, 90f);
		}
	}

	// public void HandleMultiTouch(Touch touch)
	// {
	// 	touchZero = Input.GetTouch(0); 
	// 	touchOne = Input.GetTouch(1);

	// 	touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; 
	// 	touchOnePrevPos = touchOne.position - touchOne.deltaPosition; 

	// 	prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; 
	// 	touchDelatMag = (touchZero.position - touchOne.position).magnitude; 

	// 	// Find the difference in the distances between each frame
	// 	deltaMagnitudeDiff = prevTouchDeltaMag - touchDelatMag; 

	// 	// If the camera is orthographic 
	// 	if(camera.orthographic)
	// 	{
	// 		// .. change the orthographic size based on the change in distance ...
	// 		camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed; 


	// 		// ... Make sure the orthographic 
	// 	}

	// }

	private void HandleInteraction(Gesture ges) 
	{
	}

	private void Press(Touch touch)
	{

	}

	private void Move(Touch touch, Vector2 delta) 
	{
		if	(OrganManager.IsMoving)
			PerformTranform(delta);
		else
			PerformRotate(delta);
	}

	private void PerformTranform(Vector2 delta)
	{
		Vector3 translate = new Vector3(	camera.transform.position.x - delta[0] * eigen * 0.02f * Mathf.Cos(gyroscope.transform.eulerAngles.y * Mathf.Deg2Rad),
											camera.transform.position.y - delta[1] * eigen * 0.02f,
											camera.transform.position.z +  delta[0] * eigen * 0.02f * Mathf.Sin(gyroscope.transform.eulerAngles.y * Mathf.Deg2Rad));
		if (10 < translate.magnitude && translate.magnitude < 12)
			camera.transform.position = translate;
	}

	private void PerformRotate(Vector2 delta)
	{
		Ele -= delta[1] * eigen;
		if (Mathf.Abs(Ele) < 80f)
			gyroscope.transform.eulerAngles += new Vector3(-delta[1], delta[0], 0) * eigen;
		else
			gyroscope.transform.eulerAngles += new Vector3(0, delta[0], 0) * eigen;
	}
}
