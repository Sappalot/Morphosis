using System.Collections;
using UnityEngine;

public class ParticlesCellScatter : Particles {

	public ParticleSystem particles;

	public void Prime(Color shardColor) {
		particles.startColor = shardColor;
		particles.Play();
		StartCoroutine(RemoveSelf()); 
	}

	private IEnumerator RemoveSelf() {
		yield return new WaitForSeconds(2);
		ParticlePool.instance.Recycle(this);
		//Destroy(gameObject);
	}

	public override ParticleTypeEnum GetParticlesType() {
		return ParticleTypeEnum.cellScatter;
	}
}
