using UnityEngine;
using System.Collections;
using SpyHunter.Game;

public class EnemyBehavior : MonoBehaviour 
{
	public float accelVal;
	public float maxVel;
	public float health;
	public float targetOffset;
	public float ramPower;
	public float ramInterval;
	public int pointValue;
	public GameObject target;
	public GameObject flame;
	public GameObject sparks;
	public AudioClip crashSound;
	public AudioClip tireSquealSound;
	public bool bulletproof;

	Rigidbody rb;
	Vector3 towardsTarget;
	float targetMaxVelocityLoGear;
	//float targetMaxVelocityHiGear;
	float forwardAngle;
	public float ForwardAngle { set {forwardAngle = value;}}
	Transform currentRoad;

	bool alive;
	bool grounded;
	bool oilSlicked;

	float timer;


	// Player mass is 1
	// Switchblade mass 0.3?
	// Road Lord mass 2?

	// Use this for initialization
	void Start () 
	{
		alive = true;
		grounded = true;
		oilSlicked = false;
		towardsTarget = Vector3.zero;
		rb = this.GetComponent<Rigidbody>();
		targetMaxVelocityLoGear = target.GetComponent<CarMoveBasic>().topSpeed;
		//targetMaxVelocityHiGear = target.GetComponent<CarMoveBasic>().topSpeedInHighGear;
		timer = 0;
		//forwardAngle = this.transform.rotation.eulerAngles.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(target.GetComponent<PhoneControls>().Menu == false)
		{
			if(alive == true && grounded == true && oilSlicked == false)
			{
				if(Vector3.Dot(this.transform.position-target.transform.position, target.transform.forward) > 5)
				{
					rb.velocity = Vector3.ClampMagnitude(
						rb.velocity + new Vector3(
							Mathf.Cos(Mathf.Deg2Rad*forwardAngle),
							0,
							-Mathf.Sin(Mathf.Deg2Rad*forwardAngle)) * accelVal, 
						targetMaxVelocityLoGear - 10);
					turnTowardsVelocity();
				}
				else if(Vector3.Distance(this.transform.position, target.transform.position) > 40 &&
				        Vector3.Dot(target.transform.forward, this.transform.position - target.transform.position) < 0)
				{
					// Instead of this, move enemy up to piece 8
					// Change position, rotation, velocity, and forward vector

					rb.velocity = Vector3.ClampMagnitude(
						rb.velocity + new Vector3(
						Mathf.Cos(Mathf.Deg2Rad*forwardAngle),
						0,
						-Mathf.Sin(Mathf.Deg2Rad*forwardAngle)) * accelVal, 
						100);
					turnTowardsVelocity();
				}
				else
				{
					aimVector();
					addToVelocity();
					turnTowardsVelocity();
					constrainRotation(10);
					moveAwayFromEdges();
					Ram ();
					//KeepEnemyClose();
				}
			}
			else
			{
				flame.transform.rotation = Quaternion.Euler(Vector3.up);
			}

			if(oilSlicked == true)
			{
				SpinOut();
			}

			DestroyCar();
		}
	}

	void aimVector()
	{
		float mult = -1;	// Moves to right or something
		if(CheckIfToRight() == true)
			mult = 1;	// Moves to left or something
		Vector3 shiftForward = new Vector3(Mathf.Cos(Mathf.Deg2Rad*forwardAngle),0,-Mathf.Sin(Mathf.Deg2Rad*forwardAngle));

		Vector3 targetPos = target.transform.position + 
			target.transform.right * targetOffset * mult + 
			shiftForward*2;
		towardsTarget = Vector3.Normalize(targetPos - this.transform.position);
	}

	bool CheckIfToRight()
	{
		if(Vector3.Dot (target.transform.right, this.transform.position-target.transform.position) > 0)
			return true;
		return false;
	}

	void addToVelocity()
	{
		rb.velocity = rb.velocity + (towardsTarget * accelVal);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVel);
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

	void constrainRotation(float angle)
	{
		float leftBound = forwardAngle + 90 - angle;
		float rightBound = forwardAngle + 90 + angle;
		Vector3 rotation = this.transform.rotation.eulerAngles;
		if(rotation.y < leftBound) rotation.y = leftBound;
		if(rotation.y > rightBound) rotation.y = rightBound;
		this.transform.rotation = Quaternion.Euler(rotation);
	}

	void Ram()
	{
		if(Vector3.Distance(this.transform.position, target.transform.position) < targetOffset + 0.5f)
		{
			++timer;
			if(timer > ramInterval)
			{
				float mult = 1;	// On right, ram left
				if(CheckIfToRight() == true)
					mult = -1;	// On left, ram right
				rb.velocity += this.transform.right * ramPower * mult;

				timer = 0;
			}
		}
	}

	void KeepEnemyClose()
	{
		/*
		if(this.transform.position.x < target.transform.position.x)
		{
			if(Vector3.Distance(this.transform.position, target.transform.position) > 50)
			{
				this.transform.position += new Vector3(10,0,0);
			}
		}
		*/
		if(Vector3.Distance(this.transform.position, target.transform.position) > 50 &&
		   Vector3.Dot(target.transform.forward, this.transform.position - target.transform.position) < 0)
		{

		}
	}

	void SpinOut()
	{
		int dir = 1;
		if(this.transform.position.z < 0) dir = -1;
		float angle = forwardAngle + 90*(-dir);
		rb.velocity += new Vector3(Mathf.Cos(Mathf.Deg2Rad*angle),0,-Mathf.Sin(Mathf.Deg2Rad*angle));
		this.transform.Rotate(new Vector3(0,-dir*2,0));
	}

	void die()
	{
		if(alive == true)
		{
			alive = false;
			rb.constraints = RigidbodyConstraints.None;
			flame.SetActive(true);
			GameManager.instance.AddToScore(pointValue);
		}
	}

	void DestroyCar()
	{
		if(Vector3.Distance(this.transform.position, target.transform.position) > 1500)
		{
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ground")
		{
			grounded = true;
		}
		else 
		{
			if(col.relativeVelocity.magnitude > 10)
			{
				AudioSource.PlayClipAtPoint(crashSound, this.transform.position, 0.01f);
				GameObject spark = (GameObject)Instantiate(
					sparks,
					col.contacts[0].point, 
					Quaternion.identity);
				spark.transform.parent = this.transform;
				//spark.GetComponent<EllipsoidParticleEmitter>().worldVelocity = rb.velocity;
			}
			if(col.relativeVelocity.magnitude > 75)
			{
				if(col.gameObject.tag != "PlayerBullet")
				{
					die ();
				}
			}
			if(col.gameObject.tag == "PlayerBullet" && bulletproof == false)
			{
				--health;
				if(health <= 0)
					die ();
			}
		}
		if(col.gameObject.tag == "SideGround")
		{
			die();
		}
		if(oilSlicked == true)
		{
			die ();
		}
	}

	void OnCollisionStay(Collision col)
	{
		if(col.gameObject.tag == "Ground")
		{
			grounded = true;
		}
	}

	void OnCollisionExit(Collision col)
	{
		if(col.gameObject.tag == "Ground")
		{
			grounded = false;
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(oilSlicked == false &&
		   c.gameObject.tag == "OilSlick")
		{
			oilSlicked = true;
			AudioSource.PlayClipAtPoint(tireSquealSound, this.transform.position, 0.1f);
		}

		if(c.gameObject.tag == "RoadTriggerBox")
		{
			forwardAngle = c.gameObject.transform.parent.transform.rotation.eulerAngles.y;
			currentRoad = c.gameObject.transform;
		}
	}
}
