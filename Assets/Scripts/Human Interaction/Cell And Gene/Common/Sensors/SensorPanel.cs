using UnityEngine;
using UnityEngine.UI;

//a panel that can change genotype and handles signals and has 6 output and 0 input(signals)
// Me <== EnergySensorPanel, EffectSensorPanel
public abstract class SensorPanel : CellAndGeneSignalUnitPanel {
	public SensorOutputPanel[] outputPanels;

	public GeneSignalUnit affectedGeneSensor {
		get {
			if (selectedGene != null && selectedGene.type == CellTypeEnum.Egg && signalUnit == SignalUnitEnum.WorkSensorA) {
				return selectedGene.eggCellFertilizeEnergySensor;
			}
			return null;
		}
	}

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, bool isInsideOtherComponent) {
		base.Initialize(mode, signalUnit, isInsideOtherComponent);

		for (int i = 0; i < outputPanels.Length; i++) {
			outputPanels[i].Initialize(mode, signalUnit, (SignalUnitSlotEnum)i, this);
		}
	}

	public virtual void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			foreach (SensorOutputPanel output in outputPanels) {
				output.MakeDirty();
			}

			isDirty = false;
		}
	}
}