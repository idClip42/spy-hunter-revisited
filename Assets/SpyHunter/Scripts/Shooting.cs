using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour 
{
	public GameObject player;
	public GameObject projectile;
	public GameObject gunShotNoise;
	public Transform barrelLeft;
	public Transform barrelRight;
	public GameObject flashLeft;
	public GameObject flashRight;
	public float bulletVel;
	public float timeDelaySec;
	float nextShot;
	bool leftGun;
	CarMoveBasic playerScript;
	PhoneControls phoneScript;

	// Use this for initialization
	void Start () 
	{
		nextShot = 0;
		leftGun = true;
		playerScript = player.GetComponent<CarMoveBasic>();
		phoneScript = player.GetComponent<PhoneControls>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		flashLeft.SetActive(false);
		flashRight.SetActive(false);
		flashLeft.transform.Rotate(new Vector3(0,0,20));
		flashRight.transform.Rotate(new Vector3(0,0,-20));

		if(playerScript.Alive == true)
			Shoot ();
	}

	void Shoot()
	{
		if(Time.time >= nextShot)
		{
			Transform gun;
			if(leftGun == true) gun = barrelLeft;
			else gun = barrelRight;
			
			if(Input.GetKey(KeyCode.X) || phoneScript.TapShooting())
			{
				GameObject bullet = (GameObject)Instantiate(
					projectile, 
					gun.position, 
					this.transform.rotation);
				Rigidbody bRb = bullet.GetComponent<Rigidbody>();
				bRb.velocity = gun.forward * bulletVel;
				
				GameObject gunSound = (GameObject)Instantiate(
					gunShotNoise,
					gun.position, 
					this.transform.rotation);

				if(leftGun == true) flashLeft.SetActive(true);
				else flashRight.SetActive(true);
			}
			leftGun = !leftGun;
			nextShot = Time.time + timeDelaySec;
		}
	}
}
