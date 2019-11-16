using UnityEngine;
using UnityEngine.UI;

public class SensorOutputPanel : MonoBehaviour {
	public Image image;

	[HideInInspector]
	public PhenoGenoEnum mode { get; set; }
	private SignalUnitEnum signalUnit;
	private SignalUnitSlotEnum signalUnitSlot;
	private SensorPanel motherPanel;

	public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot, SensorPanel motherPanel) {
		this.mode = mode;
		this.signalUnit = signalUnit;
		this.signalUnitSlot = signalUnitSlot;
		this.motherPanel = motherPanel;
	}

	[HideInInspector]
	public bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void OnClicked() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			LogicBoxInputPanel.TryAnswerSetReference(signalUnit, signalUnitSlot);
			AxonInputPanel.TryAnswerSetReference(signalUnit, signalUnitSlot);

			MouseAction.instance.actionState = MouseActionStateEnum.free;
		}
	}

	public void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					image.color = selectedCell.GetOutputFromUnit(signalUnit, signalUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				image.color = ColorScheme.instance.signalOff;
			}

			isDirty = false;
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}
}
