using System.Collections.Generic;
using UnityEngine;

public class CellAndGenePanel : MonoBehaviour {
	public RectTransform cellAndGenePanelRectTransform;

	public OverviewPanel overvirewPanel;
	public WorkPanel workPanel;
	public SensorPanel constantSensorPanel;
	public AxonPanel axonPanel;
	public LogicBoxPanel dendritesLogicBoxPanel;
	public SensorPanel energySensorPanel;
	public SensorPanel effectSensorPanel;
	public BuildPriorityPanel buildPriorityPanel;
	public OriginPanel originPanel;
	public GeneNeighboursPanel geneNeighboursPanel;
	
	public HudSignalArrowHandler hudSignalArrowHandler;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private bool isDirty = true;

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;

		overvirewPanel.Initialize(mode);

		workPanel.Initialize(mode);

		constantSensorPanel.Initialize(mode, SignalUnitEnum.ConstantSensor);

		axonPanel.Initialize(mode);

		// dendrites
		dendritesLogicBoxPanel.Initialize(mode, SignalUnitEnum.DendritesLogicBox);

		// sensors
		energySensorPanel.Initialize(mode, SignalUnitEnum.EnergySensor);
		effectSensorPanel.Initialize(mode, SignalUnitEnum.EffectSensor);

		// origin
		originPanel.Initialize(mode);

		// build priority
		buildPriorityPanel.mode = mode;


		if (mode == PhenoGenoEnum.Genotype) {
			geneNeighboursPanel.gameObject.SetActive(true);
			geneNeighboursPanel.Initialize();
		} else {
			geneNeighboursPanel.gameObject.SetActive(false);
		}
		

		hudSignalArrowHandler.Initialize(mode);
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
		workPanel.MakeDirty();
		constantSensorPanel.MakeDirty();
		axonPanel.MakeDirty();
		dendritesLogicBoxPanel.MakeDirty();
		energySensorPanel.MakeDirty();
		effectSensorPanel.MakeDirty();
		buildPriorityPanel.MakeDirty();

		if (selectedGene.isOrigin) {
			originPanel.MakeDirty();
		} else {
			buildPriorityPanel.MakeDirty();
		}

		if (mode == PhenoGenoEnum.Genotype) {
			geneNeighboursPanel.MakeDirty();
		}

		hudSignalArrowHandler.MakeDirtyConnections();
		hudSignalArrowHandler.MakeDirtySignal();
	}

	public List<IGeneInput> GetAllGeneInputs() {
		List<IGeneInput> inputList = new List<IGeneInput>();
		
		List<IGeneInput> inputs = workPanel.currentWorkPanel.GetAllGeneInputs();
		if (inputs != null) {
			inputList.AddRange(inputs);
		}

		inputs = axonPanel.GetAllGeneInputs();
		if (inputs != null) {
			inputList.AddRange(inputs);
		}

		inputs = dendritesLogicBoxPanel.GetAllGeneInputs();
		if (inputs != null) {
			inputList.AddRange(inputs);
		}		

		if (selectedGene.isOrigin) {
			inputs = originPanel.GetAllGeneInputs();
			if (inputs != null) {
				inputList.AddRange(inputs);
			}
		}

		return inputList;
	}

	public Vector3 TotalPanelOffset(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		RectTransform outTransform = null;


		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			outTransform = workPanel.nerveLocationsPanel.logicBoxA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkLogicBoxB) {
			outTransform = workPanel.nerveLocationsPanel.logicBoxB.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorA) {
			outTransform = workPanel.nerveLocationsPanel.sensorA.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorB) {
			outTransform = workPanel.nerveLocationsPanel.sensorB.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorC) {
			outTransform = workPanel.nerveLocationsPanel.sensorC.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.WorkSensorD) {
			outTransform = workPanel.nerveLocationsPanel.sensorD.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.ConstantSensor) {
			outTransform = constantSensorPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.Axon) {
			outTransform = axonPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.DendritesLogicBox) {
			outTransform = dendritesLogicBoxPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.EnergySensor) {
			outTransform = energySensorPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.EffectSensor) {
			outTransform = effectSensorPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.OriginDetatchLogicBox) {
			outTransform = originPanel.detatchLogicBoxPanel.GetLocation(signalUnitSlot);
		} else if (signalUnit == SignalUnitEnum.OriginSizeSensor) {
			outTransform = originPanel.sizeSensorPanel.GetLocation(signalUnitSlot);
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

			originPanel.gameObject.SetActive(selectedGene.isOrigin);
			buildPriorityPanel.gameObject.SetActive(!selectedGene.isOrigin);

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