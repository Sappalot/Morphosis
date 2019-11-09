using System;
using System.Collections.Generic;

[Serializable]
public class CellMapData {
	//public Dictionary<Vector2i, int> radiusAtGridPosition = new Dictionary<Vector2i, int>();
	// Cant seem to be able to serialize this dictionary. Don't know why
	// Implement Workaround instead
	public List<Vector2i> radiusAtGridPositionKey = new List<Vector2i>();
	public List<int> radiusAtGridPositionValue = new List<int>();


	public List<Vector2i> gridPositionsWithinRadius0 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius1 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius2 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius3 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius4 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius5 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius6 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius7 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius8 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius9 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius10 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius11 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius12 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius13 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius14 = new List<Vector2i>();
	public List<Vector2i> gridPositionsWithinRadius15 = new List<Vector2i>();

}