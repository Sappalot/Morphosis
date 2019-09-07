using UnityEngine;
using UnityEngine.UI;

public class SignalLogicBoxInputPanel : MonoBehaviour {
	public Image motherBlockBackground;
	public Image motherPassBackground;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private bool isDirty = false;
	private bool ignoreSliderMoved = false;
	private SignalLogicBoxPanel motherPanel;
	public GeneLogicBoxInput geneLogicBoxInput;

	public void Initialize(PhenoGenoEnum mode, SignalLogicBoxPanel motherPanel) {
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
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		geneLogicBoxInput.valveMode = SignalFlowValveEnum.Block;
		motherPanel.MarkAsNewForge();
		motherPanel.MakeDirty();
		MakeDirty();
	}

	public void OnMotherPassClicked() {
		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}
		geneLogicBoxInput.valveMode = SignalFlowValveEnum.Pass;
		motherPanel.MarkAsNewForge();
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
				motherBlockBackground.color = geneLogicBoxInput.valveMode == SignalFlowValveEnum.Block ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				motherPassBackground.color = geneLogicBoxInput.valveMode == SignalFlowValveEnum.Pass ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
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
