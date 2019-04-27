using System.Collections;
using UnityEngine;

public class ParticlesCellBleed : Particles {

	public ParticleSystem cellBleed;

	public void Prime(Color bloodColor) {
		cellBleed.startColor = bloodColor;
		cellBleed.Play();
		StartCoroutine(RemoveSelf());
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(5);
		ParticlePool.instance.Recycle(this);
	}

	public override ParticleTypeEnum GetParticlesType() {
		return ParticleTypeEnum.cellBleed;
	}
}