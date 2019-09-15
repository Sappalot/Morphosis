using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalArrowHandler : MonoBehaviour {
	public HudSignalArrow hudSignalArrowTemplate;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreSliderMoved = false;

	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
	}

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	void Update() {
		if (isDirty) {
			Debug.Log("Updating arrows: " + mode.ToString());

			isDirty = false;
		}
	}
}
