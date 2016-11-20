using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSliderController : MonoBehaviour {

	private const int MAP_MIN_X = 380;
	private const int MAP_MAX_X = 1550;

	public Image currentMap;
	public Image oldMap;
	public Text debugTextPosition;

	private bool fingerDown;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);
			var pos = transform.position;
			var touchX = touch.position.x;
			debugTextPosition.text = "Position: " + touchX.ToString();
			if(touchX >= MAP_MIN_X && touchX <= MAP_MAX_X) {
				pos.x = touch.position.x;
				transform.position = pos;
				currentMap.fillAmount = (float)(1 / (float)(MAP_MAX_X - MAP_MIN_X)) * (touchX - MAP_MIN_X);
			}
		}
	}

	public void changeMaps(int floor) {
		Sprite newCurrentMap = Resources.Load<Sprite>("maps/currentmapfloor" + floor);
		Sprite newOldMap = Resources.Load<Sprite>("maps/oldmapfloor" + floor);
		currentMap.sprite = newCurrentMap;
		oldMap.sprite = newOldMap;
	}
}
