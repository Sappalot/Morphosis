﻿using UnityEngine;
using UnityEngine.UI;

public class LogicBoxGatePanel : MonoBehaviour {
	[HideInInspector]
	public bool isGhost;

	public Text operatorTypeLabel;
	public Image lockedOverlayImage;


	public Image background;
	public Image bigArrowUp;
	public Image lockedOverlay;
	public GameObject buttonOverlay;
	public Image andButtonImage;
	public Image orButtonImage;
	public Image nandButtonImage;
	public Image norButtonImage;

	public Image[] inputArrows = new Image[GeneLogicBox.columnCount];

	private bool isMouseHoverng;

	private int row;
	private int index;

	public GeneLogicBoxGate affectedGeneLogicBoxGate { 
		get {
			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				return null;
			}

			if (selectedGene.type == CellTypeEnum.Egg && motherPanel.signalUnitEnum == SignalUnitEnum.WorkLogicBoxA) {
				return selectedGene.eggCellFertilizeLogic.GetGate(row, index);
			} else if (motherPanel.signalUnitEnum == SignalUnitEnum.DendritesLogicBox) {
				return selectedGene.dendritesLogicBox.GetGate(row, index);
			}

			if (motherPanel.signalUnitEnum == SignalUnitEnum.OriginDetatchLogicBox) {
				return selectedGene.originDetatchLogicBox.GetGate(row, index);
			}

			return null;
		}
	}

	private LogicBoxPanel motherPanel;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private CellAndGenePanel cellAndGenePanel;


	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode, int row, int index, LogicBoxPanel motherPanel, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.motherPanel = motherPanel;
		this.row = row;
		this.index = index;
		this.cellAndGenePanel = cellAndGenePanel;
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;

	}

	public void OnPointerEnterArea() {
		if (isGhost) {
			return;
		}

		isMouseHoverng = (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && affectedGeneLogicBoxGate.lockness  == LocknessEnum.Unlocked);
		MakeDirty();
	}

	public void OnPointerExitArea() {
		if (isGhost) {
			return;
		}

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

	public void OnClickedNandOperator() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.operatorType != LogicOperatorEnum.Nand) {
			affectedGeneLogicBoxGate.operatorType = LogicOperatorEnum.Nand;
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedNorOperator() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.operatorType != LogicOperatorEnum.Nor) {
			affectedGeneLogicBoxGate.operatorType = LogicOperatorEnum.Nor;
			motherPanel.MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedDelete() {
		RemoveGate();
	}

	public void RemoveGate() {
		if (affectedGeneLogicBoxGate != null && affectedGeneLogicBoxGate.row > 0) {
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
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update Signal logic box");
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirty = false;
				return;
			}

			// ...ghost ...
			GetComponent<Image>().enabled = !isGhost;
			operatorTypeLabel.gameObject.SetActive(!isGhost);
			background.gameObject.SetActive(!isGhost);
			bigArrowUp.gameObject.SetActive(!isGhost);
			lockedOverlay.gameObject.SetActive(!isGhost);
			if (isGhost) {

				buttonOverlay.SetActive(false);

				foreach (Image i in inputArrows) {
					i.gameObject.SetActive(false);
				}

				return;
			}
			// ^ ghost ^ 

			if (affectedGeneLogicBoxGate != null) {
				if (affectedGeneLogicBoxGate.GetTransmittingInputCount() <= 1) {
					operatorTypeLabel.text = "'" + affectedGeneLogicBoxGate.operatorType.ToString().ToLower() + "'";
				} else {
					operatorTypeLabel.text = affectedGeneLogicBoxGate.operatorType.ToString().ToUpper();
				}
				if (mode == PhenoGenoEnum.Genotype) {
					lockedOverlayImage.gameObject.SetActive(affectedGeneLogicBoxGate.lockness == LocknessEnum.Locked);
				} else if (mode == PhenoGenoEnum.Phenotype) {
					lockedOverlayImage.gameObject.SetActive(false);
				}


				operatorTypeLabel.color = affectedGeneLogicBoxGate.isTransmittingSignal ? ColorScheme.instance.signalOff : ColorScheme.instance.signalUnused;
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
				andButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.And ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
				orButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.Or ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
				nandButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.Nand ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
				norButtonImage.color = affectedGeneLogicBoxGate.operatorType == LogicOperatorEnum.Nor ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
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
					// render arrows off if entire logic box is off
					arrowColor = (connectedPart.isTransmittingSignal && motherPanel.isAnyAffectedSignalUnitsRootedGenotype) ? ColorScheme.instance.signalOff : ColorScheme.instance.signalUnused;
				} else {
					if (connectedPart.isTransmittingSignal) {
						if (motherPanel.affectedSignalUnit.rootnessEnum == RootnessEnum.Rooted) {
							if (connectedPart is GeneLogicBoxGate) {
								arrowColor = LogicBox.HasSignalPostGate((connectedPart as GeneLogicBoxGate), selectedCell) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
							} else if (connectedPart is GeneLogicBoxInput) {
								arrowColor = LogicBox.HasSignalPostInputValve(selectedCell, motherPanel.signalUnitEnum, (connectedPart as GeneLogicBoxInput)) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
							}
						} else if (motherPanel.affectedSignalUnit.rootnessEnum == RootnessEnum.Rootable) {
							arrowColor = ColorScheme.instance.signalRootable;
						} else /*Unrooted*/ {
							arrowColor = ColorScheme.instance.signalUnused;
						}
					} else {
						arrowColor = ColorScheme.instance.signalUnused;
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
				return cellAndGenePanel.cell != null ? cellAndGenePanel.gene : null;
			} else {
				return cellAndGenePanel.gene;
			}
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
