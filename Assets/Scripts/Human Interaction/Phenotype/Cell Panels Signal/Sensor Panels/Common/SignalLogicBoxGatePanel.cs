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
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedOrOperator() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.operatorType != LogicOperatorEnum.Or) {
			geneLogicBoxGate.operatorType = LogicOperatorEnum.Or;
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedDelete() {
		if (geneLogicBoxGate != null) {
			geneLogicBoxGate.isUsed = false;
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankLeft() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveLeftFlankLeft()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankRight() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveLeftFlankRight()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankRight() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveRightFlankRight()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankLeft() {
		if (geneLogicBoxGate != null && geneLogicBoxGate.TryMoveRightFlankLeft()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Signal logic box");
			}

			if (geneLogicBoxGate != null) {
				if (geneLogicBoxGate.GetTransmittingInputCount() <= 1) {
					operatorTypeLabel.text = "'" + geneLogicBoxGate.operatorType.ToString().ToLower() + "'" + (geneLogicBoxGate.isLocked ? " (L)" : "");
				} else {
					operatorTypeLabel.text = geneLogicBoxGate.operatorType.ToString().ToUpper() + (geneLogicBoxGate.isLocked ? " (L)" : "");
				}
				
				operatorTypeLabel.color = geneLogicBoxGate.isTransmittingSignal ? ColorScheme.instance.signalOff : Color.gray;
			} else {
				operatorTypeLabel.text = "???";
			}

			if (geneLogicBoxGate.isUsed) {
				//Scale and position
				GetComponent<RectTransform>().sizeDelta = new Vector2(SignalLogicBoxPanel.cellWidth * (geneLogicBoxGate.rightFlank - geneLogicBoxGate.leftFlank), SignalLogicBoxPanel.cellHeight);
				transform.position = motherPanel.gateGridOrigo + Vector3.right * geneLogicBoxGate.leftFlank * SignalLogicBoxPanel.cellWidth + Vector3.down * geneLogicBoxGate.row * SignalLogicBoxPanel.cellHeight;
			} else {
				transform.position = motherPanel.gateGridOrigo + Vector3.right * 1500f;
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
			foreach (GeneLogicBoxComponent inputComponent in geneLogicBoxGate.inputsConnected) {
				inputArrows[arrowIndex].gameObject.SetActive(true);
				int targetLeftFlank = 0;
				int targetRightFlank = 0;
				if (row == 1 && inputComponent.row == 2) {
					targetLeftFlank = Mathf.Max(inputComponent.leftFlank, leftFlank);
					targetRightFlank = Mathf.Min(inputComponent.rightFlank, rightFlank);
				} else {
					targetLeftFlank = inputComponent.leftFlank;
					targetRightFlank = inputComponent.rightFlank;
				}
				inputArrows[arrowIndex].transform.position = motherPanel.gateGridOrigo + Vector3.right * (targetLeftFlank + targetRightFlank) * 0.5f * SignalLogicBoxPanel.cellWidth + Vector3.down * ((row + 1) * SignalLogicBoxPanel.cellHeight - 10f);
				inputArrows[arrowIndex].GetComponent<RectTransform>().sizeDelta = new Vector3(20f, Mathf.Max(20f, 20f + SignalLogicBoxPanel.cellHeight * (inputComponent.row - row - 1)), 1f);
				inputArrows[arrowIndex].GetComponent<Image>().color = inputComponent.isTransmittingSignal ? ColorScheme.instance.signalOff : ColorScheme.instance.signalGrayedOut;
				arrowIndex++;
			}

			isDirty = false;
		}
	}

	private int leftFlank {
		get {
			return geneLogicBoxGate.leftFlank;
		}
	}

	private int rightFlank {
		get {
			return geneLogicBoxGate.rightFlank;
		}
	}

	private int row {
		get {
			return geneLogicBoxGate.row;
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
