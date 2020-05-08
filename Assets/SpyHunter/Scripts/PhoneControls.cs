using UnityEngine;
using System.Collections;

public class PhoneControls : MonoBehaviour
{

	public bool phoneControlsOn;
	public bool tiltScreen;

	public GameObject UI;

	public GameObject fireButton;
	public GameObject fireButtonUp;
	public GameObject fireButtonDown;

	public GameObject brake;
	public GameObject brakeUp;
	public GameObject brakeDown;

	public GameObject gear;
	public GameObject gearUp;
	public GameObject gearDown;

	public GameObject oilButton;
	public GameObject oilButtonUp;
	public GameObject oilButtonDown;

	bool tappedOnShift;
	public GameObject camera;

	public GameObject cubeCam;
	public Cubemap cubemap;
	public GameObject carModel;

	public GameObject oilSlickWeapon;
	OilSlicks oilSlickScript;

	public GameObject rearViewMirror;

	public GameObject logo;
	public GameObject tapToStart;
	public GameObject tapToRestart;
	public GameObject settings;
	public GameObject missions;
	public GameObject leaderboards;

	bool menu;
	public bool Menu { get { return menu; } }

	// Use this for initialization
	void Start ()
	{
		if(phoneControlsOn == false)
		{
			UI.SetActive(false);
			menu = false;
		}
		else
		{
			//BakeCubemap();
			//SetReflection();
			menu = true;
			InitializeCanvas();
			tappedOnShift = false;
			oilSlickScript = oilSlickWeapon.GetComponent<OilSlicks>();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(phoneControlsOn == true)
		{
			tapToRestart.SetActive(false);
			if(gameObject.GetComponent<CarMoveBasic>().Alive == false)
			{
				tapToRestart.SetActive(true);
				fireButton.SetActive(false);
				oilButton.SetActive(false);
				gear.SetActive(false);
				brake.SetActive(false);
				rearViewMirror.SetActive(false);
			}
			else if(menu == false)
			{
				fireButton.SetActive(true);
				oilButton.SetActive(true);
				gear.SetActive(true);
				brake.SetActive(true);
				rearViewMirror.SetActive(true);

				logo.SetActive(false);
				tapToStart.SetActive(false);
				settings.SetActive(false);
				missions.SetActive(false);
				leaderboards.SetActive(false);

				if(tiltScreen) TiltScreen();

				if(oilSlickScript.HasOilSlicks() == true) oilButton.SetActive(true);
				else oilButton.SetActive(false);
			}
			else
			{
				fireButton.SetActive(false);
				oilButton.SetActive(false);
				gear.SetActive(false);
				brake.SetActive(false);
				rearViewMirror.SetActive(false);
				
				logo.SetActive(true);
				tapToStart.SetActive(true);
				settings.SetActive(true);
				missions.SetActive(true);
				leaderboards.SetActive(true);

				TapMenu();
			}
		}
		
		if(Input.GetKeyDown(KeyCode.E))
		{
			//BakeCubemap();
		}
	}

	void BakeCubemap()
	{
		cubeCam.SetActive(true);
		cubeCam.GetComponent<Camera>().RenderToCubemap(cubemap);
		cubeCam.SetActive(false);
	}

	void SetReflection()
	{
		ReflectionProbe r = carModel.GetComponent<ReflectionProbe>();
		r.bakedTexture = cubemap;
		r.customBakedTexture = cubemap;
	}

	void InitializeCanvas()
	{
		float fBSize = Screen.height/4;
		float fBOffset = Screen.height/10;
		fireButton.transform.localScale *= fBSize/100;				
		fireButton.transform.position = new Vector3(
			Screen.width-Screen.height/4 - fBOffset,
			fBOffset,
			0);

		float brakeSize = Screen.height/4;
		float brakeOffset = Screen.height/15;
		brake.transform.localScale *= brakeSize/100;				
		brake.transform.position = new Vector3(
			brakeOffset,
			brakeOffset,
			0);
		
		float gearSize = Screen.height/4;
		float gearOffset = Screen.height/15;
		gear.transform.localScale *= gearSize/100;				
		gear.transform.position = new Vector3(
			Screen.width/5 + gearOffset,
			gearOffset,
			0);

		float oilBSize = Screen.height/5;
		float oilBOffset = Screen.height/10;
		oilButton.transform.localScale *= oilBSize/100;				
		oilButton.transform.position = new Vector3(
			Screen.width-Screen.height*3/6 - oilBOffset,
			oilBOffset/2,
			0);

		logo.transform.localScale = new Vector3(
			Screen.width * 0.8f / 100f,
			Screen.height * 0.2f / 100f,
			0);
		logo.transform.position = new Vector3(
			Screen.width*0.1f,
			Screen.height*0.6f,
			0);

		tapToStart.transform.localScale = new Vector3(
			Screen.width / 3f / 100f,
			Screen.height / 10f / 100f,
			0);
		tapToStart.transform.position = new Vector3(
			Screen.width/3f,
			Screen.height/2f,
			0);

		tapToRestart.transform.localScale = new Vector3(
			Screen.width / 3f / 100f,
			Screen.height / 10f / 100f,
			0);
		tapToRestart.transform.position = new Vector3(
			Screen.width/3f,
			Screen.height/2f,
			0);

		settings.transform.localScale = new Vector3(
			Screen.height / 6f / 100f,
			Screen.height / 6f / 100f,
			0);
		settings.transform.position = new Vector3(
			Screen.width * 9f / 10f,
			Screen.height * 6.4f / 8f,
			0);

		missions.transform.localScale = new Vector3(
			Screen.height / 6f / 100f,
			Screen.height / 6f / 100f,
			0);
		missions.transform.position = new Vector3(
			Screen.width * 9f / 10f,
			Screen.height * 0.2f / 8f,
			0);

		leaderboards.transform.localScale = new Vector3(
			Screen.height / 6f / 100f,
			Screen.height / 6f / 100f,
			0);
		leaderboards.transform.position = new Vector3(
			Screen.width * 0.2f / 10f,
			Screen.height * 6.4f / 8f,
			0);

	}

	bool CheckIfTouchInBox(Vector2 t, Transform b)
	{
		if(t.x > b.position.x &&
		   t.x < b.position.x + b.localScale.x * 100 &&
		   t.y > b.position.y &&
		   t.y < b.position.y + b.localScale.y * 100)
			return true;

		return false;
	}

	public void TapMenu()
	{
		for(int n = 0; n < Input.touchCount; ++n)
		{
			if(CheckIfTouchInBox(Input.touches[n].position, settings.transform) == true)
			{

			}
			else if(CheckIfTouchInBox(Input.touches[n].position, missions.transform) == true)
			{
				
			}
			else if(CheckIfTouchInBox(Input.touches[n].position, leaderboards.transform) == true)
			{
				
			}
			else
			{
				menu = false;
			}
		}

		// Press enter to exit menu to game
		if(Input.GetKeyDown(KeyCode.Return))
		{
			menu = false;
		}
	}

	public bool TapRestart(bool alive)
	{
		if(phoneControlsOn == true && 
		   alive == false && 
		   Input.touchCount > 0)
			return true;
		else return false;
	}

	public float PhoneSteering()
	{
		if(phoneControlsOn == true)
		{
			return Input.acceleration.x/0.3f;
		}
		return 0;
	}

	void TiltScreen()
	{
		if(phoneControlsOn == true)
		{
			Vector3 newRot = camera.transform.rotation.eulerAngles;
			newRot.z = -(Input.acceleration.x)/2*180;
			camera.transform.rotation = Quaternion.Euler(newRot);
		}
	}

	public bool TapShooting()
	{
		if(phoneControlsOn == true)
		{
			for(int n = 0; n < Input.touchCount; ++n)
			{
				if(CheckIfTouchInBox(Input.touches[n].position, fireButton.transform) == true)
				{
					fireButtonUp.SetActive(false);
					fireButtonDown.SetActive(true);
					return true;
				}
			}
			fireButtonUp.SetActive(true);
			fireButtonDown.SetActive(false);
		}
		return false;
	}

	public bool TapSlicks()
	{
		if(phoneControlsOn == true)
		{
			for(int n = 0; n < Input.touchCount; ++n)
			{
				if(CheckIfTouchInBox(Input.touches[n].position, oilButton.transform) == true)
				{
					oilButtonUp.SetActive(false);
					oilButtonDown.SetActive(true);
					return true;
				}
			}
			oilButtonUp.SetActive(true);
			oilButtonDown.SetActive(false);
		}
		return false;
	}

	public bool TapBraking()
	{
		if(phoneControlsOn == true)
		{
			for(int n = 0; n < Input.touchCount; ++n)
			{
				if(CheckIfTouchInBox(Input.touches[n].position, brake.transform) == true)
				{
					brakeUp.SetActive(false);
					brakeDown.SetActive(true);
					return true;
				}
			}
			brakeUp.SetActive(true);
			brakeDown.SetActive(false);
		}
		return false;
	}

	public bool TapGearShift()
	{if(phoneControlsOn == true)
		{
			bool foundOne = false;
			for(int n = 0; n < Input.touchCount; ++n)
			{
				if(CheckIfTouchInBox(Input.touches[n].position, gear.transform) == true)
				{
					foundOne = true;
					if(tappedOnShift == false)
					{
						gearDown.SetActive(!gearDown.activeSelf);
						gearUp.SetActive(!gearUp.activeSelf);
						tappedOnShift = true;
						return true;
					}
				}
			}
			//gearDown.SetActive(true);
			//gearUp.SetActive(false);
			if(foundOne == false) tappedOnShift = false;
		}
		return false;
	}
}
