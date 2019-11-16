using System;
using UnityEngine;

// a panel that can change genotype and handles signals
// Me <== (SensorPanel), LogicBoxPnel, AxonPanel
public abstract class SignalUnitPanel : ComponentPanel {
	public SignalLocations locations = new SignalLocations();
	public GameObject componentHeaderPanel;

	[HideInInspector]
	public SignalUnitEnum signalUnit;

	[Serializable]
	public struct SignalLocations {
		public RectTransform A;
		public RectTransform B;
		public RectTransform C;
		public RectTransform D;
		public RectTransform E;
		public RectTransform F;
		public RectTransform processedEarly; // output for component which has also at leas 1 input
		public RectTransform processedLate;
	}

	public RectTransform GetLocation(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.A) {
			return locations.A;
		} else if (slot == SignalUnitSlotEnum.B) {
			return locations.B;
		} else if (slot == SignalUnitSlotEnum.C) {
			return locations.C;
		} else if (slot == SignalUnitSlotEnum.D) {
			return locations.D;
		} else if (slot == SignalUnitSlotEnum.E) {
			return locations.E;
		} else if (slot == SignalUnitSlotEnum.F) {
			return locations.F;
		} else if (slot == SignalUnitSlotEnum.processedEarly) { // Will we ever need to reach this one?
			return locations.processedEarly;
		} else if (slot == SignalUnitSlotEnum.processedLate) {
			return locations.processedLate;
		}
		return null;
	}

	public virtual void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit) {
		base.Initialize(mode);
		this.signalUnit = signalUnit;
	}
}
