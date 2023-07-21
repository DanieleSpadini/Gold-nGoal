using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	private void Awake()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 300;
	}

	//void OnGUI()
	//{
	//	GUI.matrix = Matrix4x4.TRS(
	//		Vector3.zero,
	//		Quaternion.identity,
	//		Vector3.one * 3
	//	);
	//	GUILayout.Space(200);
	//	StringBuilder sb = new StringBuilder();
	//	sb.AppendLine($"FPS: {(1.0f / Time.deltaTime).ToString("0")}");
	//	GUILayout.Box(sb.ToString());
	//}
}
