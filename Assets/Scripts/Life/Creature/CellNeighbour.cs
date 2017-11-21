﻿using UnityEngine;

public class CellNeighbour
{
	public CellNeighbour(int cardinalIndex) {
		this.cardinalIndex = cardinalIndex;
		bindAngle = AngleUtil.CardinalIndexToAngle(cardinalIndex);
	}

	public Cell cell;
	public float angle; //worldSpace degrees, from me to this neighbour (as sen in world when creature is floating about, rotating )
	public float bindAngle; //genomeSpace degrees, from me to this neighbour (as seen in gene view, origin points up)
	public Vector3 coreToThis;
	public int cardinalIndex; //cardinal direction index

	public bool IsSameCreature(Cell other) {
		return other.creature == cell.creature;
	}
}

