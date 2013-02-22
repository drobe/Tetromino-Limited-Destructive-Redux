using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour 
{
	// GAME MUSIC
	
	static MusicController Instance;
	public static MusicController GetInstance()
	{
		return Instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	
	public string[] musicNames = {"Tetris_C_Blachly_OC_ReMix","Tetris_CheDDer_OC_ReMix","Tetris_Linear_Groove_OC_ReMix","Tetris_McVaffeQuasi_Ultimix_OC_ReMix"};
	public int currentTrack = 0;
	public bool volumeup = false;
	public bool volumedown = false;
	public float volumeScale = 0.5f;
	public float levelUpSpeed = 1.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.JoystickButton9) && TetrisMode.GetInstance().isactive)
		{
			NextTrack();
		}
		if (volumeup)
		{
			gameObject.audio.volume = Mathf.Min(gameObject.audio.volume +Time.deltaTime * volumeScale, 1.0f);
			if (gameObject.audio.volume == 1.0f)
			{
				volumeup = false;
			}
		}
		else if (volumedown)
		{
			gameObject.audio.volume = Mathf.Max(gameObject.audio.volume -Time.deltaTime*volumeScale, 0.0f);
			if (gameObject.audio.volume == 0.0f)
			{
				gameObject.audio.Stop();
				volumedown = false;
			}
		}
	}
	
	public void PitchUp()
	{
		gameObject.audio.pitch *= levelUpSpeed;
	}
	public void FadeOut()
	{
		volumedown = true;
	}
	public void FadeIn ()
	{
		gameObject.audio.Play();
		gameObject.audio.pitch = 1.0f;
		volumeup = true;
	}
	public void NextTrack()
	{
		gameObject.audio.Stop();
		
		currentTrack++;
		if (currentTrack >= musicNames.Length)
		{
			currentTrack = 0;
		}
		print (musicNames[currentTrack]);
		gameObject.audio.clip = (AudioClip)Resources.Load(musicNames[currentTrack]);
		gameObject.audio.Play();
	}
	
	void OnGUI()
	{
		if (TetrisMode.GetInstance().isactive)
		{
			if (GUI.Button(new Rect(5,5,200,50),"Next Track:\nAlt: Push Right Thumbstick"))
			{
				NextTrack();
			}
		}
	}
}
