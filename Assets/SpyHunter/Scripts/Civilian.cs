using UnityEngine;
using System.Collections;
using SpyHunter.Game;

public class Civilian : MonoBehaviour {

	public GameObject player;
	public GameObject flame;
	public float health;
	bool alive;
	float accel;
	float maxSpeed;
	Rigidbody rb;
	GameObject cam;
	float forwardAngle;
	Transform currentRoad;

	// Use this for initialization
	void Start ()
	{
		alive = true;
		accel = 1;
		maxSpeed = 40;
		rb = this.GetComponent<Rigidbody>();
		//rb.velocity = new Vector3(maxSpeed, 0, 0);
		//rb.velocity = maxSpeed * this.transform.forward;
		cam = player.GetComponent<CarMoveBasic>().cam;
		forwardAngle = this.transform.rotation.eulerAngles.y;
	}

	public void SetInitialVelocity(Vector3 dir)
	{
		rb.velocity = maxSpeed * dir;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(player.GetComponent<PhoneControls>().Menu == false)
		{
			if(alive == true)
			{
				move ();
				turnTowardsVelocity();
				constrainRotation(10);
			}
			DestroyCar();
		}
	}

	void die(bool playerKilledMe)
	{
		if(alive == true)
		{
			alive = false;
			flame.SetActive(true);
			if(playerKilledMe == true) GameManager.instance.AddToScore(-1000);
		}
	}

	void move()
	{
		float rA = Mathf.Deg2Rad*forwardAngle;

		//Debug.Log(Mathf.Sin(rA));

		rb.velocity += accel * new Vector3(
			Mathf.Sin(rA),
			0,
			Mathf.Cos(rA));

		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
	}

	void turnTowardsVelocity()
	{
		if(rb.velocity.magnitude > 0.3)
		{
			Vector3 horizVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
			if(horizVel.x < 0) horizVel = horizVel * -1;	// Keeps vehicle facing forwards
			this.transform.rotation = Quaternion.Lerp (
				this.transform.rotation, 
				Quaternion.LookRotation(horizVel), 
				0.1f);
		}
	}
	
	void constrainRotation(float angle)
	{
		float leftBound = forwardAngle - angle;
		float rightBound = forwardAngle + angle;
		Vector3 rotation = this.transform.rotation.eulerAngles;
		if(rotation.y < leftBound) rotation.y = leftBound;
		if(rotation.y > rightBound) rotation.y = rightBound;
		this.transform.rotation = Quaternion.Euler(rotation);
	}

	void moveAwayFromEdges()
	{
		if(currentRoad == null) return;
		
		// ISSUES WILL ARISE ON CURVED ROADS
		float roadWidth = 50;
		
		Vector3 positionVector = this.transform.position - currentRoad.position;
		float dotProduct = Vector3.Dot(-currentRoad.forward, positionVector);
		//Debug.Log (distanceToSide);
		float distanceToSide = dotProduct;
		
		float leftBound = -roadWidth/2;
		float rightBound = roadWidth/2;
		float threshhold = 10;		// How close before force becomes active
		float closeness = 32;		// Scale of -1 to 1
		float forceSize = 3;
		
		float pos = distanceToSide;
		
		if(pos < leftBound + threshhold)
		{
			closeness = (pos - leftBound)/threshhold;
		}
		if(pos > rightBound - threshhold)
		{
			closeness = (rightBound - pos)/threshhold * -1;
		}
		if(closeness != 32)
		{
			Vector3 accel = this.transform.right * closeness * forceSize;
			rb.velocity += accel;
		}
	}

	void DestroyCar()
	{
		if(Vector3.Distance(this.transform.position, player.transform.position) > 1500)
		{
			Destroy (this.gameObject);
		}

		if(Vector3.Dot(this.transform.position-cam.transform.position, cam.transform.forward) < 0)
		{
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "SideGround")
		{
			//die (false);
		}
		
		if(col.gameObject.tag != "Ground" && 
		   col.gameObject.tag != "PlayerBullet" && 
		   col.relativeVelocity.magnitude > 75)
		{
			die(true);
		}

		if(col.gameObject.tag == "PlayerBullet")
		{
			--health;
			if(health <= 0)
				die (true);
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.tag == "RoadTriggerBox")
		{
			forwardAngle = c.gameObject.transform.parent.transform.rotation.eulerAngles.y + 90;
			currentRoad = c.gameObject.transform;
		}
	}
}
