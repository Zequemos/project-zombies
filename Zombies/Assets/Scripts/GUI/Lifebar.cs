using UnityEngine;
using System.Collections;

public class Lifebar : MonoBehaviour {

	public UISlider lifebar;
	public UILabel label;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		lifebar.sliderValue = (PlayerLogic.getHealth ()/100);
		label.text = ((int)PlayerLogic.getHealth() + "/100");
	}
}
