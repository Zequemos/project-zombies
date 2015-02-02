using UnityEngine;
using System.Collections;

public class StaminaBar : MonoBehaviour {
	
	public UISlider staminaBar;
	public UILabel label;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		staminaBar.sliderValue = (PlayerLogic.getStamina ()/100);
		label.text = ((int)PlayerLogic.getStamina() + "/100");
	}
}