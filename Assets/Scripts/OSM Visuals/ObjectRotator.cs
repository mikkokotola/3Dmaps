﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime script for turning POI icons and labels towards currently active camera.
/// </summary>

public class ObjectRotator : MonoBehaviour {

	void Update () {
		transform.LookAt(transform.position - Camera.main.transform.rotation * Vector3.back,
			Camera.main.transform.rotation * Vector3.up);
	}
}
