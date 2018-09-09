using UnityEngine;
using UnityEngine.UI;

public class OriginCellPanel : MonoBehaviour {
	public Text detatchConditionsText;

	[HideInInspector]
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			detatchConditionsText.text = "Detatch when: -";
			detatchConditionsText.color = Color.gray;

			if (mode == PhenoGenoEnum.Phenotype && CellPanel.instance.selectedCell != null && CellPanel.instance.selectedCell.isOrigin) {
				detatchConditionsText.text = "Detatch when:";
				Cell originCell = CellPanel.instance.selectedCell;
				if (originCell.creature.creation != CreatureCreationEnum.Born) {
					detatchConditionsText.text = "Detatch when: Creature wasn't born ==> No detatch conditions";
				} else if (originCell.originDetatchMode == ChildDetatchModeEnum.Size) {
					detatchConditionsText.text = string.Format("Detatch when: Body size ≥ {0:F1}% which is {1:F0} of {2:F0} cells", originCell.originDetatchSizeThreshold * 100f, (Mathf.Clamp(Mathf.RoundToInt(originCell.originDetatchSizeThreshold * originCell.creature.genotype.geneCellCount), 1, originCell.creature.genotype.geneCellCount)), originCell.creature.genotype.geneCellCount);
				} else {
					detatchConditionsText.text = string.Format("Detatch when: Can't grow more and cell energy ≥ {0:F1}%", originCell.originDetatchEnergyThreshold * 100f);
				}
				detatchConditionsText.color = Color.black;
			}

			isDirty = false;
		}
	}
}