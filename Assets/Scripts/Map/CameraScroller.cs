using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
	[SerializeField] private Vector2 cameraYRange;

	private float wholeCameraRange;

	private void Start()
	{
		wholeCameraRange = cameraYRange.y - cameraYRange.x;
	}

	private void Update()
	{
		// Change scrollbar value according to mouse scroll
		if (Mouse.current != null)
		{
			Vector2 vec = Mouse.current.scroll.ReadValue();
			if (vec.y >= 120)
				scrollbar.value -= 0.05f * (vec.y / 120);
			else if (vec.y <= -120)
				scrollbar.value -= 0.05f * (vec.y / 120);
		}
		scrollbar.value = Mathf.Clamp(scrollbar.value, 0.0f, 1.0f);

		// If scrollbar is changed, change camera position accordingly
		if (transform.position.y + wholeCameraRange / 2 != scrollbar.value)
		{
			float newY = cameraYRange.y - scrollbar.value * wholeCameraRange;

			transform.position = new Vector3
			(
				transform.position.x,
				newY,
				transform.position.z	
			);
		}
	}
}
