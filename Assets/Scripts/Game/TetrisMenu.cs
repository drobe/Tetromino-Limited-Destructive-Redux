using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisMenu : MonoBehaviour 
{
	public string[] PieceNames = {"S","Z","I","J","L","O","T"};
	GamesceneManager GSM = null;
	float spawn_duration = 0;
	
	List<GameObject> spawnedPieces;
	List<GameObject> piecesToDelete;
	
	public float spawn_delay;
	public int spawn_width = 5;
	// Use this for initialization
	void Awake ()
	{
		spawnedPieces = new List<GameObject>();
		piecesToDelete = new List<GameObject>();
	}
	void Start () 
	{
		GSM = GamesceneManager.GetInstance();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GSM.inMenu)
		{
			spawn_duration -= Time.deltaTime;
			if (spawn_duration <= 0.0f)
			{
				CreatePiece();
				spawn_duration = spawn_delay;
			}
		}
		foreach (GameObject go in spawnedPieces)
		{
			if (go.transform.position.y <= -40.0f)
			{
				piecesToDelete.Add(go);
				GameObject.DestroyImmediate(go);
			}
		}
		foreach (GameObject go in piecesToDelete)
		{
			spawnedPieces.Remove(go);
		}
		
	}
	
	public void Activate()
	{
		MusicController.GetInstance();
	}
	
	public void CreatePiece()
	{
		string type = PieceNames[Random.Range(0, PieceNames.Length)];
		
		GameObject currentPiece = (GameObject)GameObject.Instantiate(Resources.Load(type), transform.position + (Vector3.right * Random.Range(-spawn_width, spawn_width)), Quaternion.identity);
		//currentPiece.transform.parent = transform;
		currentPiece.rigidbody.isKinematic = false;
		currentPiece.rigidbody.useGravity = true;
		currentPiece.rigidbody.angularVelocity = Random.onUnitSphere;
		currentPiece.rigidbody.velocity = Vector3.down;
		spawnedPieces.Add(currentPiece);
	}
}
