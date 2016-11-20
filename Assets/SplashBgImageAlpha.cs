using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashBgImageAlpha : MonoBehaviour {

	private Image image;

	// Use this for initialization
	void Start () {
		image = GetComponent<Image>();
		var tempColor = image.color;
		tempColor.a = 0.5f;
		image.color = tempColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
