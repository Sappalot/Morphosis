using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class Cell : MonoBehaviour {
	[HideInInspector]
	public Gene gene;
	[HideInInspector]
	public FlipSideEnum flipSide;
	[HideInInspector]
	public Vector2i mapPosition = new Vector2i();
	[HideInInspector]
	public int buildOrderIndex = 0;
	[HideInInspector]
	public float lastTime = 0; //Last time muscle cell was updated
	[HideInInspector]
	public float radius = 0.5f;
	[HideInInspector]
	public float timeOffset;

	//  The direction the cell is facing in creature space
	public int bindCardinalIndex;
	public float heading; // where the cells flip triangle is pointing at the moment (0 is east, 90 is north ...)
	//public float angleDiffFromBindpose;

	public string id;
	public int groups = 0;
   

	public SpringJoint2D northSpring;
	public SpringJoint2D southEastSpring;
	public SpringJoint2D southWestSpring;
	public float springFrequenzy = 5f;
	public float springDamping = 11f;

	[HideInInspector]
	public Creature creature;

	public SpriteRenderer cellSelectedSprite; //transparent
	public SpriteRenderer triangleSprite;
	public SpriteRenderer openCircleSprite; //cell type colour
	public SpriteRenderer filledCircleSprite; //cell type colour
	public SpriteRenderer creatureSelectedSprite;
	public SpriteRenderer shadowSprite;

	public Transform triangleTransform;

	//  World space position
	public Vector2 position {
		get {
			return transform.position;
		}
	}

	//Set only once, when adding cell to CellMap
	public Vector2 modelSpacePosition;

	public void Show(bool show) {
		for (int index = 0; index < transform.childCount; index++) {
			transform.GetChild(index).gameObject.SetActive(show);
		}
	}

	public void ShowTriangle(bool show) {
		triangleSprite.enabled = show;
	}

	public void ShowOpenCircle(bool show) {
		openCircleSprite.enabled = show;
	}

	public void ShowFilledCircle(bool show) {
		filledCircleSprite.enabled = show;
	}

	public void ShowCreatureSelected(bool on) {
		creatureSelectedSprite.enabled = on;
	}

	public void ShowCellSelected(bool on) {
		cellSelectedSprite.enabled = on;
	}

	public void ShowShadow(bool on) {
		shadowSprite.enabled = on;
	}

	public void SetTringleHeadingAngle(float angle) {
		triangleTransform.rotation = Quaternion.Euler(0, 0, angle);
	}

	public void SetTringleFlipSide(FlipSideEnum flip) {
		triangleSprite.flipX = (flip == FlipSideEnum.WhiteBlack);
	}

	public Vector3 velocity {
		get { return this.GetComponent<Rigidbody2D>().velocity; }
		private set { }
	}

	[HideInInspector]
	public CellNeighbour northEastNeighbour =   new CellNeighbour(0);
	[HideInInspector]
	public CellNeighbour northNeighbour =       new CellNeighbour(1);
	[HideInInspector]
	public CellNeighbour northWestNeighbour =   new CellNeighbour(2);
	[HideInInspector]
	public CellNeighbour southWestNeighbour =   new CellNeighbour(3); 
	[HideInInspector]
	public CellNeighbour southNeighbour =       new CellNeighbour(4);
	[HideInInspector]
	public CellNeighbour southEastNeighbour =   new CellNeighbour(5);
   
	private Dictionary<int, CellNeighbour> index_Neighbour = new Dictionary<int, CellNeighbour>();
	private List<SpringJoint2D> springList = new List<SpringJoint2D>();

	private void Awake() {
		index_Neighbour.Add(0, northEastNeighbour);
		index_Neighbour.Add(1, northNeighbour);
		index_Neighbour.Add(2, northWestNeighbour);
		index_Neighbour.Add(3, southWestNeighbour);
		index_Neighbour.Add(4, southNeighbour);
		index_Neighbour.Add(5, southEastNeighbour);

		springList.Add(northSpring);
		springList.Add(southEastSpring);
		springList.Add(southWestSpring);

		ShowCreatureSelected(false);
	}

	public void EvoUpdate() {

	}

	public void EvoFixedUpdate(float fixedTime) {
		//Optimize further
		transform.rotation = Quaternion.identity; //dont turn the cells

		UpdateNeighbourVectors(); //costy, update only if cell has direction and is in frustum
		if (groups > 1) {
			TurnHingeNeighboursInPlace(); //optimize further
		}
		UpdateRotation(); //costy, update only if cell has direction and is in frustum
		UpdateFlipSide();

		UpdateRadius(fixedTime);
		UpdateSpringLengths(); // It is costy to update spring length
	}

	public void RemovePhysicsComponents() {

		SpringJoint2D[] springJoints = gameObject.GetComponents<SpringJoint2D>();
		foreach (SpringJoint2D springJoint in springJoints) {
			Destroy(springJoint);
		}
		Destroy(GetComponent<Rigidbody2D>());
	}

	public int GetDirectionOfNeighbourCell(Cell cell) {
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index) && GetNeighbour(index).cell == cell) {
				return index;
			}
		}
		return -1;
	}

	virtual public void UpdateRadius(float fixedTime) { }

	virtual public void UpdateSpringLengths() { }

	virtual public void UpdateSpringFrequenzy() { }

	virtual public bool IsContracting() {
		return false;
	}

	public void TurnHingeNeighboursInPlace() {
		//TODO Update turn springs only when nessesary 
        
		//CellNeighbour firstNeighbour = null;
		int springs = 0;
		Vector3 responceForce = new Vector3();
		for (int index = 0; index < 12; index++) {
			if(!HasNeighbourCell(index)) {
				continue;
			}
			if (!HasNeighbourCell(index + 1)) {
				//this(0)...empty(1)
				if (HasNeighbourCell(index + 2)) {
					//this(0)...empty(1)...full(2)
					responceForce += ApplyTorqueToPair(index, index+2);
					springs++;
					//if (springs >= groups - 1) {
					if (springs >= groups)
					{
						break;
					}
					//jump to where spring was attached
					index++; //+1
					continue;
				}
				else {
					//this(0)...empty(1)...empty(2)
					if (HasNeighbourCell(index + 3)) {
						//this(0)...empty(1)...empty(2)...full(3)
						responceForce += ApplyTorqueToPair(index, index + 3);
						springs++;
						//if (springs >= groups - 1) {
						if (springs >= groups)
						{
							break;
						}
                        
						//jump to where spring was attached
						index += 2; //+1
						continue;
					}
					else {
						//this(0)...empty(1)...empty(2)...empty(3)
						if (HasNeighbourCell(index + 4))
						{
							//this(0)...empty(1)...empty(2)...empty(3)...full(4)
							responceForce += ApplyTorqueToPair(index, index + 4);
							springs++;
							//if (springs >= groups - 1) {
							if (springs >= groups)
							{
								break;
							}

							//jump to where spring was attached
							index += 3; //+1
							continue;
						}
						else
						{
							//this(0)...empty(1)...empty(2)...empty(3)...empty(4)
							break; //if there is a "this + 4" aka "this-1" it must be connected to "this". if not then "this" is the only neibour and we would not be here in the first place
						}
					}
				} 
			}
		}
        
		GetComponent<Rigidbody2D>().AddForce(responceForce, ForceMode2D.Impulse);
	}

	//Applies forces to neighbour and returns reaction force to this
	Vector3 ApplyTorqueToPair(int alphaIndex, int betaIndex) {
		CellNeighbour alphaNeighbour = GetNeighbour(alphaIndex);
		CellNeighbour betaNeighbour = GetNeighbour(betaIndex);

		Vector3 alphaVector = alphaNeighbour.coreToThis; //Allways after = lower angle
		Vector3 betaVector = betaNeighbour.coreToThis; //Allways ahead = lower angle
		//float angle = Vector3.Angle(alphaVector, betaVector);
		float angle = LongAngle(alphaVector, betaVector);

		float goalAngle = AngleUtil.GetAngleDifference(alphaIndex, betaIndex);
		//Debug.Log(this.id + " Spring: " + alphaNeighbour.cell.id + "-" + betaNeighbour.cell.id + " = GoalA: " + goalAngle + " A: " + angle);
		float diff = (goalAngle - angle);


		float k1 = 0.00005f; //Works with RB mass 0.08
		float k2 = 0.00003f;
		//float k1 = 0.0005f;
		//float k2 = 0.0003f;
		float magnitude = k1 * diff + Mathf.Sign(diff) * k2 * diff * diff;
		Vector3 alphaForce = Vector3.Cross(alphaVector, new Vector3(0f, 0f, 1f)) * magnitude;
		Vector3 betaForce =  Vector3.Cross(betaVector, new Vector3(0f, 0f, -1f)) * magnitude;

		alphaNeighbour.cell.GetComponent<Rigidbody2D>().AddForce(alphaForce, ForceMode2D.Impulse);
		betaNeighbour.cell.GetComponent<Rigidbody2D>().AddForce(betaForce, ForceMode2D.Impulse);

		return -(alphaForce + betaForce);

	}

	float LongAngle(Vector3 after, Vector3 ahead) {
		float angle = Vector3.Angle(after, ahead);
		if (Vector3.Cross(after, ahead).z < 0) {
			angle = 360 - angle;
		}
		return angle;
	}

	public SpringJoint2D GetSpring(Cell askingCell) {
		if (HasNeighbour(CardinalEnum.north) && askingCell == northNeighbour.cell) {
			return northSpring;
		}
		else if (HasNeighbour(CardinalEnum.southEast) && askingCell == southEastNeighbour.cell) {
			return southEastSpring;
		}
		else if (HasNeighbour(CardinalEnum.southWest) && askingCell == southWestNeighbour.cell) {
			return southWestSpring;
		}
		return null;
	}

	public void SetNeighbourCell(CardinalEnum direction, Cell cell) {
		index_Neighbour[AngleUtil.CardinalEnumToCardinalIndex(direction)].cell = cell;
	}

	public Cell GetNeighbourCell(CardinalEnum direction) {
		return GetNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(direction));
	}

	public Cell GetNeighbourCell(int cardinalDirectionIndex) {
		return index_Neighbour[cardinalDirectionIndex % 6].cell;
	}

	public CellNeighbour GetNeighbour(CardinalEnum direction) {
		return GetNeighbour(AngleUtil.CardinalEnumToCardinalIndex(direction));
	}

	public CellNeighbour GetNeighbour(int index) {
		return index_Neighbour[index % 6];
	}

	public float GetRotation() {
		return transform.rotation.z;
	}

	public int GetNeighbourCount() {
		int count = 0;
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index))
				count++;
		}
		return count;
	}

	public bool HasNeighbour(CardinalEnum direction) {
		return HasNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(direction));
	}

	public bool HasNeighbourCell(int index) {
		return index_Neighbour[index % 6].cell != null;
	}

	public void SetOrderInLayer(int order) {
		SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer renderer in renderers) {
			renderer.sortingOrder = order;
		}
	}

	////  Updates world space rotation (heading) derived from neighbour position relative to this
	public void UpdateRotation() {
		if (mapPosition == new Vector2i() && neighbourCount == 0) {
			return;
		}
			

		UpdateNeighbourAngles();

		float angleDiffFromBindpose = 0f; 
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index)) {
				angleDiffFromBindpose = AngleUtil.GetAngleDifference(index_Neighbour[index].bindAngle, index_Neighbour[index].angle);
				break;
			}
		}
		heading = AngleUtil.CardinalIndexToAngle(bindCardinalIndex) + angleDiffFromBindpose;
		triangleTransform.localRotation = Quaternion.Euler(0f, 0f, heading);
	}

	public float angleDiffFromBindpose {
		get {
			return heading - AngleUtil.CardinalIndexToAngle(bindCardinalIndex);
		}
	}

	public void UpdateFlipSide() {
		SetTringleFlipSide(flipSide);
	}

	private void UpdateNeighbourAngles() {
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index)) {
				GetNeighbour(index).angle = FindAngle(GetNeighbour(index).coreToThis);
			}
		}
	}

	private int neighbourCount {
		get {
			int count = 0;
			for (int index = 0; index < 6; index++) {
				if (HasNeighbourCell(index)) {
					count++;
				}
			}
			return count;
		}
	}

	public void UpdateNeighbourVectors() {
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index)) {
				GetNeighbour(index).coreToThis = GetNeighbourCell(index).position - position;
			}
		}
	}

	public void UpdateSpringConnections() {
		if (northNeighbour.cell) {
			northSpring.connectedBody = northNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			northSpring.enabled = true;
		}
		else {
			northSpring.connectedBody = null;
			northSpring.enabled = false;
		}

		if (southWestNeighbour.cell) {
			southWestSpring.connectedBody = southWestNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			southWestSpring.enabled = true;
		}
		else {
			southWestSpring.connectedBody = null;
			southWestSpring.enabled = false;
		}

		if (southEastNeighbour.cell) {
			southEastSpring.connectedBody = southEastNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			southEastSpring.enabled = true;
		}
		else {
			southEastSpring.connectedBody = null;
			southEastSpring.enabled = false;
		}
	}

	public void UpdateGroups() {
		//TODO check if this cell is a hinge
		int groups = 0;
		bool lastWasNeighbor = false;
		bool neighbourFound = false;
		for (int index = 0; index < 7; index++) {
			if (HasNeighbourCell(index)) {
				neighbourFound = true;
				lastWasNeighbor = true;
			} else {
				if (lastWasNeighbor) { //down flank detected
					groups++;
				}
				lastWasNeighbor = false;
			}
		}
		if (neighbourFound && groups == 0) {
			groups = 1;
		}
		this.groups = groups;
	}

	public static float FindAngle(Vector2 direction) {
		float angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(direction.y) / Mathf.Abs(direction.x));

		if (direction.x > 0f) {
			if (direction.y < 0f)
				return 360f - angle;
			return angle;
		}
		if (direction.y > 0f)
			return 180f - angle;
		return 180f + angle;

		//return 180 + Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);

	}

	// Only for LMB
	private void OnMouseDown() {
		bool e = GetComponent<Collider2D>().enabled;
		if (Input.GetKey("mouse 0") && !EventSystem.current.IsPointerOverGameObject() && MouseAction.instance.actionState == MouseActionStateEnum.free) {
			if (Input.GetKey(KeyCode.LeftControl)) {
				if (CreatureSelectionPanel.instance.IsSelected(creature)) {
					CreatureSelectionPanel.instance.RemoveFromSelection(creature);
				} else {
					CreatureSelectionPanel.instance.AddToSelection(creature);
					creature.StoreState();
				}
			} else {
				if (CreatureSelectionPanel.instance.soloSelected != creature) {
					creature.StoreState();
				}
				CreatureSelectionPanel.instance.Select(creature, this);
				GenePanel.instance.MakeDirty();
				GenomePanel.instance.MakeDirty();
				GenomePanel.instance.MakeScrollDirty();
				CreatureSelectionPanel.instance.soloSelected.MakeDirty();
			}
		}
	}

	//data
	private CellData cellData = new CellData();
	public CellData UpdateData() {
		cellData.position = transform.position;
		cellData.heading = heading;
		cellData.bindCardinalIndex = bindCardinalIndex;
		cellData.geneIndex = gene.index;
		cellData.mapPosition = mapPosition;
		cellData.buildOrderIndex = buildOrderIndex;
		cellData.flipSide = flipSide;
		cellData.timeOffset = timeOffset;
		cellData.lastTime = lastTime;
		cellData.radius = radius;
		cellData.velocity = transform.GetComponent<Rigidbody2D>().velocity;
		return cellData;
	}

	public void ApplyData(CellData cellData, Creature creature) {
		transform.position = cellData.position;
		heading = cellData.heading;
		bindCardinalIndex = cellData.bindCardinalIndex;
		gene = creature.genotype.genome[cellData.geneIndex];
		mapPosition = cellData.mapPosition;
		buildOrderIndex = cellData.buildOrderIndex;
		flipSide = cellData.flipSide;
		timeOffset = cellData.timeOffset;
		lastTime = cellData.lastTime;
		radius = cellData.radius;
		transform.GetComponent<Rigidbody2D>().velocity = cellData.velocity;
		this.creature = creature;

	}

}