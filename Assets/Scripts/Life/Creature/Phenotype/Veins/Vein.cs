using UnityEngine;

public class Vein : MonoBehaviour {
	public GameObject mainArrow;

	private EdgeAttachment attachmentFront;
	private EdgeAttachment attachmentBack;

	public bool isPlacentaVein;

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

		isPlacentaVein = parentCell.creature.id != childCell.creature.id;

		if (!IsHighEfficiency(parentCell.GetCellType()) && !IsHighEfficiency(childCell.GetCellType())) {
			effectType = EffectEnum.LowLow;
		} else if ((!IsHighEfficiency(parentCell.GetCellType()) && IsHighEfficiency(childCell.GetCellType())) || (IsHighEfficiency(parentCell.GetCellType()) && !IsHighEfficiency(childCell.GetCellType()))) {
			effectType = EffectEnum.LowHigh;
		} else if (IsHighEfficiency(parentCell.GetCellType()) && IsHighEfficiency(childCell.GetCellType())) {
			effectType = EffectEnum.HighHigh;
		}
	}

	private bool IsHighEfficiency(CellTypeEnum cellType) {
		return cellType == CellTypeEnum.Vein;
	}

	public void UpdateGraphics(bool show) {
		//TODO: If draw wings && inside frustum
		if (show) {
			if (frontCell != null && backCell != null) {
				mainArrow.SetActive(true);
				//draw main
				mainArrow.GetComponent<LineRenderer>().SetPosition(1, frontCell.transform.position);  // front = start = 1
				mainArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position); // back = end = 0

				float intencity = Mathf.Abs(flowEffectFrontToBack);
				if (flowEffectFrontToBack > 0f) { //start to end

					if (!isPlacentaVein) {
						mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, 0f);
						mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, 0f);
					} else {
						mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, intencity);
						mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, intencity);
					}
					mainArrow.GetComponent<LineRenderer>().startWidth = Mathf.Max(0f, width - intencity); ;
					mainArrow.GetComponent<LineRenderer>().endWidth = width;
				} else {
					if (!isPlacentaVein) {
						mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, 0f);
						mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, 0f);
					} else {
						mainArrow.GetComponent<LineRenderer>().startColor = new Color(intencity, intencity, intencity);
						mainArrow.GetComponent<LineRenderer>().endColor = new Color(intencity, intencity, intencity);
					}

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
				return 0.2f;
			} else if (effectType == EffectEnum.LowHigh) {
				return 0.3f;
			} else if (effectType == EffectEnum.HighHigh) {
				return 0.4f;
			}
			return 0f;
		}
	}

	public float flowEffectFrontToBack;
	public void UpdateFluxEffect() {
		float moreEnergyFront = frontCell.energy - backCell.energy;
		flowEffectFrontToBack = Mathf.Clamp(moreEnergyFront, -100f, 100f) * flowEffectFactor;
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

	public void UpdateEnergy(int deltaTicks) {
		float deltaEnergyBack = flowEffectFrontToBack * deltaTicks * Time.fixedDeltaTime;
		backCell.energy += deltaEnergyBack;
		frontCell.energy -= deltaEnergyBack;
	}

	public void OnRecycle() {
		attachmentFront = null;
		attachmentBack = null;
		isPlacentaVein = false;

		mainArrow.gameObject.SetActive(false);
	}
}