using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PageBehavior : MonoBehaviour {

	bool isVisible;
	public Text DebugText;

	// Use this for initialization
	void Start () {
		isVisible = false;
		changeDebugText ("Invisible");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnBecameVisible() {
		isVisible = true;
		changeDebugText ("Visible");
	}

	void OnBecameInvisible() {
		isVisible = false;
		changeDebugText ("Invisible");
	}

	void changeDebugText(string text) {
		DebugText.text = text;
	}
}