using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flags : MonoBehaviour {

	public Flag flag;
	private List<Flag> flagPool = new List<Flag>();


	public void UpdateCanvas(Rect graphArea) {
		foreach (Flag f in flagPool) {
			f.UpdateCanvas(graphArea);
		}
		//flag.UpdateCanvas(graphArea);
	}

	private int oldPositionCount = 0;
	public void UpdateGraphics(Rect graphArea, float scale, short level, History history) {
		float levelScale = scale * Mathf.Pow(2f, level);
		int positionCount = Mathf.CeilToInt(graphArea.width / levelScale) + 1;
		if (positionCount != oldPositionCount) {
			oldPositionCount = positionCount;
		}

		// move all flags out of the view
		for (int i = 0; i < flagPool.Count; i++) {
			flagPool[i].SetPosition(graphArea, 1000 + i * 5f, true);
		}

		int flagCursor = 0;

		for (int i = 0; i < positionCount; i++) {
			int stepsAgo = (positionCount - 1) - i;
			if (history.GetRecord(level, stepsAgo).HasTag()) {
				Flag borrowedFlag = null;
				if (flagCursor >= flagPool.Count) {
					// Out of flags
					Flag newFlag = Instantiate(flag);
					newFlag.transform.parent = transform;
					newFlag.UpdateCanvas(graphArea);
					flagPool.Add(newFlag);
				}

				borrowedFlag = flagPool[flagCursor];

				bool draw = history.GetRecord(level, stepsAgo).tagShowLine; //history.GetRecord(level, stepsAgo).tag == "Big Bang";
				borrowedFlag.SetPosition(graphArea, - levelScale * stepsAgo, draw);
				borrowedFlag.text.text = history.GetRecord(level, stepsAgo).tagText;

				Color color = history.GetRecord(level, stepsAgo).color;
				borrowedFlag.text.color =      color;
				borrowedFlag.line.startColor = color;
				borrowedFlag.line.endColor =   color;

				flagCursor++;
			}
		}
	}
}