using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTile
{
	public Vector3 worldPos = new Vector3();
	[SerializeField]
	private int[] index = new int[2];
	
	public GridTile(int x, int y)
	{
		index[0] = x;
		index[1] = y;
	}
	
	public int x
	{	get
		{return index[0];}
		set
		{index[0] = value;}}
	
	public int y
	{	get
		{return index[1];}
		set
		{index[1] = value;}}
	
	public GameObject contents = null;
}

public class TetrisMode : MonoBehaviour 
{
	public string[] PieceNames = {"S","Z","I","J","L","O","T"};
	public int boardHeight 	= 8;
	public int boardWidth 	= 8;
	public float tileHeight = 1;
	public float tileWidth  = 1;
	public float dropDelay	= 1.0f;
	public float defaultDropDelay = 1.0f;
	public float repeatDelay= 0.15f;
	private float repeatDuration = 0.0f;
	private float dropDuration = 0.0f;
	public GridTile[,] Grid;
	public GameObject[,] tiles;
	public GameObject currentPiece = null;
	public GameObject nextPiece = null;
	public GameObject objectPool = null;
	public GameObject objectMorgue = null;
	public GameObject objectMorgueFalling = null;
	public GameObject boardObject = null;
	public GameObject piecesObject = null;
	public GameObject linesText = null;
	public GameObject timeText = null;
	public GameObject levelText = null;
	public GameObject nextPieceZone = null;
	
	
	public float seconds = 0.0f;
	public int lines = 0;
	public int levels = 0;
	public int linesPerLevel = 10;
	
	private bool started = false;
	private bool paused = false;
	
	public bool isactive = false;
	public bool dying = false;
	public float dieTimer = 0.0f;
	
	public float Explosiveness = 5.0f;
	
	static TetrisMode Instance = null;
	static public TetrisMode GetInstance()
	{
		return Instance;
	}
	
	public Rect boundaries;
	
