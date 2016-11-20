using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserController : MonoBehaviour {

	private const int TOTAL_PAGES = 10;
	private string[,] currentExhibits = new string[,] {
		{
			"Guggenheim Hall of Minerals",
			"Ross Hall of Meteorites",
			"Spitzer Hall of Human Origins",
			"Grand Gallery",
			"Warburg Hall of New York State Environment",
			"North American Forests",
			"Northwest Coast Indians",
			"Lefrak Imax Theater"
		},
		{
			"South American Peoples",
			"Mexico and Central America",
			"Birds of the World",
			"African Peoples",
			"Stout Hall of Asian Peoples",
			"Akeley Gallery",
			"", "",
		},
		{
			"Margaret Mead Hall of Pacific Peoples",
			"Plains Indians",
			"Eastern Woodlands Indians",
			"Primates",
			"Special Exhibition Gallery 3",
			"Sanford Hall of North American Birds",
			"New York State Mammals",
			""
		},
		{
			"Wallach Orientation Center",
			"Vertebrate Origins",
			"Milstein Hall of Advanced Mammals",
			"Primitive Mammals", "", "", "", ""
		}
	};

	private int foundPages;
	private int progressPercentage;
	private string hall;
	private int floor;
	private bool fingerDown;

	public Text hallText;
	public Text noPagesText;
	public Text percentageText;

	void Start () {
		foundPages = 0;
		progressPercentage = 0;
		hall = "";
		fingerDown = false;
		initializePlayerPrefs ();
	}

	void Update () {
		if (Input.touchCount > 0) {
			if (fingerDown == false) {
				updateProgress ();
			}
			fingerDown = true;
		} else {
			fingerDown = false;
		}
	}

	public int getProgress() {
		return progressPercentage;
	}

	public string getHall() {
		return hall;
	}

	public int getFloor() {
		return floor;
	}

	public void updateProgress() {
		foundPages++;
		if(progressPercentage >= 100) return;
		progressPercentage = (int)(((double)foundPages / TOTAL_PAGES) * 100);
		noPagesText.text = "# Pages: " + foundPages.ToString ();
		percentageText.text = progressPercentage.ToString () + "%";
	}

	public void setHall(string newHall) {
		hall = newHall;
		hallText.text = hall;
	}

	public void setFloor(int newFloor) {
		floor = newFloor;
	}

	private void initializePlayerPrefs() {
		for(int i = 0; i < 4; i++) {
			for(int j = 0; j < 8; j++) {
				PlayerPrefs.SetInt (currentExhibits[i, j], 0);
			}
		}
	}

	public string[] getCurrentExhibits(int floor) {
		string[] exhibitsForFloor = new string[8];
		for(int i = 0; i < 8; i++) {
			exhibitsForFloor [i] = currentExhibits [floor, i];
		}
		return exhibitsForFloor;
	}

	public string getHallNameFromIndex(int index) {
		return currentExhibits[floor, index];
	}
}
