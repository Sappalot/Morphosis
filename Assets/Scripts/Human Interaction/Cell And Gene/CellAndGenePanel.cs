using System;
using System.Collections.Generic;
using UnityEngine;

// Will this one ever be nessesary??
public class CellAndGenePanel : MonoBehaviour {
	public RectTransform cellAndGenePanelRectTransform;

	public LogicBoxPanel eggFertilizeLogicBoxPanel;
	public SensorPanel eggEnergySensorPanel;

	public CellAndGeneWorkComponentPanel cellWorkComponentPanel;
	public EggCellPanel eggCellPanel;
	// TODO implement the rest of the panels all of them cellComponentPanels

	public HudSignalArrowHandler arrowHandler;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
		arrowHandler.Initialize(mode);
	}

	public List<GeneLogicBoxInput> GetAllGeneLogicBoxInputs() {
		if (cellWorkComponentPanel.cellType == CellTypeEnum.Egg) {
			return eggCellPanel.GetAllGeneGeneLogicBoxInputs();
		}
		return new List<GeneLogicBoxInput>();
	}

	public Vector3 TotalPanelOffset(SignalUnitEnum signalUnit, SignalUnitSlotEnum signalUnitSlot) {
		RectTransform outTransform = null;
		if (signalUnit == SignalUnitEnum.WorkLogicBoxA) {
			if (cellWorkComponentPanel.cellType == CellTypeEnum.Egg) {
				outTransform = eggFertilizeLogicBoxPanel.GetLocation(signalUnitSlot);
			}
		} else if (signalUnit == SignalUnitEnum.WorkSensorA) {
			//signalUnitSlot doesn't matter
			if (cellWorkComponentPanel.cellType == CellTypeEnum.Egg) {
				outTransform = eggEnergySensorPanel.GetLocation(signalUnitSlot);
			}
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
}