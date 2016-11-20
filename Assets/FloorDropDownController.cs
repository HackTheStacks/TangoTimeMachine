using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloorDropDownController : MonoBehaviour {

	public UserController playerController;
	public Dropdown floorDropdown;
	public HallDropDownController hallDropdown;
	public MapSliderController mapSliderController;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onValueChanged() {
		playerController.setFloor (floorDropdown.value+1);
		hallDropdown.switchFloors (floorDropdown.value);
		mapSliderController.changeMaps (floorDropdown.value + 1);
	}
}
