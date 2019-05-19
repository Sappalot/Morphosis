using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBuds : MonoBehaviour {
	public SpriteRenderer northEastBud;
	public SpriteRenderer northBud;
	public SpriteRenderer northWestBud;
	public SpriteRenderer southWestBud;
	public SpriteRenderer southBud;
	public SpriteRenderer southEastBud;

	public SpriteRenderer northEastPriority;
	public SpriteRenderer northBudPriority;
	public SpriteRenderer northWestBudPriority;
	public SpriteRenderer southWestBudPriority;
	public SpriteRenderer southBudPriority;
	public SpriteRenderer southEastBudPriority;

	private Dictionary<int, SpriteRenderer> buds = new Dictionary<int, SpriteRenderer>();
	private Dictionary<int, SpriteRenderer> priority = new Dictionary<int, SpriteRenderer>();

	public void Init() {
		buds.Add(0, northEastBud);
		buds.Add(1, northBud);
		buds.Add(2, northWestBud);
		buds.Add(3, southWestBud);
		buds.Add(4, southBud);
		buds.Add(5, southEastBud);

		priority.Add(0, northEastPriority);
		priority.Add(1, northBudPriority);
		priority.Add(2, northWestBudPriority);
		priority.Add(3, southWestBudPriority);
		priority.Add(4, southBudPriority);
		priority.Add(5, southEastBudPriority);
	}

	public void SetColorOfBud(int cardinalDirection, Color color) {
		buds[cardinalDirection].color = color;
	}

	public void SetEnabledBud(int cardinalDirection, bool enabled) {
		buds[cardinalDirection].enabled = enabled;
	}

	public void SetEnabledPriority(int cardinalDirection, bool enabled) {
		priority[cardinalDirection].enabled = enabled;
	}
}
