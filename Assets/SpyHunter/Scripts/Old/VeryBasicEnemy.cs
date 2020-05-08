using UnityEngine;
using System.Collections;

public class VeryBasicEnemy : MonoBehaviour {

	Quaternion initialRotation;
	Vector3 nextToPlayer;

	public float distanceAlongside;
	public float accel;
	public GameObject player;
	public float maxDistForSlow;
	public float speed;

	public float speedDif;


	// Use this for initialization
	void Start ()
	{
		initialRotation = this.transform.rotation;

		/*
		this.rigidbody.velocity = new Vector3(
			startSpeed,
			this.rigidbody.velocity.y,
			this.rigidbody.velocity.z);
		*/
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		MaintainRotation();
		ChangeVelocityTowardsPlayer();
	}

	void MaintainRotation()
	{
		this.transform.rotation = Quaternion.Lerp(
			this.transform.rotation, 
			initialRotation,
			0.1f);
	}

	void ChangeVelocityTowardsPlayer()
	{
		// Moving in x direction. For some reason
		Vector3 vel = this.gameObject.GetComponent<Rigidbody>().velocity;
		float pVel = player.GetComponent<Rigidbody>().velocity.x;
		float dist = player.transform.position.x - this.transform.position.x;
		Vector3 newVel = new Vector3(pVel*dist/2f,
		                             vel.y,
		                             vel.z);
		if(pVel >= 0)
			this.gameObject.GetComponent<Rigidbody>().velocity = newVel;

		Ram ();
	}

	void NewMoveTowardsPlayer()
	{
		// Decide whether to go to left or right of player
		if(this.transform.position.z > player.transform.position.z)
		{
			nextToPlayer = new Vector3(player.transform.position.x, 
			                           player.transform.position.y, 
			                           player.transform.position.z + distanceAlongside);
		}
		else
		{
			nextToPlayer = new Vector3(player.transform.position.x, 
			                           player.transform.position.y, 
			                           player.transform.position.z - distanceAlongside);
		}
		
		// Lerp towards the side of the car in one dimension (z)
		Vector3.Lerp (
			this.transform.position,
			new Vector3(this.transform.position.x,
		            this.transform.position.y,
		            nextToPlayer.z),
			0.25f);

		// Accelerate towards player

		float bufferZone = 0.5f;

		if(this.transform.position.x < player.transform.position.x - bufferZone)
		{
			this.GetComponent<Rigidbody>().velocity = new Vector3(
				player.GetComponent<Rigidbody>().velocity.x + speedDif,
				this.GetComponent<Rigidbody>().velocity.y,
				this.GetComponent<Rigidbody>().velocity.z);
		}
		
		else if(this.transform.position.x > player.transform.position.x + bufferZone)
		{
			this.GetComponent<Rigidbody>().velocity = new Vector3(
				player.GetComponent<Rigidbody>().velocity.x - speedDif,
				this.GetComponent<Rigidbody>().velocity.y,
				this.GetComponent<Rigidbody>().velocity.z);
		}
		else
		{
			this.GetComponent<Rigidbody>().velocity = new Vector3(
				player.GetComponent<Rigidbody>().velocity.x,
				this.GetComponent<Rigidbody>().velocity.y,
				this.GetComponent<Rigidbody>().velocity.z);
		}


		/*
		float bufferZone = 10;

		if(this.transform.position.x < player.transform.position.x - bufferZone)
		{
			this.rigidbody.velocity = new Vector3(
				player.rigidbody.velocity.x + speedDif,
				this.rigidbody.velocity.y,
				this.rigidbody.velocity.z);
		}

		else if(this.transform.position.x > player.transform.position.x + bufferZone)
		{
			this.rigidbody.velocity = new Vector3(
				player.rigidbody.velocity.x - speedDif,
				this.rigidbody.velocity.y,
				this.rigidbody.velocity.z);
		}

		else
		{
			// If thisPos - playerPos is -, this is behind player, at higher speed, must slow down over distance
			// If thisPos - playerPos is +, this is ahead of player, at slower speed, must speed up down over distance
			float distance = this.transform.position.x - player.transform.position.x;
			float distPercent = distance/bufferZone;
				// Percent of distance away from player
			float newSpeedDif = distPercent * speedDif;
			this.rigidbody.velocity = new Vector3(
				player.rigidbody.velocity.x + newSpeedDif,
				this.rigidbody.velocity.y,
				this.rigidbody.velocity.z);
		}
		*/
	}







	void MoveTowardsPlayer()
	{
		// Dist of 5
		if(this.transform.position.z > player.transform.position.z)
		{
			nextToPlayer = new Vector3(player.transform.position.x, 
			                           player.transform.position.y, 
			                           player.transform.position.z + distanceAlongside);
		}
		else
		{
			nextToPlayer = new Vector3(player.transform.position.x, 
			                           player.transform.position.y, 
			                           player.transform.position.z - distanceAlongside);
		}

		Vector3 between = new Vector3(
			nextToPlayer.x - this.transform.position.x,
		    0f,
			nextToPlayer.z - this.transform.position.z);

		Vector3 dir = between.normalized;

		float distance = between.x;
		float thisSpeed = this.GetComponent<Rigidbody>().velocity.magnitude;
		float playerSpeed = player.GetComponent<Rigidbody>().velocity.magnitude;
		float speedDif = playerSpeed - thisSpeed;
		float topSpeed;

		// Match speed 

		if(distance > maxDistForSlow)
		{
			topSpeed = speed;
		}
		else
		{
			topSpeed = (maxDistForSlow/distance) * playerSpeed;
		}


		if(thisSpeed < topSpeed)
		{
			//this.rigidbody.AddForce(new Vector4(dir.x * accel, dir.y, dir.z), ForceMode.Acceleration);
			this.GetComponent<Rigidbody>().AddForce(dir * accel, ForceMode.Acceleration);
		}
		else if(thisSpeed > topSpeed)
		{
			this.GetComponent<Rigidbody>().AddForce(dir * -accel, ForceMode.Acceleration);
		}
		/*
		if(between.magnitude < maxDistForSlow)
		{
			float factor = between.magnitude/maxDistForSlow;
			//this.rigidbody.AddForce(dir * accel * factor, ForceMode.Acceleration);
			this.rigidbody.AddForce(new Vector3(dir.x * -accel, dir.y, dir.z), ForceMode.Acceleration);
		}
		else
		{
			this.rigidbody.AddForce(dir * accel, ForceMode.Acceleration);
		}


		if(between.x < 20f && between.x > -20f)
		{
			Debug.Log (between.x);
			this.rigidbody.AddForce(
					new Vector3(dir.x * accel * (between.x - 5f), dir.y, dir.z), 
					ForceMode.Acceleration);
		}
		*/


	}

	void Ram()
	{
		if(player.transform.position.x - this.transform.position.x < 2 ||
		   player.transform.position.x - this.transform.position.x > -2)
		{
			if(player.GetComponent<Rigidbody>().velocity.magnitude - this.GetComponent<Rigidbody>().velocity.magnitude < 1 ||
			   player.GetComponent<Rigidbody>().velocity.magnitude - this.GetComponent<Rigidbody>().velocity.magnitude > -1)
			{
				if(this.transform.position.z > player.transform.position.z)
				{
					this.GetComponent<Rigidbody>().AddForce(Vector3.back*60f, ForceMode.Acceleration);
				}
				if(this.transform.position.z < player.transform.position.z)
				{
					this.GetComponent<Rigidbody>().AddForce(Vector3.forward*60f, ForceMode.Acceleration);
				}
			}
		}
	}
}
