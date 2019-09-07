using UnityEngine;
using UnityEngine.UI;

public class SignalLogicBoxGatePanel : MonoBehaviour {
	public Text operatorTypeLabel;

	public GameObject buttonOverlay;
	public Image andButtonImage;
	public Image orButtonImage;

	public Image[] inputArrows = new Image[5];

	private bool isMouseHoverng;

	public GeneLogicBoxGate geneLogicBoxGate;

	private SignalLogicBoxPanel motherPanel;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode, SignalLogicBoxPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;

	}

	public void OnPointerEnterArea() {
		isMouseHoverng = (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !geneLogicBoxGate.isLocked);
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

	public void OnClickedDelete() {
		if (geneLogicBoxGate != null) {
			geneLogicBoxGate.isUsed = false;
			motherPanel.UpdateConnections();
			MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankLeft() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveLeftFlankLeft()) {
			motherPanel.UpdateConnections();
			MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankRight() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveLeftFlankRight()) {
			motherPanel.UpdateConnections();
			MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankRight() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveRightFlankRight()) {
			motherPanel.UpdateConnections();
			MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankLeft() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveRightFlankLeft()) {
			motherPanel.UpdateConnections();
			MarkAsNewForge();
			motherPanel.MakeDirty();
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
				operatorTypeLabel.text = geneLogicBoxGate.operatorType.ToString() + (geneLogicBoxGate.isLocked ? " (L)" : "");
			} else {
				operatorTypeLabel.text = "???";
			}

			if (geneLogicBoxGate.isUsed) {
				//Scale and position
				GetComponent<RectTransform>().sizeDelta = new Vector2(SignalLogicBoxPanel.cellWidth * (geneLogicBoxGate.rightFlank - geneLogicBoxGate.leftFlank), SignalLogicBoxPanel.cellHeight);
				transform.position = motherPanel.gateGridOrigo + Vector3.right * geneLogicBoxGate.leftFlank * SignalLogicBoxPanel.cellWidth + Vector3.down * geneLogicBoxGate.row * SignalLogicBoxPanel.cellHeight;
			} else {
				transform.position = motherPanel.gateGridOrigo + Vector3.right * 500f;
			}

			buttonOverlay.SetActive(isMouseHoverng);
			if (buttonOverlay && geneLogicBoxGate != null) {
				
				andButtonImage.color = geneLogicBoxGate.operatorType == LogicOperatorEnum.And ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				orButtonImage.color = geneLogicBoxGate.operatorType == LogicOperatorEnum.Or ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			// input arrows
			foreach (Image i in inputArrows) {
				i.gameObject.SetActive(false);
			}

			int arrowIndex = 0;
			foreach (GeneLogicBoxGate g in geneLogicBoxGate.inputGates) {
				inputArrows[arrowIndex].gameObject.SetActive(true);

				inputArrows[arrowIndex].transform.position = motherPanel.gateGridOrigo + Vector3.right * (g.leftFlank + g.rightFlank) * 0.5f * SignalLogicBoxPanel.cellWidth + Vector3.down * (geneLogicBoxGate.row + 1) * SignalLogicBoxPanel.cellHeight;

				arrowIndex++;
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
