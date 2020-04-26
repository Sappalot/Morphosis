using UnityEngine;
using UnityEngine.UI;

public class SensorOutputPanel : MonoBehaviour {
	public Image image;

	[HideInInspector]
	private PhenoGenoEnum mode { get; set; }
	private CellAndGenePanel cellAndGenePanel;
	private SignalUnitEnum signalUnit;
	private SignalUnitSlotEnum signalUnitSlot;
	private SensorPanel motherPanel;

	public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot, SensorPanel motherPanel, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.signalUnit = signalUnit;
		this.signalUnitSlot = signalUnitSlot;
		this.motherPanel = motherPanel;
		this.cellAndGenePanel = cellAndGenePanel;
	}

	[HideInInspector]
	public bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void OnClicked() {
		if (MouseAction.instance.actionState == MouseActionStateEnum.selectSignalOutput && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			if (motherPanel != null) {
				//LogicBoxInputPanel.TryAnswerSetReference(signalUnit, signalUnitSlot);
				//AxonInputPanel.TryAnswerSetReference(signalUnit, signalUnitSlot);
				//MouseAction.instance.actionState = MouseActionStateEnum.free;

				AssignNerveInputPanel.instance.TrySetNerveInput(signalUnit, signalUnitSlot);
				GenePanel.instance.cellAndGenePanel.MakeDirty(); // arrows need to be updated
				MarkAsNewForge();

			} else {
				Debug.Log("Can't connect a nerve to a ghost output");
				Audio.instance.ActionDenied(1f);
			}
		}
	}

	private void MarkAsNewForge() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
	}

	public void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Energy Sensor Panel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (motherPanel == null || motherPanel.affectedGeneSensor == null || !motherPanel.affectedGeneSensor.isUsedInternal) {
					image.color = ColorScheme.instance.signalUnused;
				} else if (cellAndGenePanel.cell != null) {
					image.color = selectedCell.GetOutputFromUnit(signalUnit, signalUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				if (motherPanel == null || motherPanel.affectedGeneSensor == null || !motherPanel.affectedGeneSensor.isUsedInternal) {
					image.color = ColorScheme.instance.signalUnused;
				} else {
					image.color = ColorScheme.instance.signalOff;
				}
			}

			isDirty = false;
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return cellAndGenePanel.cell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}
}
