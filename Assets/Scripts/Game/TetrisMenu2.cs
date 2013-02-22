using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisMenu2 : MonoBehaviour 
{
	public string[] PieceNames = {"S","Z","I","J","L","O","T"};
	float spawn_duration = 0;
	
	List<GameObject> spawnedPieces;
	List<GameObject> piecesToDelete;
	
	public float spawn_delay;
	public int spawn_width = 5;
	public float Floor = -1200f;
	public int maxPieces = 100;
	public float borders = 100;
	// Use this for initialization
	void Awake ()
	{
		spawnedPieces = new List<GameObject>();
		piecesToDelete = new List<GameObject>();
	}
	void Start () 
	{
		/*
		for (int i = 0; i < maxPieces; i++)
		{
			CreatePiece();
		}
		float middleY = Floor + ((transform.position.y - Floor) * 0.5f);
		float heightSpan = ((transform.position.y - Floor) * 0.5f);
		float z = transform.position.z;
		float x = transform.position.x;
		foreach (GameObject go in spawnedPieces)
		{
			go.transform.position = new Vector3( 	x + Random.Range(-spawn_width + borders, spawn_width - borders),
												 	middleY + Random.Range(-heightSpan, heightSpan),
													z);
		}*/
	}
	
	// Update is called once per frame
	void Update () 
	{
		spawn_duration -= Time.deltaTime;
		if (spawn_duration <= 0.0f)
		{
			CreatePiece();
			spawn_duration = spawn_delay;
		}
		foreach (GameObject go in spawnedPieces)
		{
			if (go.transform.position.y <= Floor)
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
	
	public void CreatePiece()
	{
		if (spawnedPieces.Count >= maxPieces)
		{
			return;
		}
		
		string type = PieceNames[Random.Range(0, PieceNames.Length)];
		
		GameObject currentPiece = (GameObject)GameObject.Instantiate(Resources.Load(type), transform.position + (Vector3.right * Random.Range(-spawn_width, spawn_width)), Quaternion.identity);
		//currentPiece.transform.parent = transform;
		currentPiece.rigidbody.isKinematic = false;
		currentPiece.rigidbody.useGravity = true;
		currentPiece.rigidbody.angularVelocity = Random.onUnitSphere;
		currentPiece.rigidbody.velocity = Vector3.down;
		spawnedPieces.Add(currentPiece);
		currentPiece.layer = gameObject.layer;
	}
}
