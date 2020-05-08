using UnityEngine;
using System.Collections;

public class OilSlicks : MonoBehaviour
{
	public GameObject oilSlicks;
	public GameObject player;
	public GameObject oilPour;
	public float timeDelaySec;
	public float maxOilSlicks;

	public GameObject camPos;
	public Vector3 shiftCamPos;
	public bool shiftCamera;
	Vector3 origCamPos;

	public Material pouringMaterial;

	float nextShot;
	bool reverseImg;
	CarMoveBasic playerScript;
	PhoneControls phoneScript;

	float currentOilSlicks;

	public bool limitOilSlicks;


	// Use this for initialization
	void Start ()
	{
		nextShot = 0;
		reverseImg = false;
		playerScript = player.GetComponent<CarMoveBasic>();
		phoneScript = player.GetComponent<PhoneControls>();

		origCamPos = camPos.transform.localPosition;


		currentOilSlicks = maxOilSlicks;

		if(shiftCamera == false) shiftCamPos = Vector3.zero;

		pouringMaterial.SetTextureOffset("_MainTex", Vector2.zero);
		pouringMaterial.SetTextureOffset("_BumpMap", Vector2.zero);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		oilPour.SetActive(false);

		if(playerScript.Alive == true)
		{
			int add = 0;
			if(reverseImg) add = 180;
			if(Time.time >= nextShot)
			{
				if(Input.GetKey(KeyCode.Z) || phoneScript.TapSlicks())
				{
					if(currentOilSlicks > 0)
					{
						oilPour.SetActive(true);

						GameObject slicks = (GameObject)Instantiate(
							oilSlicks, 
							new Vector3(this.transform.position.x, 0, this.transform.position.z), 
							Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + add, 0));
						Destroy(slicks, 2);

						camPos.transform.localPosition = Vector3.Lerp (camPos.transform.localPosition, origCamPos + shiftCamPos, 0.03f);

						if(limitOilSlicks == true) currentOilSlicks--;
					}

					AnimatePouring();
				}
				else
				{
					camPos.transform.localPosition = Vector3.Lerp(camPos.transform.localPosition, origCamPos, 0.03f);
					
					pouringMaterial.SetTextureOffset("_MainTex", Vector2.zero);
					pouringMaterial.SetTextureOffset("_BumpMap", Vector2.zero);
				}

				nextShot = Time.time + timeDelaySec;
				reverseImg = !reverseImg;
			}
		}
	}

	void AnimatePouring()
	{
		Vector2 o = pouringMaterial.GetTextureOffset("_MainTex");
		o.y += 0.1f;
		pouringMaterial.SetTextureOffset("_MainTex", o);
		pouringMaterial.SetTextureOffset("_BumpMap", o);
	}

	public bool HasOilSlicks()
	{
		if(currentOilSlicks > 0) return true;
		else return false;
	}
}
