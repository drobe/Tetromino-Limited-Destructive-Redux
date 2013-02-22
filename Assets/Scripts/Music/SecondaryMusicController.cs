using UnityEngine;
using System.Collections;

public class SecondaryMusicController : MonoBehaviour 
{
	// MENU MUSIC
	
	static SecondaryMusicController Instance;
	public static SecondaryMusicController GetInstance()
	{
		return Instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	
	public bool volumeup = false;
	public bool volumedown = false;
	public float volumeScale = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
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
	
	public void FadeOut()
	{
		volumedown = true;
	}
	public void FadeIn ()
	{
		gameObject.audio.Play();
		volumeup = true;
	}
}
