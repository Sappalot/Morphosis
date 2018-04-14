public class EggCellPanel : MonoSingleton<EggCellPanel> {

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}
}