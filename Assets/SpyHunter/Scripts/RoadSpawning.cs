using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadSpawning : MonoBehaviour {

	public GameObject hillRoad;
	public GameObject hillRoadRight;
	public GameObject hillRoadLeft;
	public GameObject tunnelRoad;
	public GameObject player;
	public GameObject roadCollider;
	public GameObject[] enemies;
	public GameObject[] civilians;
	public int civCarsPerSpace;

	public double roadLength;
	//Vector3 prevLoc;
	int previousRoadObj;
	List<RoadObject> roadPieceList;

	float roadAngle;
	bool curvedPreviously;

	void Start ()
	{
		roadCollider.transform.localScale = new Vector3(
			int.MaxValue, 
			roadCollider.transform.localScale.y,
			int.MaxValue);

		roadAngle = 0;

		roadPieceList = new List<RoadObject>();
		//prevLoc = player.transform.position;
		for(int n = 0; n < 20; ++n)
		{
			roadPieceList.Add (new RoadObject(tunnelRoad, new Vector3(
				(float)((n-9)*roadLength), 
				-0.5f, 
				0),
			    FindRoadDirectionFromAngle(roadAngle)));
			InstRoad(n);
		}

		previousRoadObj = 9;

		curvedPreviously = false;
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			ShiftSceneBackToCenter(roadPieceList[0].Position);
		}
	}

	Vector3 FindRoadDirectionFromAngle(float angle)
	{
		return new Vector3(Mathf.Cos(Mathf.Deg2Rad*angle),0,-Mathf.Sin(Mathf.Deg2Rad*angle));
	}
	float findAngleFromRoadDirection(Vector3 dir)
	{
		return Mathf.Rad2Deg * Mathf.Atan(dir.z/dir.x);
	}

	public void updatePlayerPosition(GameObject playerPiece)
	{
		int index = int.MaxValue;

		for(int n = 0; n < roadPieceList.Count; ++n)
		{
			if(roadPieceList[n].RoadPart == playerPiece)
			{
				index = n;
				n = roadPieceList.Count;
			}
		}

		if(index != int.MaxValue)
		{
			while(index > previousRoadObj)
			{
				Vector3 nextLocation = roadPieceList[roadPieceList.Count-1].Position + FindRoadDirectionFromAngle(roadAngle) * (float)roadLength;
				float newLoc = nextLocation.x;
				
				roadPieceList[0].KillRoad();
				roadPieceList.RemoveAt (0);
				
				if(curvedPreviously == false)
				{
					int max = 5;
					int selection = Mathf.CeilToInt(Random.value * max);
					if(selection == max)
					{
						roadAngle += 10;
						roadPieceList.Add (new RoadObject(
							hillRoadRight, 
							nextLocation,
							FindRoadDirectionFromAngle(roadAngle)));
						roadPieceList[roadPieceList.Count-2].KillRoad();
						curvedPreviously = true;
					}
					else if(selection == max-1)
					{
						roadAngle -= 10;
						roadPieceList.Add (new RoadObject(
							hillRoadLeft, 
							nextLocation,
							FindRoadDirectionFromAngle(roadAngle)));
						roadPieceList[roadPieceList.Count-2].KillRoad();
						curvedPreviously = true;
					}
					else
					{
						roadPieceList.Add (new RoadObject(
							hillRoad, 
							nextLocation,
							FindRoadDirectionFromAngle(roadAngle)));
					}
				}
				else
				{
					curvedPreviously = false;
					roadPieceList.Add (new RoadObject(
						hillRoad, 
						nextLocation,
						FindRoadDirectionFromAngle(roadAngle)));
				}
				InstRoad(roadPieceList.Count-1);
				AddCars(newLoc);
				
				--index;
			}
		}
	}

	void InstRoad(int index)
	{
		GameObject newRoad = (GameObject) Instantiate(
			roadPieceList[index].Mesh,
			roadPieceList[index].Position,
			Quaternion.Euler(0,roadAngle,0));
		roadPieceList[index].RoadPart = newRoad;
	}

	void AddCars(float xVal)
	{
		int max = 5;
		int r = Mathf.CeilToInt(Random.value * max);

		float bound = 15;
		float zCenter = 0;
		float zMin = zCenter - hillRoad.transform.localScale.z/2 + bound;
		float zWidth = hillRoad.transform.localScale.z - 2 * bound;

		float angle = 0;
		Vector3 point;

		if(r == max && enemies.Length > 0)
		{
			float zPos = Random.Range(0, zWidth) + zMin;

			Vector3 placementPos;
			int aheadOrBehind = Mathf.RoundToInt(Random.value);
			if(aheadOrBehind == 0)
			{
//				placementPos = roadPieceList[roadPieceList.Count - 1].Position + new Vector3(0,1,zPos);
//				angle = roadPieceList[roadPieceList.Count - 1].RoadPart.transform.rotation.eulerAngles.y;
//				point = roadPieceList[roadPieceList.Count - 1].Position;
				placementPos = roadPieceList[roadPieceList.Count - 2].Position + new Vector3(0,1,zPos);
				angle = roadPieceList[roadPieceList.Count - 2].RoadPart.transform.rotation.eulerAngles.y;
				point = roadPieceList[roadPieceList.Count - 2].Position;
			}
			else
			{
				placementPos = roadPieceList[8].Position + new Vector3(0,1,zPos);
				Vector3 d = roadPieceList[8].Direction;
				angle = Mathf.Rad2Deg * Mathf.Atan(d.z/-d.x);
				point = roadPieceList[8].Position;
			}

			int selection = (int)Mathf.Floor(Random.Range(0, enemies.Length));
			GameObject newEnemy = (GameObject) Instantiate(
				enemies[selection],
				placementPos,
				Quaternion.Euler(0,90,0));

			newEnemy.transform.RotateAround(point, Vector3.up, -angle);
			newEnemy.GetComponent<EnemyBehavior>().target = player;
			newEnemy.GetComponent<EnemyBehavior>().ForwardAngle = angle;
		}
		else if(civilians.Length > 0)
		{
			int num = Mathf.CeilToInt(Random.value * civCarsPerSpace);
			for(int n = 0; n < num; ++n)
			{
				float zPos = Random.Range (0, zWidth) + zMin;
				float xPos = xVal;
//
//				Vector3 placementPos = roadPieceList[roadPieceList.Count - 1].Position + new Vector3(0,1,zPos);
//				angle = findAngleFromRoadDirection(roadPieceList[roadPieceList.Count - 1].Direction);
//				point = roadPieceList[roadPieceList.Count - 1].Position;
				Vector3 placementPos = roadPieceList[roadPieceList.Count - 2].Position + new Vector3(0,1,zPos);
				angle = findAngleFromRoadDirection(roadPieceList[roadPieceList.Count - 2].Direction);
				point = roadPieceList[roadPieceList.Count - 2].Position;

				int selection = (int)Mathf.Floor(Random.Range(0, civilians.Length));
				GameObject newCiv = (GameObject) Instantiate(
					civilians[selection],
					placementPos,
					Quaternion.Euler(0,90,0));
				newCiv.transform.RotateAround(point, Vector3.up, -angle);
				newCiv.GetComponent<Civilian>().player = player;
			}
		}
	}

	void ShiftSceneBackToCenter(Vector3 offset)
	{
		GameObject[] objList = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
		List<GameObject> rootObjList = new List<GameObject>();
		for(int n = 0; n < objList.Length; ++n)
		{
			if(objList[n].transform.parent == null)
			{
				rootObjList.Add(objList[n]);
			}
		}
		foreach(GameObject obj in rootObjList)
		{
			if(obj != this.gameObject &&
			   obj != GameObject.Find("UI"))
				obj.transform.position -= offset;
			if(obj == this.gameObject)
				obj.transform.position = new Vector3(
					obj.transform.position.x,
					(float)(obj.transform.position.y-offset.y),
					obj.transform.position.z);
		}
	}
}



public class RoadObject
{
	GameObject mesh;
	Vector3 position;
	Vector3 direction;

	GameObject road;

	public RoadObject(GameObject m, Vector3 pos, Vector3 dir)
	{
		mesh = m;
		position = pos;
		direction = dir;
	}

	public GameObject Mesh
	{
		get
		{
			return mesh;
		}
	}

	public Vector3 Position
	{
		get
		{
			if(road != null)
				position = road.transform.position;
			return position;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return direction;
		}
	}

	public GameObject RoadPart
	{
		get
		{
			return road;
		}

		set
		{
			road = value;
		}
	}

	public void KillRoad()
	{
		GameObject.Destroy(road, 0.1f);
	}
}
