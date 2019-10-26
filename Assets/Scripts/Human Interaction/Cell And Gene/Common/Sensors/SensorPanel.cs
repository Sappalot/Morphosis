using UnityEngine;
using UnityEngine.UI;

//a panel that can change genotype and handles signals and has 6 output and 0 input(signals)
// Me <== EnergySensorPanel, EffectSensorPanel
public abstract class SensorPanel : CellAndGeneSignalUnitPanel {
	public SensorOutputPanel[] outputPanels;
	public RectTransform settingsPanel; // Not all sensor panels use settings panel
	protected bool isUsed = false;

	private void Awake() {
		if (!isUsed && settingsPanel != null) {
			settingsPanel.gameObject.SetActive(false);
		}
	}

	public GeneSignalUnit affectedGeneSensor {
		get {
			if (selectedGene != null) {
				if (selectedGene.type == CellTypeEnum.Egg) {
					if (signalUnit == SignalUnitEnum.WorkSensorA) {
						return selectedGene.eggCellFertilizeEnergySensor;
					}
					// Do we need attachment sensor? the thisn is it has no gene representation (holds no data)
				}

				// not a work sensor of any kind
				if (signalUnit == SignalUnitEnum.EnergySensor) {
					return selectedGene.energySensor;
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
			outputPanels[i].Initialize(mode, signalUnit, (SignalUnitSlotEnum)i, this);
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