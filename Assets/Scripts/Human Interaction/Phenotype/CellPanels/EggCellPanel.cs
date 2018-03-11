public class EggCellPanel : MonoSingleton<EggCellPanel> {
	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			Life.instance.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}
}