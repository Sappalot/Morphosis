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
		public RectTransform inputA;
		public RectTransform inputB;
		public RectTransform inputC;
		public RectTransform inputD;
		public RectTransform inputE;
		public RectTransform inputF;

		public RectTransform outputEarlyA;
		public RectTransform outputLateA;

		public RectTransform outputEarlyB;
		public RectTransform outputLateB;

		public RectTransform outputEarlyC;
		public RectTransform outputLateC;

		public RectTransform outputEarlyD;
		public RectTransform outputLateD;

		public RectTransform outputEarlyE;
		public RectTransform outputLateE;

		public RectTransform outputEarlyF;
		public RectTransform outputLateF;
	}

	public RectTransform GetLocation(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.inputA) {
			return locations.inputA;
		} else if (slot == SignalUnitSlotEnum.inputB) {
			return locations.inputB;
		} else if (slot == SignalUnitSlotEnum.inputC) {
			return locations.inputC;
		} else if (slot == SignalUnitSlotEnum.inputD) {
			return locations.inputD;
		} else if (slot == SignalUnitSlotEnum.inputE) {
			return locations.inputE;
		} else if (slot == SignalUnitSlotEnum.inputF) {
			return locations.inputF;
		} else if (slot == SignalUnitSlotEnum.outputEarlyA) {
			return locations.outputEarlyA;
		} else if (slot == SignalUnitSlotEnum.outputLateA) {
			return locations.outputLateA;
		} else if (slot == SignalUnitSlotEnum.outputEarlyB) {
			return locations.outputEarlyB;
		} else if (slot == SignalUnitSlotEnum.outputLateB) {
			return locations.outputLateB;
		} else if (slot == SignalUnitSlotEnum.outputEarlyC) {
			return locations.outputEarlyC;
		} else if (slot == SignalUnitSlotEnum.outputLateC) {
			return locations.outputLateC;
		} else if (slot == SignalUnitSlotEnum.outputEarlyD) {
			return locations.outputEarlyD;
		} else if (slot == SignalUnitSlotEnum.outputLateD) {
			return locations.outputLateD;
		} else if (slot == SignalUnitSlotEnum.outputEarlyE) {
			return locations.outputEarlyE;
		} else if (slot == SignalUnitSlotEnum.outputLateE) {
			return locations.outputLateE;
		} else if (slot == SignalUnitSlotEnum.outputEarlyF) {
			return locations.outputEarlyF;
		} else if (slot == SignalUnitSlotEnum.outputLateF) {
			return locations.outputLateF;
		}
		return null;
	}

	public virtual void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, cellAndGenePanel);
		this.signalUnit = signalUnit;
	}

	public SignalUnitSlotEnum IndexToSignalUnitSlotEnum(int index) {
		if (index == 0) {
			return SignalUnitSlotEnum.outputLateA;
		} else if (index == 1) {
			return SignalUnitSlotEnum.outputLateB;
		} else if (index == 2) {
			return SignalUnitSlotEnum.outputLateC;
		} else if (index == 3) {
			return SignalUnitSlotEnum.outputLateD;
		} else if (index == 4) {
			return SignalUnitSlotEnum.outputLateE;
		} else if (index == 5) {
			return SignalUnitSlotEnum.outputLateF;
		}
		return SignalUnitSlotEnum.outputLateA; // error
	}
}
