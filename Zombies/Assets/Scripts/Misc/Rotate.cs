﻿using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public float rotatespeed;

	void Update () {
		transform.Rotate (new Vector3 (0, rotatespeed, 0));
	}
}
