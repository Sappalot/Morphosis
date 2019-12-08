using System.Collections;
using UnityEngine;

public class ParticlesCellTeleport : Particles {
	// 2
	public ParticleSystem cellTeleport;

	public void Play(Color shardColor) {
		var mainModule = cellTeleport.main;
		mainModule.startColor = shardColor;
		cellTeleport.Play();
		StartCoroutine(RemoveSelf()); 
	}

	private void Update() {
		if (Time.timeScale < 0.01f) {
			cellTeleport.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}
