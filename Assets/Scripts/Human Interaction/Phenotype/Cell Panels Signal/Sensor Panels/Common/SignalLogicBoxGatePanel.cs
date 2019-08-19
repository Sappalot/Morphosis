using UnityEngine;
using UnityEngine.UI;

public class SignalLogicBoxGatePanel : MonoBehaviour {
	public Text operatorTypeLabel;

	public GameObject buttonOverlay;
	public Image andButtonImage;
	public Image orButtonImage;

	private GeneLogicBoxGate gate;
	private bool isMouseHoverng;



	public GeneLogicBoxGate geneLogicBoxGate;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;

	}

	public void OnPointerEnterArea() {
		isMouseHoverng = (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome);
		
		MakeDirty();
	}

	public void OnPointerExitArea() {
		isMouseHoverng = false;
		MakeDirty();
	}

	public void OnClickedAndOperator() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.operatorType != LogicOperatorEnum.And) {
			geneLogicBoxGate.operatorType = LogicOperatorEnum.And;
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedOrOperator() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.operatorType != LogicOperatorEnum.Or) {
			geneLogicBoxGate.operatorType = LogicOperatorEnum.Or;
			MarkAsNewForge();
			MakeDirty();
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

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Signal logic box");
			}

			if (geneLogicBoxGate != null) {
				operatorTypeLabel.text = geneLogicBoxGate.operatorType.ToString();
			} else {
				operatorTypeLabel.text = "???";
			}

			buttonOverlay.SetActive(isMouseHoverng);
			if (buttonOverlay && geneLogicBoxGate != null) {
				
				andButtonImage.color = geneLogicBoxGate.operatorType == LogicOperatorEnum.And ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				orButtonImage.color = geneLogicBoxGate.operatorType == LogicOperatorEnum.Or ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			isDirty = false;
		}
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
