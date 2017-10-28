using UnityEngine;

public class EggPanel : MonoBehaviour {

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			Life.instance.FertilizeCreature(CellPanel.instance.selectedCell, true);
		}
	}
}
