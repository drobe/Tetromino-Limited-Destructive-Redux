using UnityEngine;
using System.Collections;

public class GamesceneManager : MonoBehaviour 
{
	// A game scene manager is an attempt at enforcing gamestate management - like behavior into a unity project.
	// Instead of gamestates, we're going to have scenes. Scenes will be added additively over one another in order to populate the gameworld with control objects and menu things.
	// We'll also abuse the ability to tell gameobjects not to destroy themselves when levels are loaded. We'll use that to keep our persistent objects around after loading a "clear" scene.
	// Because it's a pain to track which objects come into existence when a level is loaded, we're just going to pop gamestates with a by paying one white, three colorless and dropping an armageddon. I had an excellent top-deck armageddon once.
	
	[SerializeField]
	int  IdxCore		= 0;
	[SerializeField]
	int  IdxClear 		= 1;
	[SerializeField]
	int  IdxMenu		= 2;
	[SerializeField]
	int  IdxSplash  	= 3;
	[SerializeField]
	int[]IdxModes 		= new int[0];
	[SerializeField]
	int[]IdxLevels 		= new int[0];
	
	public bool inSplash = true;
	public bool inMenu = false;
	public bool inMode = false;
	public bool resetMusic = false;
	
	public GameObject splashZone = null;
	public GameObject menuZone = null;
	public GameObject modeZone = null;
	
	public float lerpSpeed = 1.0f;
	
	static private bool isPaused = false;
	
	static public GamesceneManager Instance = null;
	static public GamesceneManager GetInstance()
	{
		// An instance is set on Awake. We should never need to test to see if an Instance exists or not.
		return Instance;
	}
	static public bool Paused()
	{
		return isPaused;
	}
	
	void Awake ()
	{
		DontDestroyOnLoad(this.gameObject);
		Instance = this;
	}
	// Use this for initialization
	void Start () 
	{
		LoadMenu();
		LoadSplash();
		LoadMode(0);
		LoadLevel(Random.Range(0,IdxLevels.Length));
		resetMusic = true;
		inMenu = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (inSplash)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, splashZone.transform.position, lerpSpeed * Time.deltaTime);
		}
		else if (inMenu)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, menuZone.transform.position, lerpSpeed * Time.deltaTime);
			if (resetMusic)
			{
				resetMusic = false;
				MusicController.GetInstance().FadeOut();
				SecondaryMusicController.GetInstance().FadeIn();
			}
		}
		else if (inMode)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, modeZone.transform.position, lerpSpeed * Time.deltaTime);
			if (resetMusic)
			{
				resetMusic = false;
				SecondaryMusicController.GetInstance().FadeOut();
				MusicController.GetInstance().FadeIn();
				TetrisMode.GetInstance().isactive = true;
			}
		}
		else
		{
			inMenu = true;
		}
	}
	
	#region Scene Loaders
	static public void LoadMode(int i)
	{
		if (i < 0 || i >= Instance.IdxModes.Length)
		{
			return;
		}
		Application.LoadLevelAdditive(Instance.IdxModes[i]);
	}
	
	static public void LoadLevel(int i)
	{
		if (i < 0 || i >= Instance.IdxLevels.Length)
		{
			print ("Didn't load level: " + i.ToString());
			return;
		}
		print ("LoadLevel: " + i.ToString());
		Application.LoadLevelAdditive(Instance.IdxLevels[i]);
	}
	
	static public void LoadSplash()
	{
		Application.LoadLevelAdditive(Instance.IdxSplash);
	}
	
	static public void LoadMenu()
	{
		Application.LoadLevelAdditive(Instance.IdxMenu);
	}
	
	static public void LoadCore()
	{
		Application.LoadLevel(Instance.IdxCore);
	}
	
	static public void Clear()
	{
		Application.LoadLevel(Instance.IdxClear);
	}
	#endregion
}
