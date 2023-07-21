using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using S = System;

[DisallowMultipleComponent]
public class Models3DUI : MonoBehaviour
{
	private RenderTexture rt = null;
	[SerializeField]
	private Camera renderCamera = null;

	[SerializeField]
	private RawImage cameraCanvas = null;


	// Script to render 3D models of the characters in UI
	void Awake()
	{
		Rect rect = cameraCanvas.rectTransform.rect;
		rt = new RenderTexture((int)rect.width, (int)rect.height, 32);
		renderCamera.targetTexture = rt;
		cameraCanvas.texture = rt;
	}
	void OnDestroy()
	{
		if(rt != null)
		{
			cameraCanvas.texture = null;
			rt.Release();
		}
	}
}
