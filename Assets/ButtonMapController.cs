using UnityEngine;
using System.Collections;

public class ButtonMapController : MonoBehaviour {

	public GameObject SlidingMap;
	public GameObject DropDownCanvas;

	private bool isMapShowing;

	// Use this for initialization
	void Start () {
		isMapShowing = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showMap() {
		isMapShowing = !isMapShowing;
		SlidingMap.SetActive (isMapShowing);
		DropDownCanvas.SetActive (!isMapShowing);
	}

}
