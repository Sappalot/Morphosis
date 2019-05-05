using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour {
	public float lifeTime;
	public ParticleTypeEnum particlesType;

	public ParticleTypeEnum GetParticlesType() {
		return particlesType;
	}

	protected IEnumerator RemoveSelf() {
		yield return new WaitForSecondsRealtime(lifeTime);
		ParticlePool.instance.Recycle(this);
	}

	public void OnBorrow() {}

	public void OnRecycle() {}
}