﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	public Transform target;
	public Vector3 offset;
	public float pitch = 2f;
	public float yawSpeed = 100f;

	float currentZoom = 2f;
	float yawInput = 0f;
	float yawInputd = 0f;

	public float zoomSpeed = 4f;
	public float minZoom = 2f;
	public float maxZoom = 4f;

	void Update()
	{
		currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
		currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

		yawInputd -= Input.GetAxis("Vertical") * yawSpeed * Time.deltaTime;
		yawInput -= Input.GetAxis("Horizontal") * yawSpeed * Time.deltaTime;
	}

	// Update is called once per frame
	void LateUpdate()
	{

		transform.position = target.position - offset * currentZoom;
		transform.LookAt(target.position + Vector3.up * pitch);

		transform.RotateAround(target.position, Vector3.up, yawInput);
		transform.RotateAround(target.position, Vector3.back, yawInputd);

	}
}