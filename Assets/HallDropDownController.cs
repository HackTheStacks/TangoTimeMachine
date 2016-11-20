using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HallDropDownController : MonoBehaviour {

	public UserController playerController;
	public Dropdown floorDropdown;
	public Dropdown hallDropdown;

	// Use this for initialization
	void Start () {
		addOptions (0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onValueChanged() {
		string hallName = playerController.getHallNameFromIndex (hallDropdown.value);
		playerController.setHall (hallName);
	}

	public void switchFloors(int floor) {
		hallDropdown.ClearOptions ();
		addOptions (floor);
	}

	public void addOptions(int floor) {
		foreach (string hall in playerController.getCurrentExhibits(floor)) {
			hallDropdown.options.Add(new Dropdown.OptionData(hall));
		}
	}
}
