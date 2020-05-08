using UnityEngine;
using System.Collections;

public class ProjectileMove : MonoBehaviour {

	public GameObject sparks;

	// Use this for initialization
	void Start ()
	{
		Destroy (this.gameObject, 1);
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnCollisionEnter (Collision col)
	{
		Quaternion rotationOfSparks = Quaternion.Inverse(this.transform.rotation);
		GameObject spark = (GameObject)Instantiate(sparks, this.transform.position, rotationOfSparks);
		Destroy (this.gameObject);
	}
}
