using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour {
	public void OnBorrow() {

	}

	public void OnRecycle() {

	}

	public virtual ParticleTypeEnum GetParticlesType() {
		return ParticleTypeEnum.undefined;
	}
}
