using System.Collections;
using UnityEngine;

public class ParticlesCellBleed : Particles {
	//5
	public ParticleSystem cellBleed;

	public void Play(Color bloodColor) {
		cellBleed.startColor = bloodColor;
		cellBleed.Play();
		StartCoroutine(RemoveSelf());
	}

	private void Update() {
		if (Time.timeScale < 0.01f) {
			cellBleed.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}