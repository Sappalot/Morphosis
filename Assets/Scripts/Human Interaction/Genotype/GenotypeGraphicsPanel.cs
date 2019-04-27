using UnityEngine;
using UnityEngine.UI;

public class GenotypeGraphicsPanel: MonoSingleton<GenotypeGraphicsPanel> {
	//Graphics

	public Dropdown graphicsGeneCellDropdown;
	public enum CellGraphicsEnum {
		type,
		buildOrder, //only selected
	}

	[HideInInspector]
	public CellGraphicsEnum graphicsGeneCell {
		get {
			return (CellGraphicsEnum)graphicsGeneCellDropdown.value;
		}
	}

	public void OnSelectionChanged() {

		for (int index = 0; index < CreatureSelectionPanel.instance.selection.Count; index++) {
			CreatureSelectionPanel.instance.selection[index].MakeDirtyGraphics();
		}

	}
}