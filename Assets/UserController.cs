using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserController : MonoBehaviour {

	private const int TOTAL_PAGES = 10;

	private int foundPages;
	private int progressPercentage;
	private string hall;
	private bool fingerDown;

	public Text hallText;
	public Text noPagesText;
	public Text percentageText;

	void Start () {
		foundPages = 0;
		progressPercentage = 0;
		hall = "";
		fingerDown = false;
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
}
