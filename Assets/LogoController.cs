using UnityEngine;
using System.Collections;

public class LogoController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LogoClick() {
		Application.LoadLevel("Main Scene");
	}
}
