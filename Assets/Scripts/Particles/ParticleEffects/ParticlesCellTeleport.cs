using System.Collections;
using UnityEngine;

public class ParticlesCellTeleport : Particles {

	public ParticleSystem particles;

	public void Prime(Color shardColor) {
		particles.startColor = shardColor;
		particles.Play();
		StartCoroutine(RemoveSelf()); 
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(2f);
		ParticlePool.instance.Recycle(this);
	}

	public override ParticleTypeEnum GetParticlesType() {
		return ParticleTypeEnum.cellTeleport;
	}
}
