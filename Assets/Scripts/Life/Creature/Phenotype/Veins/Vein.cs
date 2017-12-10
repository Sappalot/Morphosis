using UnityEngine;

public class Vein : MonoBehaviour {
	public GameObject mainArrow;

	private EdgeAttachment attachmentFront;
	private EdgeAttachment attachmentBack;

	public EffectEnum effectType = EffectEnum.LowLow;
	public enum EffectEnum {
		LowLow,
		LowHigh,
		HighHigh,
	}

	public Cell frontCell {
		get {
			return attachmentFront.cell;
		}
	}

	public Cell backCell {
		get {
			return attachmentBack.cell;
		}
	}

	public bool IsPair(Cell cellA, Cell cellB) {
		if ((cellA == frontCell && cellB == backCell) || (cellA == backCell && cellB == frontCell)) {
			return true;
		}
		return false;
	}

	public void Setup(Cell parentCell, Cell childCell, int directionChildToParentCell) {
		attachmentFront = new EdgeAttachment(parentCell, (directionChildToParentCell + 3) % 6);
		attachmentBack = new EdgeAttachment(childCell, directionChildToParentCell);

		if (!IsHighEfficiency(parentCell.GetCellType()) && !IsHighEfficiency(childCell.GetCellType())) {
			effectType = EffectEnum.LowLow;
		} else if ((!IsHighEfficiency(parentCell.GetCellType()) && IsHighEfficiency(childCell.GetCellType())) || (IsHighEfficiency(parentCell.GetCellType()) && !IsHighEfficiency(childCell.GetCellType()))) {
			effectType = EffectEnum.LowHigh;
		} else if (IsHighEfficiency(parentCell.GetCellType()) && IsHighEfficiency(childCell.GetCellType())) {
			effectType = EffectEnum.HighHigh;
		}
	}



	private bool IsHighEfficiency(CellTypeEnum cellType) {
		return (cellType == CellTypeEnum.Egg || cellType == CellTypeEnum.Vein);
	}

	public void UpdateGraphics(bool showVeins) {
		//TODO: If draw wings && inside frustum
		if ((GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.energy || GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.effect) && showVeins) {
			if (frontCell != null && backCell != null) {
				mainArrow.SetActive(true);
				//draw main
				mainArrow.GetComponent<LineRenderer>().SetPosition(1, frontCell.transform.position);  // front = start = 1
				mainArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position); // back = end = 0

				float intencity = Mathf.Abs(flowEffectFrontToBack) * 10f;
				if (flowEffectFrontToBack > 0f) { //start to end
					mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, intencity);
					mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, intencity);

					mainArrow.GetComponent<LineRenderer>().startWidth = Mathf.Max(0f, width - intencity); ;
					mainArrow.GetComponent<LineRenderer>().endWidth = width;
				} else {
					mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, intencity);
					mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, intencity);

					mainArrow.GetComponent<LineRenderer>().startWidth = width;
					mainArrow.GetComponent<LineRenderer>().endWidth = Mathf.Max(0f, width - intencity); ;
				}


			}
		} else {
			mainArrow.SetActive(false);
		}
	}

	private float width {
		get {
			if (effectType == EffectEnum.LowLow) {
				return 0.1f;
			} else if (effectType == EffectEnum.LowHigh) {
				return 0.2f;
			} else if (effectType == EffectEnum.HighHigh) {
				return 0.3f;
			}
			return 0f;
		}
	}

	private float flowEffectFrontToBack;
	public void UpdateEnergyFluxEffect() {
		float moreEnergyFront = frontCell.energy - backCell.energy;
		flowEffectFrontToBack = moreEnergyFront * flowEffectFactor;
	}
	private float flowEffectFactor {
		get {
			if (effectType == EffectEnum.LowLow) {
				return GlobalSettings.instance.phenotype.veinFluxEffectWeak;
			} else if (effectType == EffectEnum.LowHigh) {
				return GlobalSettings.instance.phenotype.veinFluxEffectMedium;
			} else {
				// HighHigh
				return GlobalSettings.instance.phenotype.veinFluxEffectStrong;
			}
		}
	}

	public void UpdateEnergy(float deltaTickTime) {
		float deltaEnergyBack = flowEffectFrontToBack * deltaTickTime;
		backCell.energy += deltaEnergyBack;
		frontCell.energy -= deltaEnergyBack;
	}
}