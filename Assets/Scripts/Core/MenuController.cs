using UnityEngine;
using System.Collections;

public class MenuController: MonoBehaviour 
{
	private Camera viewCamera;
	public Vector3 mousePos = new Vector3(0,0,0);
	
	RaycastHit	hit;
	Rigidbody targetRB;
	Ray aRay;
	
	
	// Use this for initialization
	void Start () 
	{
		viewCamera = Camera.mainCamera;
	}
	
	// Update is called once per frame
	void Update () 
	{
		mousePos = Input.mousePosition;
		aRay = viewCamera.ScreenPointToRay(mousePos);
		Vector3 rayOrigin = aRay.origin;
		rayOrigin.y -= 1;
		
		
		
		if (Physics.Raycast(aRay, out hit))
		{
			if (Input.GetMouseButton(0))
			{
				if (hit.collider.gameObject.name == "New Game" || (Input.GetKey(KeyCode.JoystickButton7) && !TetrisMode.GetInstance().isactive))
				{
					GamesceneManager.GetInstance().inMode = true;
					GamesceneManager.GetInstance().inMenu = false;
					GamesceneManager.GetInstance().inSplash = false;
					GamesceneManager.GetInstance().resetMusic = true;
				}
				else if (hit.collider.gameObject.name == "Music Credits" && !TetrisMode.GetInstance().isactive)
				{
					GamesceneManager.GetInstance().inMode = false;
					GamesceneManager.GetInstance().inMenu = false;
					GamesceneManager.GetInstance().inSplash = true;
					GamesceneManager.GetInstance().resetMusic = false;
				}
				else if (hit.collider.gameObject.name == "Back To Menu" && !TetrisMode.GetInstance().isactive)
				{
					GamesceneManager.GetInstance().inMode = true;
					GamesceneManager.GetInstance().inMenu = true;
					GamesceneManager.GetInstance().inSplash = false;
					GamesceneManager.GetInstance().resetMusic = false;
				}
				else if (hit.collider.gameObject.name == "Exit" && !TetrisMode.GetInstance().isactive)
				{
					Application.Quit();
				}
			}
		}
		
		if (Input.GetKey(KeyCode.JoystickButton7) && !TetrisMode.GetInstance().isactive)
		{
			GamesceneManager.GetInstance().inMode = true;
			GamesceneManager.GetInstance().inMenu = false;
			GamesceneManager.GetInstance().inSplash = false;
			GamesceneManager.GetInstance().resetMusic = true;
		}
		if ((Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.Escape)) && !TetrisMode.GetInstance().isactive)
		{
			print ("Goodbye");
			Application.Quit();
		}
	}
}
