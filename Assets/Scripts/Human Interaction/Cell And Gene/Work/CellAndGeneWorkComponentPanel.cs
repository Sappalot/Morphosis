using UnityEngine;
using UnityEngine.UI;

public class CellAndGeneWorkComponentPanel : MonoBehaviour {
	public Dropdown typeDropdown;

	[HideInInspector]
	public CellTypeEnum cellType {
		get {
			return (CellTypeEnum)typeDropdown.value;
		}
	}
}
