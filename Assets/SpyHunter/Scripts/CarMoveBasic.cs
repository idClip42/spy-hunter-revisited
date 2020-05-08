using UnityEngine;
using System.Collections;
using SpyHunter.Game;

public class CarMoveBasic : MonoBehaviour {

	public GameObject roadSpawn;
	public bool autoDrive;
	public bool limitTurnAngle;
	public bool lerpCenterRot;
	public bool allowReverse;
	public float accelVal;
	public float topSpeed;
	public float topSpeedInHighGear;
	public float turnSpeed;
	public GameObject cam;
	public GameObject camDest;
	public GameObject menuCamDest;
	public GameObject flame;
	public GameObject sparks;
	public bool freezeXInAir;
	public AudioClip crashSound;

	Vector3 vel;
	bool grounded;
	bool alive;
	Rigidbody rb;
	float initialCarAngle;
	float forwardAngle;
	RigidbodyConstraints startConstraints;
	bool highGear;

	PhoneControls phoneScript;
	RoadSpawning roadScript;

    float verticalAxis = 0;
    float horizAxis = 0;

    public bool Alive { get { return alive; } }

    // Use this for initialization
    void Start ()
	{
		forwardAngle = 90;
		grounded = true;
		alive = true;
		rb = this.GetComponent<Rigidbody>();
		initialCarAngle = this.transform.rotation.eulerAngles.y;
		startConstraints = rb.constraints;
		highGear = false;

		phoneScript = this.gameObject.GetComponent<PhoneControls>();
		roadScript = roadSpawn.GetComponent<RoadSpawning>();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || phoneScript.TapGearShift()) highGear = !highGear;

        verticalAxis = Input.GetAxis("Vertical");
        horizAxis = Input.GetAxis("Horizontal");

