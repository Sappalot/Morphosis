using System.Collections.Generic;
using UnityEngine;

public class CellAndGenePanel : MonoBehaviour {
	public RectTransform cellAndGenePanelRectTransform;

	public OverviewPanel overvirewPanel;
	public WorkPanel workPanel;
	public ConstantSensorPanel constantSensorPanel;
	public AxonPanel axonPanel;
	public LogicBoxPanel dendritesLogicBoxPanel;
	public EnergySensorPanel energySensorPanel;
	public EffectSensorPanel effectSensorPanel;
	public SurroundingSensorPanel surroundingSensorPanel;
	public BuildPriorityPanel buildPriorityPanel;
	public OriginPanel originPanel;
	public GeneNeighboursPanel geneNeighboursPanel;
	
	public HudSignalArrowHandler hudSignalArrowHandler;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	public bool isAuxiliary { get; private set; } // Only for genotype

	private bool isDirty = true;

	public void Initialize(PhenoGenoEnum mode, bool isAuxiliary) {
		this.isAuxiliary = isAuxiliary;
		this.mode = mode;

		overvirewPanel.Initialize(mode, this);

		workPanel.Initialize(mode, this);

		constantSensorPanel.Initialize(mode, SignalUnitEnum.ConstantSensor, this);

		axonPanel.Initialize(mode, this);

		// dendrites
		dendritesLogicBoxPanel.Initialize(mode, SignalUnitEnum.DendritesLogicBox, this);

		// sensors
		energySensorPanel.Initialize(mode, SignalUnitEnum.EnergySensor, this);
		effectSensorPanel.Initialize(mode, SignalUnitEnum.EffectSensor, this);
		surroundingSensorPanel.Initialize(mode, SignalUnitEnum.SurroundingSensor, this);

		// origin
		originPanel.Initialize(mode, this);

		// build priority
		buildPriorityPanel.Initialize(mode, this);


		if (mode == PhenoGenoEnum.Genotype) {
			geneNeighboursPanel.gameObject.SetActive(true);
			geneNeighboursPanel.Initialize(this);
		} else {
			geneNeighboursPanel.gameObject.SetActive(false);
		}
		

		hudSignalArrowHandler.Initialize(mode, this);
	}

	public void MakeDirty() {
		isDirty = true;
	}

	// new
	public List<Nerve> GetAllNerves() {
		List<Nerve> nerveList = new List<Nerve>();
		


		return nerveList;
	}

	public ViewXput? viewXput {
		get {
			if (workPanel.viewXput != null) {
				return workPanel.viewXput;
			}

			if (constantSensorPanel.viewXput != null) {
				return constantSensorPanel.viewXput;
			}

			if (axonPanel.viewXput != null) {
				return axonPanel.viewXput;
			}

			if (dendritesLogicBoxPanel.viewXput != null) {
				return dendritesLogicBoxPanel.viewXput;
			}

			if (energySensorPanel.viewXput != null) {
				return energySensorPanel.viewXput;
			}

			if (effectSensorPanel.viewXput != null) {
				return effectSensorPanel.viewXput;
			}

			if (surroundingSensorPanel.viewXput != null) {
				return surroundingSensorPanel.viewXput;
			}

			//buildPriorityPanel.MakeDirty();


			if (originPanel.viewXput != null) {
				return originPanel.viewXput;
			}

			//hudSignalArrowHandler.MakeDirtyConnections();
			//hudSignalArrowHandler.MakeDirtySignal();

			return null;
		}
	}

	// old
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

		if (gene.isOrigin) {
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
		} else if (signalUnit == SignalUnitEnum.SurroundingSensor) {
			outTransform = surroundingSensorPanel.GetLocation(signalUnitSlot);
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

	// Big trouble 2020-10-18, In genotype, when creating a nerve from one output to some other input that other input would not update (turning from ghost to signal off)
	// The reason for this was probably that the Make-dirty-functions (now in this function, before in this cass MakeDirty) was called earlier.
	// the call earlier was probably because some other Update function calling it was executed earlier
	// Stuff has worked up until now because this make dirty was called late (luckey)
	// By moving the MakeDirty functions into this function and making it Update =to=> LateUpdate
	// Stuff works again

	// I tried calling it frequently instead which worked but caused trouble for the nerve arrow xPut labels

	// The original idea eas to Let make dirty call other make dirty as much as they need
	// then everything would be updated in update, so NO make dirty inside of Update
	// Try to make it so, what a work :(
	private void LateUpdate() {
		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update GeneAndCellPanel");
			}

			//if ((mode == PhenoGenoEnum.Phenotype && cell == null) || (mode == PhenoGenoEnum.Genotype && gene == null)) {
			//	// no menu
			//	Debug.Log("No CellAndGeneCellMenu");

			//	//isDirty = false;
			//	return;
			//}

			overvirewPanel.MakeDirty();
			workPanel.MakeDirty();
			constantSensorPanel.MakeDirty();
			axonPanel.MakeDirty();
			dendritesLogicBoxPanel.MakeDirty();
			energySensorPanel.MakeDirty();
			effectSensorPanel.MakeDirty();
			surroundingSensorPanel.MakeDirty();
			buildPriorityPanel.MakeDirty();

			originPanel.isGhost = gene != null ? !gene.isOrigin : false;
			originPanel.MakeDirty();

			if (mode == PhenoGenoEnum.Genotype) {
				geneNeighboursPanel.MakeDirty();
			}

			hudSignalArrowHandler.MakeDirtyConnections();
			hudSignalArrowHandler.MakeDirtySignal();

			//originPanel.gameObject.SetActive(gene.isOrigin);
			//buildPriorityPanel.gameObject.SetActive(!gene.isOrigin);

			isDirty = false;
		}
	}

	private int panelTickPeriod;

	public Gene gene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				
				if (!isAuxiliary) {
					return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
				} else {
					return GeneAuxiliaryPanel.instance.viewedCell != null ? GeneAuxiliaryPanel.instance.viewedCell.gene : null;
				}
			} else {
				if (!isAuxiliary) {
					return GenePanel.instance.selectedGene;
				} else {
					return GeneAuxiliaryPanel.instance.viewedGene;
				}
			}
		}
	}

	public Cell cell {
		get {
			if (!isAuxiliary) {
				return CellPanel.instance.selectedCell;
			} else {
				return GeneAuxiliaryPanel.instance.viewedCell;
			}
		}
	}
}