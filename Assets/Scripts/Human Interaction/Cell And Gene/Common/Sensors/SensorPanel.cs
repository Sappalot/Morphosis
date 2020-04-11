using UnityEngine;
using UnityEngine.UI;

//a panel that can change genotype and handles signals and has 6 output and 0 input(signals)
// Me <== ConstantSensorPanel, EnergySensorPanel, EffectSensorPanel, Attachment 
public abstract class SensorPanel : SignalUnitPanel {
	public SensorOutputPanel[] outputPanels;
	public RectTransform settingsPanel; // Not all sensor panels use settings panel
	protected bool isUsed = false; // is used in what sense??????

	private void Awake() {
		if (!isUsed && settingsPanel != null) {
			settingsPanel.gameObject.SetActive(false);
		}
	}

	// merge with code in Gene unit enum = gives=> sensorUnit | now it is double coded
	public GeneSignalUnit affectedGeneSensor {
		get {
			if (selectedGene != null) {
				if (selectedGene.type == CellTypeEnum.Egg) {
					if (signalUnit == SignalUnitEnum.WorkSensorA) {
						return selectedGene.eggCellFertilizeEnergySensor;
					}

					if (signalUnit == SignalUnitEnum.WorkSensorB) {
						return selectedGene.eggCellAttachmentSensor;
					}

					// Do we need attachment sensor? the thisn is it has no gene representation (holds no data)
				}

				if (signalUnit == SignalUnitEnum.ConstantSensor) {
					return selectedGene.constantSenesor;
				}

				if (signalUnit == SignalUnitEnum.Axon) {
					return selectedGene.axon;
				}

				// not a work sensor of any kind
				if (signalUnit == SignalUnitEnum.EnergySensor) {
					return selectedGene.energySensor;
				}

				if (signalUnit == SignalUnitEnum.EffectSensor) {
					return selectedGene.effectSensor;
				}

				if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
					return selectedGene.originSizeSensor;
				}



			}

			return null;
		}
	}

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit) {
		base.Initialize(mode, signalUnit);
		if (settingsPanel != null) {
			settingsPanel.gameObject.SetActive(true); // Not all sensor panels use settings panel
		}
		
		isUsed = true;

		for (int i = 0; i < outputPanels.Length; i++) {
			outputPanels[i].Initialize(mode, signalUnit, IndexToSignalUnitSlotEnum(i), this);
		}
	}

	public virtual void Update() {
		if (isDirty) {
			foreach (SensorOutputPanel output in outputPanels) {
				output.MakeDirty();
			}

			isDirty = false;
		}
	}
}