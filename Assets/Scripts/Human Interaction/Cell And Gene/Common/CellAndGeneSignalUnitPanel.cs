using System;
using UnityEngine;

// a panel that can change genotype and handles signals
// Me <== (SensorPanel), LogicBox
public abstract class CellAndGeneSignalUnitPanel : CellAndGeneComponentPanel {
	public SignalLocations locations = new SignalLocations();
	public GameObject headingPanel;

	protected bool isInsideOtherComponent; // inside other component

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
		public RectTransform processed; // output for component which has also at leas 1 input
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
		} else if (slot == SignalUnitSlotEnum.processedEarly) {
			return locations.processed;
		}
		return null;
	}

	public virtual void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, bool isInsideOtherComponent) {
		base.Initialize(mode);
		this.signalUnit = signalUnit;
		this.isInsideOtherComponent = isInsideOtherComponent;
		//headingPanel.SetActive(!isInsideOtherComponent);
	}
}
