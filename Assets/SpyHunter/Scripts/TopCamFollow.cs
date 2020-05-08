using UnityEngine;
using System.Collections;

public class TopCamFollow : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = this.transform.position;
		pos.x = target.position.x + 10;
		pos.z = target.position.z + 10;
		this.transform.position = pos;

		this.transform.rotation = Quaternion.Euler(
			this.transform.rotation.eulerAngles.x,
			target.transform.rotation.eulerAngles.y,
			this.transform.rotation.eulerAngles.z);
	}
}
