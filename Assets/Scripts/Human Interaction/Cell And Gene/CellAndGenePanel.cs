using System.Collections.Generic;
using UnityEngine;

public class CellAndGenePanel : MonoBehaviour {
	public RectTransform cellAndGenePanelRectTransform;

	public CellAndGeneOverviewPanel overvirewPanel;
	public CellAndGeneWorkComponentPanel workComponentPanel;
	public CellAndGeneAxonComponentPanel axonComponentPanel;
	public LogicBoxPanel dendritesComponentPanel;
	public SensorPanel energySensorComponentPanel;
	public CellAndGeneBuildPriorityComponentPanel buildPriorityComponentPanel;
	public CellAndGeneOriginComponentPanel originComponentPanel;
	public GeneNeighboursComponentPanel geneNeighbourComponentPanel;
	
	public HudSignalArrowHandler arrowHandler;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private bool isDirty = true;

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;

		overvirewPanel.Initialize(mode);
		workComponentPanel.Initialize(mode);
		axonComponentPanel.Initialize(mode);
		dendritesComponentPanel.Initialize(mode, SignalUnitEnum.Dendrites, false);
		energySensorComponentPanel.Initialize(mode, SignalUnitEnum.EnergySensor, false);

		buildPriorityComponentPanel.mode = mode;
		originComponentPanel.mode = mode;

		if (mode == PhenoGenoEnum.Genotype) {
			geneNeighbourComponentPanel.gameObject.SetActive(true);
			geneNeighbourComponentPanel.Initialize();
		} else {
			geneNeighbourComponentPanel.gameObject.SetActive(false);
		}
		

		arrowHandler.Initialize(mode);
	}

	public PhenoGenoEnum GetMode() {
		return mode;
	}

	public void MakeDirty() {
		isDirty = true;

		if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
			// no menu
			isDirty = false;
			return;
		}

		overvirewPanel.MakeDirty();

		workComponentPanel.MakeDirty();

		axonComponentPanel.MakeDirty();

		dendritesComponentPanel.MakeDirty();
		energySensorComponentPanel.MakeDirty();

		if (selectedGene.isOrigin) {
			originComponentPanel.MakeDirty();
		} else {
			buildPriorityComponentPanel.MakeDirty();
		}

		if (mode == PhenoGenoEnum.Genotype) {
			geneNeighbourComponentPanel.MakeDirty();
		}

		arrowHandler.MakeDirtyConnections();
		arrowHandler.MakeDirtySignal();
	}

	public List<GeneLogicBoxInput> GetAllGeneLogicBoxInputs() {
		List<GeneLogicBoxInput> inputList = new List<GeneLogicBoxInput>();
		if (workComponentPanel.cellType == CellTypeEnum.Egg) {
			inputList.AddRange(workComponentPanel.eggPanel.GetAllGeneGeneLogicBoxInputs());
		}
		inputList.AddRange(dendritesComponentPanel.GetAllGeneGeneLogicBoxInputs());
		return inputList;
	}

	public Vector3 TotalPanelOffset(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		RectTransform outTransform = null;

		//TODO: let egg cell define where all output locations are

		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			outTransform = workComponentPanel.nerveLocationsPanel.logicBoxA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorA) {
			outTransform = workComponentPanel.nerveLocationsPanel.sensorA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.Dendrites) {
			outTransform = dendritesComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			outTransform = energySensorComponentPanel.GetLocation(signalUnitSlot);
		}

		if (outTransform != null) {
			return TotalOffset(outTransform);
		} else {
			return Vector3.zero;
		}
	}

	private Vector2 TotalOffset(RectTransform rectTransform) {
		Vector3 offset = Vector2.zero;
		RectTransform currentRectTransform = rectTransform;
		for (int sane = 0; sane < 10; sane++) {
			if (currentRectTransform == cellAndGenePanelRectTransform) {
				break;
			}
			offset += currentRectTransform.localPosition;
			currentRectTransform = currentRectTransform.transform.parent.GetComponent<RectTransform>();
		}
		return offset;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GeneAndCellPanel");
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirty = false;
				return;
			}

			originComponentPanel.gameObject.SetActive(selectedGene.isOrigin);
			buildPriorityComponentPanel.gameObject.SetActive(!selectedGene.isOrigin);

			isDirty = false;
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