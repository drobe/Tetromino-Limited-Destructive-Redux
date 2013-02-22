using UnityEngine;
using System.Collections;

public class TetrisPiece : MonoBehaviour
{
	public enum Identifier
	{
		I = 0,
		J = 1,
		L = 2,
		O = 3,
		S = 4,
		T = 5,
		Z = 6
	}
	
	public GameObject[] parts 	= new GameObject[4];
	public GridTile[] tiles		= new GridTile[0];
	public int[] XOffsets 		= new int[4];
	public int[] YOffsets 		= new int[4];
	
	public Identifier id		= 0;
	
	static float tileHeight = 1;
	static float tileWidth  = 1;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//updatePosition();
	}
	
	#region Piece Manipulation
	public GridTile[] updatePosition()
	{
		tiles = TetrisMode.PutInGridTiles(parts, tiles);
		return tiles;
	}
	
	void RotatePieceRight()
	{
		if (id == Identifier.O)
		{
			return;
		}
		transform.Rotate(0,0,-90);
		if (updatePosition().Length != 4)
		{
			transform.Rotate(0,0,90);
			updatePosition ();
		}
	}
	void RotatePieceLeft()
	{
		if (id == Identifier.O)
		{
			return;
		}
		transform.Rotate(0,0,90);
		if (updatePosition().Length != 4)
		{
			transform.Rotate(0,0,-90);
			updatePosition ();
		}
	}	
	bool MovePieceDown()
	{
		transform.position += Vector3.down * tileHeight;
		if (updatePosition().Length != 4)
		{
			transform.position -= Vector3.down * tileHeight;
			transform.DetachChildren();
			foreach(GameObject part in parts)
			{
				part.transform.parent = TetrisMode.GetInstance().objectPool.transform;
				part.AddComponent<Rigidbody>();
				part.rigidbody.isKinematic = true;
			}
			transform.parent = TetrisMode.GetInstance().objectMorgue.transform;
			updatePosition ();
			TetrisMode.GetInstance().CheckLines();
			TetrisMode.GetInstance().CreatePiece();
			return false;
		}
		return true;
	}
	void MovePieceLeft()
	{

		transform.position += Vector3.left * tileWidth;
		if (updatePosition().Length != 4)
		{
			transform.position -= Vector3.left * tileWidth;
			updatePosition ();
		}
	}
	void MovePieceRight()
	{
		transform.position += Vector3.right * tileWidth;
		if (updatePosition().Length != 4)
		{
			transform.position -= Vector3.right * tileWidth;
			updatePosition ();
		}
	}
	
	void DropPiece()
	{
		while(MovePieceDown())
		{//lol
		}
	}
	#endregion
}
