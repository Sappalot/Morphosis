using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : MetabolismCellPanel {

	public Text exposure;

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}


			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					exposure.text = string.Format("Exposure: {0:F2}%", (CellPanel.instance.selectedCell as LeafCell).lowPassExposure * 100f);
					exposure.color = Color.black;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				exposure.text = "Exposure : -";
				exposure.color = Color.gray;
			}

			isDirty = false;
		}
	}
}
