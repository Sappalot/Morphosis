using UnityEngine;
using UnityEngine.UI;

// TODO Generalize to just be an input panel for all units
public class LogicBoxInputPanel : MonoBehaviour {
	public Image motherBlockBackground;
	public Image motherPassBackground;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private bool isDirty = false;
	private bool ignoreSliderMoved = false;
	private LogicBoxPanel motherPanel;
	public GeneLogicBoxInput geneLogicBoxInput;

	public void Initialize(PhenoGenoEnum mode, LogicBoxPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
	}

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;
	}

	public void OnMotherBlockClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		geneLogicBoxInput.valveMode = SignalValveModeEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		MakeDirty();
	}

	public void OnMotherPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype || ignoreSliderMoved) {
			return;
		}
		geneLogicBoxInput.valveMode = SignalValveModeEnum.Pass;
		motherPanel.MarkAsNewForge();
		motherPanel.UpdateConnections();
		motherPanel.MakeDirty();
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Hibernate Panel");
			}
			ignoreSliderMoved = true;

			if (selectedGene != null) {
				motherBlockBackground.color = geneLogicBoxInput.valveMode == SignalValveModeEnum.Block ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				motherPassBackground.color = geneLogicBoxInput.valveMode == SignalValveModeEnum.Pass ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			ignoreSliderMoved = false;

			isDirty = false;
		}
	}

	private Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GeneCellPanel.instance.selectedGene;
			}
		}
	}

	private Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}


}
