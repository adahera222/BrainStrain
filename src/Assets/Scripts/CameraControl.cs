using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public enum CameraState
	{
		MainMenu,
		Game,
		LevelComplete
	}
	
	//public bool ControllByUser{ get; set; }
	public CameraState State{get; set;}
	public Vector3 RotationCenter;
	public float MinDistance = 2.0f;
	public float Distance = 6f;
	public float MaxDistance = 10.0f;
	public float RotationX = 45f;
	public float RotationY = 30f;
	public float SpeedX = 250.0f;
	public float SpeedY = 187.5f;
	public float MinLimitY = -90f;
	public float MaxLimitY = 90f;
	
	private float xRotation = 0.0f;
	private float yRotation = 0.0f;
	
#if UNITY_ANDROID || UNITY_IPHONE
	private const float maxHeldTime = 0.25f;
	private float heldTime = 0f;
#endif
	
	void Start()
	{
		RestartView();
	}
	
	void Update ()
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		 GenerateOnMouseEvents();
		#endif
		
		switch(State)
		{
			case CameraState.MainMenu:
				BounceBackAndForth();
				break;
			case CameraState.Game:
				ZoomCamera();
				RotateCamera();
				break;
			case CameraState.LevelComplete:
				RotateAroundCenter();
				break;
		}
        
        transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
		transform.position = transform.rotation * new Vector3(0.0f, 0.0f, -Distance) + RotationCenter;
	}
	
	private void ZoomCamera()
	{
		float zoomAmount = 0;
		
		#if UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
		{
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			
			Vector2 curDist;
			Vector2 prevDist;
			
			curDist = touch1.position - touch2.position; //current distance between finger touches
     		prevDist = ((touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition)); //difference in previous locations using delta positions
			zoomAmount = (curDist.magnitude - prevDist.magnitude) / 200f;
		}
		#else
		zoomAmount = Input.GetAxis("Mouse ScrollWheel");
		#endif
		Distance *= 1 - zoomAmount; //same as *= 1.1 or 0.9 for zooming
		Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
	}
	
	private void RotateAroundCenter()
	{
		xRotation -= Time.deltaTime * SpeedX * 0.1f;
		yRotation = 30f;
	}
	
	private int sign = 1;
	private void BounceBackAndForth()
	{
		var increase = Time.deltaTime * SpeedX * 0.07f * sign;
		if(xRotation + increase > 270 && xRotation + increase < 360)
		{
			xRotation += increase;
		}
		else
		{
			sign *= -1;
		}
		yRotation = 0f;
	}
	
	private void RotateCamera()
	{
		Vector2 rotateAmount = Vector2.zero;
		bool rotate;
		
		#if UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchCount == 1)
			heldTime += Time.deltaTime;
		else
			heldTime = 0;
		
		if(heldTime > maxHeldTime)
		{
			if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Touch touch = Input.GetTouch(0);
				rotateAmount = new Vector2(touch.deltaPosition.x, touch.deltaPosition.y) * 0.2f;
			}
			rotate = (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved);
		}
		else
		{
			rotate = false;	
		}
		#else
		if(Input.GetMouseButton(0))
		{
			rotateAmount.x = Input.GetAxis("Mouse X");
			rotateAmount.y = Input.GetAxis("Mouse Y");
		}
		rotate = Input.GetMouseButton(0);
		#endif
		
		
		if(rotate)
		{
	        xRotation += rotateAmount.x * SpeedX * 0.02f;
	        yRotation -= rotateAmount.y * SpeedY * 0.02f;
	 		yRotation = ClampAngle(yRotation, MinLimitY, MaxLimitY);
		}
	}
	
	#if UNITY_ANDROID || UNITY_IPHONE
	Collider current = null;
	Touch touch;
	private void GenerateOnMouseEvents()
	{
		if(Input.touchCount != 1 || GUIUtility.hotControl != 0)
			return;
		
		touch = Input.GetTouch(0);
		
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
		
		RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
		{
			switch(touch.phase)
			{
				case TouchPhase.Began:
					hit.collider.SendMessage("OnMouseDown");
					current = hit.collider;
					break;
				case TouchPhase.Ended:
					hit.collider.SendMessage("OnMouseUp");
					current.SendMessage("OnMouseUp");
					if(hit.collider == current)
					{
						hit.collider.SendMessage("OnMouseUpAsButton");
					}
					break;
				case TouchPhase.Canceled:
					hit.collider.SendMessage("OnMouseUp");
					current.SendMessage("OnMouseUp");
					current = null;
					break;
			}
		}
	}
    #endif
	
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
	
	public void RestartView()
	{
		Distance = (MinDistance + MaxDistance) / 2f;
		xRotation = RotationX;
		yRotation = RotationY;
	}
}
