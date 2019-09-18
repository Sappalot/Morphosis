using System;
using UnityEngine;

public abstract class CellSensorPanel : MonoBehaviour {
	[Serializable]
	public struct LogicBoxLocations {
		public RectTransform outputA;
		public RectTransform outputB;
		public RectTransform outputC;
		public RectTransform outputD;
		public RectTransform outputE;
	}
	public LogicBoxLocations locations = new LogicBoxLocations();

	public RectTransform GetLocation(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.A) {
			return locations.outputA;
		} else if (slot == SignalUnitSlotEnum.B) {
			return locations.outputB;
		} else if (slot == SignalUnitSlotEnum.C) {
			return locations.outputC;
		} else if (slot == SignalUnitSlotEnum.D) {
			return locations.outputD;
		} else if (slot == SignalUnitSlotEnum.E) {
			return locations.outputE;
		}
		return null;
	}

	[HideInInspector]
	protected bool ignoreSliderMoved = false;

	[HideInInspector]
	public PhenoGenoEnum mode { get; set; }
	protected SignalUnitEnum outputUnit;

	public void Initialize(PhenoGenoEnum mode, SignalUnitEnum outputUnit) {
		this.mode = mode;
		this.outputUnit = outputUnit;
	}

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void ApplyChange() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}

	public void MakeCreatureChanged() {
		CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
		CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
		CreatureSelectionPanel.instance.soloSelected.generation = 1;
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GeneCellPanel.instance.selectedGene;
			}
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
