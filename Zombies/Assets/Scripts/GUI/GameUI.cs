﻿using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	public UILabel cargadoresLabel, ammoLabel, weaponLabel, clockLabel, staminaLabel, lifeLabel, zombiesLabel, notificatonLabel;
	public UISlider staminaBar, lifebar, zombiesBar;
	
	void Update () {
		//ammo
		if (PlayerLogic.GetWeapon() == 0) {
			weaponLabel.text = "Knife";
			cargadoresLabel.text = "";
			ammoLabel.text = "";
			
		} else if (PlayerLogic.GetWeapon() == 1) {
			weaponLabel.text = "Pistol";
			cargadoresLabel.text = Shoot.getCurrentCargadores(PlayerLogic.GetWeapon()).ToString();
			ammoLabel.text = Shoot.getCurrentAmmo(PlayerLogic.GetWeapon()) + "/" + Shoot.getStartAmmo(PlayerLogic.GetWeapon()) ;

		} else if (PlayerLogic.GetWeapon() == 2) {
			weaponLabel.text = "AK - 47";
			cargadoresLabel.text = Shoot.getCurrentCargadores(PlayerLogic.GetWeapon()).ToString();
			ammoLabel.text = Shoot.getCurrentAmmo(PlayerLogic.GetWeapon()) + "/" + Shoot.getStartAmmo(PlayerLogic.GetWeapon());
			
		} else if (PlayerLogic.GetWeapon() == 3) {
			weaponLabel.text = "Grenades";
			cargadoresLabel.text = "";
			ammoLabel.text = Shoot.getCurrentAmmo(PlayerLogic.GetWeapon()).ToString();
		}
		//staminabar
		staminaBar.sliderValue = (PlayerLogic.getStamina()/100);
		staminaLabel.text = ((int)PlayerLogic.getStamina() + "/100");
		//lifebar
		lifebar.sliderValue = (PlayerLogic.getHealth()/100);
		lifeLabel.text = ((int)PlayerLogic.getHealth() + "/100");
		//zombiesbar
		zombiesBar.sliderValue = ((float)GameMaster.getZombiesToPackage()/50);
		zombiesLabel.text = ((GameMaster.getZombiesToPackage()) + "/50");
		if(GameMaster.getZombiesToPackage() != 50) notificatonLabel.text = " ";
		else if(GameMaster.getZombiesToPackage() == 50) notificatonLabel.text = "You can ask for ammo (5) or for a health kit (6)";
		//clock
		clockLabel.text = GameMaster.getClock();
	}
}
