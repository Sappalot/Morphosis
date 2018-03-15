using UnityEngine.UI;

public class LockedUnlockedPanel : MonoSingleton<LockedUnlockedPanel> {

	public Text locked;
	public Text unLocked;

	private bool isDirty = false;

	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				if (CreatureSelectionPanel.instance.soloSelected.hasMotherSoul || CreatureSelectionPanel.instance.soloSelected.hasChildSoul) {
					locked.enabled = true;
					unLocked.enabled = false;
				} else {
					locked.enabled = false;
					unLocked.enabled = true;
				}
			} else {
				locked.enabled = false;
				unLocked.enabled = false;
			}

			isDirty = false;
		}
	}
}