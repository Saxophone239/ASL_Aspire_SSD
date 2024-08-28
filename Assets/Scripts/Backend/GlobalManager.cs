using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    // private static GlobalManager instance;
	// public static GlobalManager Instance
	// {
	// 	get
	// 	{
	// 		if (instance != null) return instance;

	// 		instance = FindObjectOfType<GlobalManager>();
	// 		if (instance == null)
	// 		{
	// 			Debug.LogWarning("No GlobalManager in the scene!");
	// 			return null;
	// 		}

	// 		return instance;
	// 	}
	// }

	public static GlobalManager Instance;

	// Data to pass into arcade games
	public int CurrentPacket = 0; // Packet_1 starts at index 0
	public bool ReviewPreviousPackets = false;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
