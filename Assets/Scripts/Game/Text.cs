using UnityEngine;
using System.Collections;

public class Text : MonoBehaviour 
{
	public TextMesh myTextMesh = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void SetText(string text)
	{
		myTextMesh.text = text;
	}
}
