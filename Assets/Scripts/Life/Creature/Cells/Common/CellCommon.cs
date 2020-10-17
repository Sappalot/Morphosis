using UnityEngine;
using UnityEngine.UI;

// All cells knows this one in order to handle common stuff
// Made to be able to change all (types) at once via the CellCommon prefab
// stuff that need not be overridden
public class CellCommon : MonoBehaviour {
	public SpriteRenderer creatureSelectedSprite;
	public SpriteRenderer cellSelected; //transparent
	public Transform rotatedRoot; // All under this root will be rotated according to heading 0 = east, 90 = north
	public SpriteRenderer triangleSprite;
	public CellBuds buds;
	public SpriteRenderer skelletonBone;
	public SpriteRenderer openCircleSprite; //cell type colour
	public SpriteRenderer filledCircleSprite; //cell type colour
	public SpriteRenderer shadowSprite;
	public Transform cellEye;
	public CellEyeZone cellEyeZone;
	public CellEyeBall cellEyeBall;
}