        spinOutTest();
    }

    void FixedUpdate ()
	{
		if(phoneScript.Menu == false)
		{
			if(alive == true)
			{
				if(grounded == true)
				{
					AccelForward();
					Turning ();
					if(lerpCenterRot == true)
						AutoCenterRotation(forwardAngle);
				}
				constrainRotation(60, forwardAngle);
				moveCamera(camDest.transform);
				UpdateScore();

				//AddDownwardForce(20);
			
			}
			else 
			{
				moveCameraSlow();
				flame.transform.rotation = Quaternion.Euler(Vector3.up);
			}

		}
		else
		{
			moveCamera(menuCamDest.transform);
		}
	}

	void AccelForward()
	{
		float currentTopSpeed = topSpeed;
		if(highGear == true)
			currentTopSpeed = topSpeedInHighGear;

		if(phoneScript.TapBraking() == true && 
		   rb.velocity.magnitude <= currentTopSpeed)
		{
			rb.AddRelativeForce(
				new Vector3(0f,0f,
			            accelVal * -1.5f), 
				ForceMode.Acceleration);
		}

		if (verticalAxis != 0 && 
		    rb.velocity.magnitude <= currentTopSpeed)
		{
			rb.AddRelativeForce(
				new Vector3(0f,0f,
			            accelVal * verticalAxis), 
				ForceMode.Acceleration);
		}

		else if (autoDrive == true && rb.velocity.magnitude <= currentTopSpeed)
		{
			rb.AddRelativeForce(
				new Vector3(0f,0f,
			            accelVal), 
				ForceMode.Acceleration);
		}

		// Slow from high gear
		if(highGear == false &&
		   rb.velocity.magnitude > topSpeed + 10)
		{
			int mult = 2;
			if(rb.velocity.x > 0) mult = -2;
			
			rb.AddRelativeForce(
				new Vector3(0f,0f,
			            accelVal * mult), 
				ForceMode.Acceleration);
		}

		if(allowReverse == false)
		{
			// Prevent backing up
			if(Vector3.Dot (this.transform.forward, rb.velocity) < 0)
			{
				rb.velocity = Vector3.zero;
			}
		}
	}
	
	void Turning()
	{
		int forwardBack = 1;

		float rotationValue = horizAxis;
		rotationValue += phoneScript.PhoneSteering();

		float zVel = transform.InverseTransformDirection(rb.velocity).z;
			
		if(zVel >= 0) forwardBack = 1;
		else forwardBack = -1;

		float rotateSpeed = rotationValue * forwardBack * turnSpeed * rb.velocity.magnitude/10;
		Vector3 rotateAmount = new Vector3 (0f, rotateSpeed, 0f);
		this.transform.Rotate(rotateAmount);
		
		if(forwardBack == 1)
		{
			// Turns velocity
			rb.velocity = transform.forward * rb.velocity.magnitude;
			rb.velocity = new Vector3(
				transform.forward.x * rb.velocity.magnitude,
				rb.velocity.y,
				transform.forward.z * rb.velocity.magnitude);
		}
	}

	void constrainRotation(float angle, float forwardAngle)
	{
		if(limitTurnAngle)
		{
			float leftBound = forwardAngle - angle;
			float rightBound = forwardAngle + angle;
			Vector3 rotation = this.transform.rotation.eulerAngles;
			if(rotation.y < leftBound) rotation.y = leftBound;
			if(rotation.y > rightBound) rotation.y = rightBound;
			this.transform.rotation = Quaternion.Euler(rotation);
		}
	}

	void AutoCenterRotation(float forwardAngle)
	{
		this.transform.rotation = Quaternion.Lerp(
			this.transform.rotation, 
			Quaternion.Euler(0,forwardAngle,0),
			0.05f);
	}

	void spinOutTest()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			die();
			rb.AddTorque(0,100000000,0);
			rb.velocity = rb.velocity + new Vector3(0,0,-40);
		}
	}

	void AddDownwardForce(float magnitude)
	{
		Vector3 f;
		//f = Vector3.down * magnitude;
		f = this.transform.up * -magnitude;
		rb.AddForce(f);
	}

	void die()
	{
		alive = false;
		rb.constraints = RigidbodyConstraints.None;
		flame.SetActive(true);
	}

	void moveCamera(Transform dest)
	{
		cam.transform.position = Vector3.Lerp(
			cam.transform.position, 
			dest.position, 
			0.2f);

		cam.transform.rotation = Quaternion.Lerp(
			cam.transform.rotation, 
			dest.rotation, 
			0.08f);//8

		// Keep cam aboveground
		if(cam.transform.position.y < 2) cam.transform.position = new Vector3(
			cam.transform.position.x,
			2,
			cam.transform.position.z);
	}

	void moveCameraSlow()
	{
		cam.transform.position = Vector3.Lerp(
			cam.transform.position, 
			camDest.transform.position, 
			0.001f);

		cam.transform.LookAt(this.transform.position);
		
		// Keep cam aboveground
		if(cam.transform.position.y < 2) cam.transform.position = new Vector3(
			cam.transform.position.x,
			2,
			cam.transform.position.z);
	}

	void UpdateScore()
	{
        /*
		if(this.transform.position.x > score)
			score = (int)this.transform.position.x;
		//*/
        GameManager.instance.AddToScore(Mathf.RoundToInt(rb.velocity.magnitude * Time.deltaTime));
	}



	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ground" || col.gameObject.tag == "SideGround")
		{
			grounded = true;
			if(freezeXInAir) rb.constraints = startConstraints;
		}
		if(col.gameObject.tag == "SideGround")
		{
			die ();
		}

		if(col.relativeVelocity.magnitude > 10 &&
		   col.gameObject.tag != "Ground")
		{
			AudioSource.PlayClipAtPoint(crashSound, this.transform.position, 0.05f);
			GameObject spark = (GameObject)Instantiate(
				sparks,
				col.contacts[0].point, 
				Quaternion.identity);
			spark.transform.parent = this.transform;
			//spark.GetComponent<EllipsoidParticleEmitter>().worldVelocity = rb.velocity;
		}
		if(col.relativeVelocity.magnitude > 75 
		   && col.gameObject.tag != "Ground"
		   )
		{
			die ();
		}
	}

	void OnCollisionStay(Collision col)
	{
		if(col.gameObject.tag == "Ground" || col.gameObject.tag == "SideGround")
		{
			grounded = true;
			if(freezeXInAir) rb.constraints = startConstraints;
		}
	}

	void OnCollisionExit(Collision col)
	{
		if(col.gameObject.tag == "Ground" || col.gameObject.tag == "SideGround")
		{
			grounded = false;
			if(freezeXInAir) rb.constraints = startConstraints | RigidbodyConstraints.FreezeRotationX;
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.tag == "RoadTriggerBox")
		{
			//score += 100;
			roadScript.updatePlayerPosition(c.gameObject.transform.parent.gameObject);
			forwardAngle = c.gameObject.transform.parent.transform.rotation.eulerAngles.y + 90;
		}
	}
}
