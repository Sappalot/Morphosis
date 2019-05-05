using System.Collections;
using UnityEngine;

public class ParticlesCellScatter : Particles {
	// 2
	public ParticleSystem cellScatter;

	public void Play(Color shardColor) {
		cellScatter.startColor = shardColor;
		cellScatter.Play();
		StartCoroutine(RemoveSelf()); 
	}

	private void Update() {
		if (Time.timeScale < 0.01f) {
			cellScatter.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}
