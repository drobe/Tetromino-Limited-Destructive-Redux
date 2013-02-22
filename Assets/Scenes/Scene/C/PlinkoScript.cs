using UnityEngine;
using System.Collections;

public class PlinkoScript : MonoBehaviour {
	
	public Rect bounds;
	public Rect pieceBounds;
	public GameObject[] pieces = new GameObject[0];
	
	
	// Use this for initialization
	void Start ()
	{
		bounds.center = transform.position;
		
		int rows 	= Mathf.FloorToInt(bounds.height / pieceBounds.height);
		int columns	= Mathf.FloorToInt(bounds.width / pieceBounds.width);
		
		for (int x = 0; x < rows; x ++)
		{
			for (int y = 0; y < columns; y ++)
			{
				Vector3 position = new Vector3 (
									bounds.x + (y * pieceBounds.width) + (pieceBounds.width * 0.5f),
									bounds.y + (x * pieceBounds.height) + (pieceBounds.height * 0.5f),
									transform.position.z);
				if (pieces.Length == 0)
				{
					continue;
				}
				GameObject go = pieces[Random.Range(0,pieces.Length)];
				Instantiate (go, position, Quaternion.identity);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
