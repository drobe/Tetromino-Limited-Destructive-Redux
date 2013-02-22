using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
	static SoundController Instance;
	public static SoundController GetInstance()
	{
		return Instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PlayClip(string clip)
	{
		AudioClip theClip = (AudioClip)Resources.Load(clip);
		if (theClip != null)
		{
			gameObject.audio.PlayOneShot(theClip);
		}
	}
}
