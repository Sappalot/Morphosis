using System.Collections.Generic;
using UnityEngine;

static class HistoryUtil {

	public static void SpawnAddCreatureEvent(int addedCount) {
		for (int a = 0; a < addedCount; a++) {
			World.instance.AddHistoryEvent(new HistoryEvent("+", false, Color.gray));
		}
	}
}