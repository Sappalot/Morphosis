using UnityEngine;
using UnityEngine.UI;

public class LogicBoxGatePanel : MonoBehaviour {
	public Text operatorTypeLabel;

	public GameObject buttonOverlay;
	public Image andButtonImage;
	public Image orButtonImage;

	public Image[] inputArrows = new Image[5];

	private bool isMouseHoverng;

	private int row;
	private int index;

	public GeneLogicBoxGate affectedGeneLogicBoxGate { 
		get {
			if (selectedGene.type == CellTypeEnum.Egg && motherPanel.signalUnit == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic.GetGate(row, index);
			}
			return null;
		}
	}

	private LogicBoxPanel motherPanel;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode, int row, int index, LogicBoxPanel motherPanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
		this.row = row;
		this.index = index;
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;

	}

	public void OnPointerEnterArea() {
		isMouseHoverng = (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !affectedGeneLogicBoxGate.isLocked);
		MakeDirty();
	}

	public void OnPointerExitArea() {
		isMouseHoverng = false;
		MakeDirty();
	}

	public void OnClickedAndOperator() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.operatorType != LogicOperatorEnum.And) {
			affectedGeneLogicBoxGate.operatorType = LogicOperatorEnum.And;
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedOrOperator() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.operatorType != LogicOperatorEnum.Or) {
			affectedGeneLogicBoxGate.operatorType = LogicOperatorEnum.Or;
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedDelete() {
		if (affectedGeneLogicBoxGate != null) {
			affectedGeneLogicBoxGate.isUsed = false;
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankLeft() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.TryMoveLeftFlankLeft()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedLeftFlankRight() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.TryMoveLeftFlankRight()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankRight() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.TryMoveRightFlankRight()) {
			motherPanel.UpdateConnections();
			motherPanel.MarkAsNewForge();
			motherPanel.MakeDirty();
		}
	}

	public void OnClickedRightFlankLeft() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.TryMoveRightFlankLeft()) {
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

			if (affectedGeneLogicBoxGate != null) {
				if (affectedGeneLogicBoxGate.GetTransmittingInputCount() <= 1) {
					operatorTypeLabel.text = "'" + affectedGeneLogicBoxGate.operatorType.ToString().ToLower() + "'" + (affectedGeneLogicBoxGate.isLocked ? " (L)" : "");
				} else {
					operatorTypeLabel.text = affectedGeneLogicBoxGate.operatorType.ToString().ToUpper() + (affectedGeneLogicBoxGate.isLocked ? " (L)" : "");
				}
				
				operatorTypeLabel.color = affectedGeneLogicBoxGate.isTransmittingSignal ? ColorScheme.instance.signalOff : Color.gray;
			} else {
				operatorTypeLabel.text = "???";
			}

			if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.isUsed) {
				//Scale and position
				GetComponent<RectTransform>().sizeDelta = new Vector2(LogicBoxPanel.cellWidth * (affectedGeneLogicBoxGate.rightFlank - affectedGeneLogicBoxGate.leftFlank), LogicBoxPanel.cellHeight);
				transform.position = motherPanel.gateGridOrigo + Vector3.right * affectedGeneLogicBoxGate.leftFlank * LogicBoxPanel.cellWidth + Vector3.down * affectedGeneLogicBoxGate.row * LogicBoxPanel.cellHeight;
			} else {
				transform.position = motherPanel.gateGridOrigo + Vector3.right * 1500f;
			}

			buttonOverlay.SetActive(isMouseHoverng);
			if (buttonOverlay && affectedGeneLogicBoxGate != null) {
				andButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.And ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
				orButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.Or ? ColorScheme.instance.selectedButtonBackground : ColorScheme.instance.notSelectedButtonBackground;
			}

			// input arrows
			foreach (Image i in inputArrows) {
				i.gameObject.SetActive(false);
			}

			int arrowIndex = 0;
			foreach (GeneLogicBoxPart connectedPart in affectedGeneLogicBoxGate.partsConnected) {
				inputArrows[arrowIndex].gameObject.SetActive(true);
				int targetLeftFlank = 0;
				int targetRightFlank = 0;
				if (row == 1 && connectedPart.row == 2) {
					targetLeftFlank = Mathf.Max(connectedPart.leftFlank, leftFlank);
					targetRightFlank = Mathf.Min(connectedPart.rightFlank, rightFlank);
				} else {
					targetLeftFlank = connectedPart.leftFlank;
					targetRightFlank = connectedPart.rightFlank;
				}
				inputArrows[arrowIndex].transform.position = motherPanel.gateGridOrigo + Vector3.right * (targetLeftFlank + targetRightFlank) * 0.5f * LogicBoxPanel.cellWidth + Vector3.down * ((row + 1) * LogicBoxPanel.cellHeight - 10f);
				inputArrows[arrowIndex].GetComponent<RectTransform>().sizeDelta = new Vector3(20f, Mathf.Max(20f, 20f + LogicBoxPanel.cellHeight * (connectedPart.row - row - 1)), 1f);

				Color arrowColor = Color.black;
				if (mode == PhenoGenoEnum.Genotype) {
					arrowColor = connectedPart.isTransmittingSignal ? ColorScheme.instance.signalOff : ColorScheme.instance.signalGrayedOut;
				} else {
					if (connectedPart.isTransmittingSignal) {
						if (connectedPart is GeneLogicBoxGate) {
							arrowColor = LogicBox.GetGateResult((connectedPart as GeneLogicBoxGate), selectedCell) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
						} else if (connectedPart is GeneLogicBoxInput) {
							arrowColor = LogicBox.GetInputResult((connectedPart as GeneLogicBoxInput), selectedCell) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
						}
					} else {
						arrowColor = ColorScheme.instance.signalGrayedOut;
					}
				}
				inputArrows[arrowIndex].GetComponent<Image>().color = arrowColor;

				arrowIndex++;
			}

			isDirty = false;
		}
	}

	private int leftFlank {
		get {
			return affectedGeneLogicBoxGate.leftFlank;
		}
	}

	private int rightFlank {
		get {
			return affectedGeneLogicBoxGate.rightFlank;
		}
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
