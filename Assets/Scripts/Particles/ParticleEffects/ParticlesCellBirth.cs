using System.Collections;
using UnityEngine;

public class ParticlesCellBirth : Particles {
	// 1

	public ParticleSystem cellBirth;
	public ParticleSystem cover;

	public void Play(Color color) {
		//cellBirth.startColor = color;
		cellBirth.Play();
		cover.Play();
		StartCoroutine(RemoveSelf()); 
	}

	private void Update() {
		if (Time.timeScale < 0.01f) {
			cellBirth.Simulate(Time.unscaledDeltaTime, true, false);
			cover.Simulate(Time.unscaledDeltaTime, true, false);
		}
	}
}