	void Awake ()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start () 
	{
		Grid = new GridTile[boardWidth, boardHeight];
		tiles = new GameObject[boardWidth, boardHeight];
		
		for (int y = 0; y < boardHeight; y++)
		{
			for (int x = 0; x < boardWidth; x++)
			{
				GridTile gt = new GridTile(x,y);
				gt.worldPos = new Vector3 ((gt.x - (boardWidth * 0.5f))* tileWidth,(gt.y - (boardHeight * 0.5f))* tileHeight,0f);
				Grid[x,y] = gt;
				
				tiles[x,y] = ((GameObject)GameObject.Instantiate(Resources.Load ("TileMarker"),gt.worldPos + (Vector3.forward * tileWidth),Quaternion.identity));
				tiles[x,y].transform.parent = boardObject.transform;
				if (y >= boardHeight - 4)
				{
					tiles[x,y].renderer.material.color = new Color(1.0f,0.0f,0.0f,0.3f);
				}
			}
		}
		
		boundaries = new Rect(Grid[0,0].worldPos.x,Grid[0,0].worldPos.y, tileWidth * boardWidth, tileHeight * boardHeight);
		
		nextPieceZone.transform.position = new Vector3(boundaries.xMax + (tileWidth * 2),boundaries.yMax,0);
		linesText.SendMessage("SetText","0 Lines",SendMessageOptions.DontRequireReceiver);
		timeText.SendMessage("SetText","0s",SendMessageOptions.DontRequireReceiver);
		levelText.SendMessage("SetText","Level 0",SendMessageOptions.DontRequireReceiver);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!started && isactive)
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
			{
				string type = PieceNames[Random.Range(0, PieceNames.Length)];
				nextPiece = (GameObject)GameObject.Instantiate(Resources.Load(type), nextPieceZone.transform.position, Quaternion.identity);
				CreatePiece();
				started = true;
				return;
			}
		}
		if (started && isactive)
		{
			if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown(KeyCode.JoystickButton6))
			{
				paused = !paused;
				return;
			}
		}
		if (started && !paused && !dying)
		{
			float vertical = 0.0f;
			float horizontal = 0.0f;
			
			repeatDuration -= Time.deltaTime;
			if (repeatDuration <= 0.0f)
			{
				vertical = Input.GetAxis("JoystickVertical");
				horizontal = Input.GetAxis("JoystickHorizontal");
			}
			
			seconds+= Time.deltaTime;
			timeText.SendMessage("SetText",Mathf.CeilToInt(seconds).ToString() + "s",SendMessageOptions.DontRequireReceiver);
			
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
			{
				DropPiece ();
				dropDuration = dropDelay;
			}
			
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.JoystickButton3))
			{
				RotatePieceRight();
				dropDuration = dropDelay; // Easy spin
			}
			if (Input.GetKeyDown(KeyCode.JoystickButton4))
			{
				RotatePieceLeft ();
				dropDuration = dropDelay; // Easy spin
			}
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || horizontal < -0.5f)
			{
				MovePieceLeft ();
				repeatDuration = repeatDelay;
			}
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || horizontal > 0.5f)
			{
				MovePieceRight ();
				repeatDuration = repeatDelay;
			}
			if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || vertical > 0.5f)
			{
				MovePieceDown ();
				repeatDuration = repeatDelay;
			}
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
			{
				Die();
			}
			
			dropDuration -= Time.deltaTime;
			if (dropDuration <= 0.0f)
			{
				MovePieceDown ();
				dropDuration = dropDelay;
			}
			
			/*
			for (int y = 0; y < boardHeight; y++)
			{
				for (int x = 0; x < boardWidth; x++)
				{
					if (Grid[x,y].contents != null)
					{
						Debug.DrawLine(Grid[x,y].worldPos, Grid[x,y].contents.transform.position, Color.red, Time.deltaTime);
						tiles[x,y].renderer.material.color = Color.red;
					}
					else
					{
						tiles[x,y].renderer.material.color = Color.blue;
					}
				}
			}
			*/
		}
		if (dying)
		{
			dieTimer-= Time.deltaTime;
			if (dieTimer <= 0.0f)
			{
				dying = false;
				started = false;
				paused = false;
				GamesceneManager.GetInstance().inMenu = true;
				GamesceneManager.GetInstance().inSplash = false;
				GamesceneManager.GetInstance().inMode = false;
				GamesceneManager.GetInstance().resetMusic = true;
				
				isactive = false;
				print ("goBack!");
				
				if (currentPiece!= null)
				{
					GameObject.DestroyImmediate(currentPiece);
				}
				currentPiece = null;
				
				if (nextPiece!= null)
				{
					GameObject.DestroyImmediate(nextPiece);
				}
				nextPiece = null;
			}
		}
		EmptyMorgue();
	}
	
	#region Piece manipulation
	public void CreatePiece()
	{
		GameObject temp = nextPiece;
		nextPiece = null;
		string type = PieceNames[Random.Range(0, PieceNames.Length)];
		print (type);
		
		currentPiece = temp;
		currentPiece.transform.parent = piecesObject.transform;
		currentPiece.transform.position = Grid[(boardWidth/2)-1,boardHeight -3].worldPos;
		
		nextPiece = (GameObject)GameObject.Instantiate(Resources.Load(type), nextPieceZone.transform.position, Quaternion.identity);
	}
	void RotatePieceLeft()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.Rotate(0,0,90);
			currentPiece.SendMessage("RotatePieceLeft",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	void RotatePieceRight()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.Rotate(0,0,90);
			currentPiece.SendMessage("RotatePieceRight",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	void MovePieceDown()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.position += Vector3.down * tileHeight;
			currentPiece.SendMessage("MovePieceDown",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	void MovePieceLeft()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.position += Vector3.left * tileWidth;
			currentPiece.SendMessage("MovePieceLeft",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	void MovePieceRight()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.position += Vector3.right * tileWidth;
			currentPiece.SendMessage("MovePieceRight",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	void DropPiece()
	{
		if (currentPiece != null)
		{
			//currentPiece.transform.position += Vector3.right * tileWidth;
			currentPiece.SendMessage("DropPiece",SendMessageOptions.DontRequireReceiver);
		}
		// Add blocking code
	}
	#endregion
	
	#region Point Translation
	Vector3 indexToWorld(int x, int y)
	{
		float px = boundaries.x + (x * tileWidth);
		float py = boundaries.y + (y * tileHeight);
		return new Vector3(px,py,0);
	}
	
	int[] worldToIndex(Vector3 worldPos)
	{
		int worldX = Mathf.RoundToInt(worldPos.x);
		int worldY = Mathf.RoundToInt(worldPos.y);
		int px = Mathf.RoundToInt((worldX - boundaries.x) / (tileWidth));
		int py = Mathf.RoundToInt((worldY - boundaries.y) / (tileHeight));
		int[] output = {px, py};
		return output;
	}
	#endregion
	
	#region Tile Assimilation
	static public GridTile[] PutInGridTiles(GameObject[] tiles, GridTile[] oldTiles)
	{
		List<GridTile> output = new List<GridTile>();
		foreach(GridTile tile in oldTiles)
		{
			foreach(GameObject go in tiles)
			{
				if (tile.contents == go)
				{
				tile.contents = null;
				}
			}
			
		}
		foreach(GameObject tile in tiles)
		{
			int[] index = Instance.worldToIndex (tile.transform.position);
			if (index[0] >= 0 && index[0] < Instance.boardWidth && index[1] >= 0 && index[1] < Instance.boardHeight)
			{
				if (Instance.Grid[index[0],index[1]].contents == null)
				{
					output.Add(Instance.Grid[index[0],index[1]]);
					Instance.Grid[index[0],index[1]].contents = tile;
				}
			}
		}
		GridTile[] OutputArray = new GridTile[output.Count];
		output.CopyTo(OutputArray);
		return OutputArray;
	}
	#endregion
	
	#region Line Management
	public void CheckLines()
	{
		// We could do the raycast idea, but no. We'll just scan the lines.
		/*
		int[] counts = new int[boardHeight];
		for(int y = 0; y < boardHeight; y++)
		{
			for(int x = 0; x < boardWidth; x++)
			{
				if (Grid[x,y].contents != null)
				{
					counts[y]++;
				}
			}
		}
		foreach(int count in counts)
		{
			if (count >= boardWidth)
			{
				for(int x = 0; x < boardWidth; x++)
				{
					settledTiles.Remove(Grid[x,count].contents);
					GameObject.Destroy(Grid[x,count].contents);
					Grid[x,count].contents = null;
				}
			}
		}
		*/
		// Fuck it, we're doing it live
		List<int> linesToCollapse = new List<int>();
		for(int y = 0; y < boardHeight; y++)
		{
			//int mask = LayerMask.NameToLayer("TETRIS");
			RaycastHit[] hits = Physics.RaycastAll(Grid[0,y].worldPos + (Vector3.left * (tileWidth + 1)),Vector3.right,tileWidth * (boardWidth + 2));
			//Debug.DrawLine(Grid[0,y].worldPos + (Vector3.left * (tileWidth + 1)),Grid[0,y].worldPos + (Vector3.left * (tileWidth + 1)) + (Vector3.right * tileWidth * (boardWidth + 2)), Color.green, Time.deltaTime);
			if (hits.Length == boardWidth)
			{
				//print ("Aaaaah");
				lines++;
				if (lines % linesPerLevel == 0)
				{
					Levelup();
				}
				for(int x = 0; x < boardWidth; x++)
				{
					Grid[x,y].contents = null;
					hits[x].collider.isTrigger = true;
					hits[x].rigidbody.isKinematic = false;
					hits[x].rigidbody.angularVelocity = Random.onUnitSphere;
					hits[x].rigidbody.AddForce(Random.insideUnitSphere * Explosiveness, ForceMode.VelocityChange);
					hits[x].transform.parent = objectMorgueFalling.transform;
				}
				linesToCollapse.Add(y);
			}
		}
		List<GameObject> listToUpdate = new List<GameObject>();
		List<GridTile> oldGrid = new List<GridTile>();
		foreach(int line in linesToCollapse)
		{
			for (int y = line; y < boardHeight; y++)
			{
				for (int x = 0; x < boardWidth; x++)
				{
					if (Grid[x,y].contents != null && Grid[x,y].contents.transform.IsChildOf(objectPool.transform))
					{
						Grid[x,y].contents.transform.position += Vector3.down * tileWidth;
						oldGrid.Add (Grid[x,y]);
						listToUpdate.Add (Grid[x,y].contents);
					}
				}
			}
		}
		
		for(int x = 0; x < boardWidth; x++)
		{
			if (Grid[x,boardHeight-4].contents != null && Grid[x,boardHeight-4].contents.transform.IsChildOf(objectPool.transform))
			{
				Die();	
			}
		}
		TetrisMode.PutInGridTiles(listToUpdate.ToArray(),oldGrid.ToArray());
		linesText.SendMessage("SetText",lines.ToString() + " Lines",SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion
	
	#region Cleanup
	void EmptyMorgue()
	{
		if(objectMorgue.transform.GetChildCount() > 0)
		{
			GameObject.DestroyImmediate(objectMorgue.transform.GetChild(0).gameObject);
		}
		if (objectMorgueFalling.transform.GetChildCount() > 0)
		{
			Transform t = objectMorgueFalling.transform.GetChild(0);
			if (t.position.y < -100.0f)
			{
				GameObject.DestroyImmediate(t.gameObject);
			}
		}
	}
	#endregion
	
	void OnGUI()
	{
		if (!started && isactive)
		{
			if(GUI.Button(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3),"Start"))
			{
				string type = PieceNames[Random.Range(0, PieceNames.Length)];
				nextPiece = (GameObject)GameObject.Instantiate(Resources.Load(type), nextPieceZone.transform.position, Quaternion.identity);
				CreatePiece();
				started = true;
			}
			
			linesText.SendMessage("SetText","0 Lines",SendMessageOptions.DontRequireReceiver);
			timeText.SendMessage("SetText","0s",SendMessageOptions.DontRequireReceiver);
			levelText.SendMessage("SetText","Level 0",SendMessageOptions.DontRequireReceiver);
		}
		
		if (paused)
		{
			if (GUI.Button(new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3),"Paused"))
			{
				paused = false;
			}
		}
	}
	
	#region Die
	void Die()
	{
		dieTimer = 1.0f;
		dying = true;
		
		while(objectPool.transform.GetChildCount() > 0)
		{
			GameObject go = objectPool.transform.GetChild(0).gameObject;
			go.collider.isTrigger = true;
			go.rigidbody.isKinematic = false;
			go.rigidbody.angularVelocity = Random.onUnitSphere;
			go.rigidbody.AddForce((Random.insideUnitSphere * Explosiveness) + (go.transform.position - transform.position).normalized * Explosiveness, ForceMode.VelocityChange);
			go.transform.parent = objectMorgueFalling.transform;
		}
		
		foreach (GridTile t in Grid)
		{
			t.contents = null;
		}
		
		
		seconds = 0;
		lines = 0;
		levels = 0;
		dropDelay = defaultDropDelay;
	}
	#endregion
	
	#region Levelup
	void Levelup()
	{
		levels++;
		levelText.SendMessage("SetText","Level " + levels.ToString(),SendMessageOptions.DontRequireReceiver);
		MusicController.GetInstance().PitchUp();
		dropDelay *= 0.9f;
	}
	#endregion
}
