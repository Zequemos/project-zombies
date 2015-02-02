using UnityEngine;
using System.Collections;

public class ZombiesBar : MonoBehaviour {

	public UISlider zombiesBar;
	public UILabel label;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		zombiesBar.sliderValue = ((GameMaster.getZombiesToPackage())/50);
		label.text = ((GameMaster.getZombiesToPackage()) + "/50");
	}
}