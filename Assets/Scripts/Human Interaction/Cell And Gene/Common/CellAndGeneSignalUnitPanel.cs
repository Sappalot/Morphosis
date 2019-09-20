using System;
using UnityEngine;

public abstract class CellAndGeneSignalUnitPanel : MonoBehaviour {
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
	public SignalLocations locations = new SignalLocations();

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
		} else if (slot == SignalUnitSlotEnum.processed) {
			return locations.processed;
		}
		return null;
	}

	[HideInInspector]
	public bool dirt = false;
	public void MakeDirty() {
		dirt = true;
	}

	[HideInInspector]
	protected bool ignoreSliderMoved = false;

	[HideInInspector]
	public PhenoGenoEnum mode { get; set; }
	protected SignalUnitEnum signalUnit;

	virtual public void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit) {
		this.mode = mode;
		this.signalUnit = signalUnit;
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
				return GenePanel.instance.selectedGene;
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
