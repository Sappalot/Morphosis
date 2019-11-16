using System.Collections.Generic;
using UnityEngine;

public class CellAndGenePanel : MonoBehaviour {
	public RectTransform cellAndGenePanelRectTransform;

	public CellAndGeneOverviewPanel overvirewPanel;
	public CellAndGeneWorkComponentPanel workComponentPanel;
	public SensorPanel constantSensorComponentPanel;
	public CellAndGeneAxonComponentPanel axonComponentPanel;
	public LogicBoxPanel dendritesComponentPanel;
	public SensorPanel energySensorComponentPanel;
	public SensorPanel effectSensorComponentPanel;
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

		constantSensorComponentPanel.Initialize(mode, SignalUnitEnum.ConstantSensor);

		axonComponentPanel.Initialize(mode);

		// dendrites
		dendritesComponentPanel.Initialize(mode, SignalUnitEnum.DendritesLogicBox);

		// sensors
		energySensorComponentPanel.Initialize(mode, SignalUnitEnum.EnergySensor);
		effectSensorComponentPanel.Initialize(mode, SignalUnitEnum.EffectSensor);

		// origin
		originComponentPanel.Initialize(mode);

		// build priority
		buildPriorityComponentPanel.mode = mode;


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
		constantSensorComponentPanel.MakeDirty();
		axonComponentPanel.MakeDirty();
		dendritesComponentPanel.MakeDirty();
		energySensorComponentPanel.MakeDirty();
		effectSensorComponentPanel.MakeDirty();
		buildPriorityComponentPanel.MakeDirty();

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

	public List<IGeneInput> GetAllGeneInputs() {
		List<IGeneInput> inputList = new List<IGeneInput>();
		List<IGeneInput> inputs = workComponentPanel.currentWorkPanel.GetAllGeneInputs();
		if (inputs != null) {
			inputList.AddRange(inputs);
		}
		inputList.AddRange(axonComponentPanel.GetAllGeneInputs());
		inputList.AddRange(dendritesComponentPanel.GetAllGeneInputs());

		if (selectedGene.isOrigin) {
			inputs = originComponentPanel.GetAllGeneInputs();
			if (inputs != null) {
				inputList.AddRange(inputs);
			}
		}

		return inputList;
	}

	public Vector3 TotalPanelOffset(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		RectTransform outTransform = null;

		//TODO: let egg cell define where all output locations are

		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			outTransform = workComponentPanel.nerveLocationsPanel.logicBoxA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkLogicBoxB) {
			outTransform = workComponentPanel.nerveLocationsPanel.logicBoxB.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorA) {
			outTransform = workComponentPanel.nerveLocationsPanel.sensorA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorB) {
			outTransform = workComponentPanel.nerveLocationsPanel.sensorB.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorC) {
			outTransform = workComponentPanel.nerveLocationsPanel.sensorC.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorD) {
			outTransform = workComponentPanel.nerveLocationsPanel.sensorD.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.ConstantSensor) {
			outTransform = constantSensorComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.Axon) {
			outTransform = axonComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.DendritesLogicBox) {
			outTransform = dendritesComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			outTransform = energySensorComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.EffectSensor) {
			outTransform = effectSensorComponentPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.OriginDetatchLogicBox) {
			outTransform = originComponentPanel.detatchLogicBoxPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
			outTransform = originComponentPanel.sizeSensorPanel.GetLocation(signalUnitSlot);
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