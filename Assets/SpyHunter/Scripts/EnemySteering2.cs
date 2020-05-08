using UnityEngine;
using System.Collections;

public class EnemySteering2 : MonoBehaviour 
{
	public float maxSpeed;
	public float maxForce;
	public float forwardWeight;
	public float seekWeight;
	[Range(0.0f, 1.0f)]public float turnLerp;
	public float sideOffset;
	public float angleLimit;
	public float ramPower;
	public float ramInterval;
	public float ramDistance;
	Vector3 acceleration;
	Rigidbody rb;
	Vector3 currentForwardDirection;
	GameObject player;
	float speedOffset;
	int speedOffsetMult;
	int playerSideMult;
	float ramTime;

	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		player = GameObject.FindGameObjectWithTag("Player");
		currentForwardDirection = transform.forward;
		speedOffsetMult = 1;
		ramTime = 0;
	}
	
	void Update () 
	{
		ChangeSpeed();
		TurnCar ();
		MoveForward ();
	}

	void MoveForward ()
	{
		rb.velocity += transform.forward * maxForce;
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed + (speedOffset * speedOffsetMult));

		Ram ();
	}

	void UpdateForward(Vector3 dir)
	{
		currentForwardDirection = dir;
	}

	void TurnCar()
	{
		acceleration = Vector3.zero;
		acceleration += AimForward();
		acceleration += AimSeek();
		acceleration.Normalize();
		transform.forward = Vector3.Lerp(transform.forward, acceleration, turnLerp);
		CheckIfTurnedTooFar();
	}

	void CheckIfTurnedTooFar()
	{
		float a = Vector3.Angle(currentForwardDirection, transform.forward);
		if (a > angleLimit)
			transform.forward = Vector3.Lerp(transform.forward, currentForwardDirection, turnLerp);
	}

	Vector3 AimForward()
	{
		return currentForwardDirection * forwardWeight;
	}

	Vector3 AimSeek()
	{
		return (GetPlayerSidePosition() - transform.position).normalized * seekWeight;
	}

	Vector3 GetPlayerSidePosition()
	{
		playerSideMult = -1;	// Moves to right or something
		if(Vector3.Dot (player.transform.right, transform.position-player.transform.position) > 0)
			playerSideMult = 1;	// Moves to left or something
		return player.transform.position + player.transform.right * sideOffset * playerSideMult;
	}

	void ChangeSpeed()
	{
		float dotProduct = Vector3.Dot (transform.forward, player.transform.position - transform.position);
		speedOffset = dotProduct+1;
		if(dotProduct < -1)
			speedOffset/=2;
	}

	void Ram()
	{
		Debug.DrawLine(player.transform.position, player.transform.position + player.transform.forward);

		float fDotProduct = Vector3.Dot (transform.forward, player.transform.position - transform.position);
		float hDotProduct = Vector3.Dot (transform.right, player.transform.position - transform.position);
		if(fDotProduct > -0.5f && fDotProduct < 0.5f &&
		   hDotProduct > -ramDistance && hDotProduct < ramDistance)
			ramTime += Time.deltaTime;
		if(ramTime >= ramInterval)
		{
			ramTime -= ramInterval;
			rb.velocity += transform.right * -playerSideMult * ramPower;
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.tag == "RoadTriggerBox")
		{
			currentForwardDirection = c.gameObject.transform.right;
				// The orientation of this was stupid, so the forward direction is right
		}
	}
}
