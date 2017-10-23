using UnityEngine;

public class EggPanel : MonoBehaviour {

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell);
			

		}
	}
}
