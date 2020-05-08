using UnityEngine;
using System.Collections;

public class FlipCam : MonoBehaviour 
{
	Camera camera;

	// Use this for initialization
	void Start () 
	{
		camera = this.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnPreCull()
	{
		camera.ResetWorldToCameraMatrix ();
		camera.ResetProjectionMatrix ();
		camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3 (-1, 1, 1));
	}

	void OnPreRender()
	{
		GL.SetRevertBackfacing (true);
	}

	void OnPostRender()
	{
		GL.SetRevertBackfacing (false);
	}
}
