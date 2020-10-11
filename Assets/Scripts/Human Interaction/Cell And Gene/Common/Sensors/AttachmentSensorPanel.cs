using UnityEngine;
using UnityEngine.UI;

public class AttachmentSensorPanel : SignalUnitPanel {
	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update Attachment Sensor Panel");
			}

			isDirty = false;
		}
	}
}