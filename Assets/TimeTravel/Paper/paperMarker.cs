using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaperMarker : MonoBehaviour {
	
	public Text debugText;

	public Renderer rend;

	// Use this for initialization
	void Start () {
		debugText.text = "HEY WE STARTED!!!";
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		debugText.text = rend.ToString ();
		Texture resource = Resources.Load("105_Early_Man_Mastodons_and_Mammoths") as Texture;
		rend.material.mainTexture = resource;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
