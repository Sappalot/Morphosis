using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldViewportPanel : MonoBehaviour {
	public RectTransform bottomAndRightPanelsBlocking;
	public RectTransform bottomPanelBlocking;
	public RectTransform noPanelBlocking;

	public RectTransform usedViewportPanel {
		get {
			return bottomAndRightPanelsBlocking;

			if (CreatureSelectionPanel.instance.hasSelection) {
				if (CreatureSelectionPanel.instance.hasSoloSelected) {
					return bottomAndRightPanelsBlocking;
				} else {
					return bottomPanelBlocking;
				}
			} else {
				return noPanelBlocking;
			}
		}
	}
}
