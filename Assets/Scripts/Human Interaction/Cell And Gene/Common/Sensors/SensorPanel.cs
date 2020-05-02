using UnityEngine;

// A panel that can change genotype and handles signals and has 6 output and 0 input(signals)

// Me <== AxonPanel, ConstantSensorPanel, EnergySensorPanel, EffectSensorPanel, AttachmentSensorPanel
public abstract class SensorPanel : SignalUnitPanel {
	public RectTransform settingsPanel; // Not all sensor panels use settings panel

	// merge with code in Gene unit enum = gives=> sensorUnit | now it is double coded
	public GeneSignalUnit affectedGeneSensor {
		get {
			if (gene != null) {
				if (gene.type == CellTypeEnum.Egg) {
					if (signalUnit == SignalUnitEnum.WorkSensorA) {
						return gene.eggCellFertilizeEnergySensor;
					}

					if (signalUnit == SignalUnitEnum.WorkSensorB) {
						return gene.eggCellAttachmentSensor;
					}

					// Do we need attachment sensor? the thisn is it has no gene representation (holds no data)
				}

				if (signalUnit == SignalUnitEnum.ConstantSensor) {
					return gene.constantSenesor;
				}

				if (signalUnit == SignalUnitEnum.Axon) {
					return gene.axon;
				}

				// not a work sensor of any kind
				if (signalUnit == SignalUnitEnum.EnergySensor) {
					return gene.energySensor;
				}

				if (signalUnit == SignalUnitEnum.EffectSensor) {
					return gene.effectSensor;
				}

				if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
					return gene.originSizeSensor;
				}
			}

			return null;
		}
	}

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, signalUnit, cellAndGenePanel);
		if (settingsPanel != null) {
			settingsPanel.gameObject.SetActive(true); // Not all sensor panels use settings panel
		}

		for (int i = 0; i < outputPanels.Length; i++) {
			outputPanels[i].Initialize(mode, signalUnit, IndexToSignalUnitSlotEnum(i), this, cellAndGenePanel);
		}

		MakeDirty();
	}

	public virtual void Update() {
		if (isDirty) {

			foreach (SensorOutputPanel output in outputPanels) {
				output.isGhost = isGhost;
				output.MakeDirty();
			}

			if (settingsPanel != null) {
				settingsPanel.gameObject.SetActive(!isGhost);
			}

			isDirty = false;
		}
	}
}