using UnityEngine;
using UnityEngine.UI;

public class AttachmentSensorPanel : SensorPanel {
	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Attachment Sensor Panel");
			}

			isDirty = false;
		}
	}
}