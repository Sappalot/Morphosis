using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : MetabolismCellPanel {

	public Text prayCellCount;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					prayCellCount.text = "Pray count: " + (CellPanel.instance.selectedCell as JawCell).prayCount;
					prayCellCount.color = Color.black;
				}				
			} else if (mode == PhenoGenoEnum.Genotype) {
				prayCellCount.text = "Pray count: -";
				prayCellCount.color = Color.gray;
			}

			isDirty = false;
		}
	}
}
