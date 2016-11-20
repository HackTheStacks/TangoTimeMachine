using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaperMarker : MonoBehaviour {

	public Renderer rend;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		Texture resource = Resources.Load("115_Fossil_Reptiles_and_Fishes") as Texture;
		rend.material.mainTexture = resource;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
