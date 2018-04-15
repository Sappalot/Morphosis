using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class Cell : MonoBehaviour {
	//------- Inspector

	public SpriteRenderer cellSelectedSprite; //transparent
	public SpriteRenderer triangleSprite;
	public SpriteRenderer openCircleSprite; //cell type colour
	public SpriteRenderer filledCircleSprite; //cell type colour
	public SpriteRenderer creatureSelectedSprite;
	public SpriteRenderer shadowSprite;

	public Transform triangleTransform;

	public SpringJoint2D northSpring;
	public SpringJoint2D southEastSpring;
	public SpringJoint2D southWestSpring;

	//----- ^ Inspector ^

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
	[HideInInspector]
	public int bindCardinalIndex; // where cells flip triangle is pointing in gene mode (0 is 30 degrees, N is 30 + N * 60 drgrees)

	[HideInInspector]
	public float heading; // where the cells flip triangle is pointing at the moment (0 is east, 90 is north ...)
						  //public float angleDiffFromBindpose;

	[HideInInspector]
	public string id;

	[HideInInspector]
	public int groups = 0;

	protected SpringJoint2D[] placentaSprings;

	private List<JawCell> predators = new List<JawCell>(); //Who is eating on me

	private int didUpdateFunctionThisFrame = 0;
	private int didUpdateEnergyThisFrame = 0;

	//---- Egg only
	[HideInInspector]
	public float eggCellFertilizeThreshold; // J

	[HideInInspector]
	public bool eggCellCanFertilizeWhenAttached;

	[HideInInspector]
	public ChildDetatchModeEnum eggCellDetatchMode;

	[HideInInspector]
	public float eggCellDetatchSizeThreshold; //J 

	[HideInInspector]
	public float eggCellDetatchEnergyThreshold; //J 
												//---- Egg only ^

	//---- Origin only
	[HideInInspector]
	public ChildDetatchModeEnum originDetatchMode;

	[HideInInspector]
	public float originDetatchSizeThreshold;

	[HideInInspector]
	public float originDetatchEnergyThreshold; // J  If we have more energy than this in the origin cell and it is attached with mother, it will separate
											   //   This amoutn is inherited from the mothers eggCell (eggCellSeparateThreshold), set at the moment of fertilization and can not be changed 
	//--- Origin only ^
	
	// Controlled by cell mouth of other creature
	public void AddPredator(JawCell predator) {
		if (!predators.Contains(predator)) {
			predators.Add(predator);
		}
	}

	// Controlled by cell mouth of other creature
	public void RemovePredator(JawCell predator) {
		if (predators.Contains(predator)) {
			predators.Remove(predator);
		}
	}

	public int predatorCount {
		get {
			return predators.Count;
		}
	}

	// metabolism
	[HideInInspector]
	public static float maxEnergy = 100f;

	[HideInInspector]
	private float m_energy;
	public float energy {
		get {
			return m_energy;
		}
		set {
			m_energy = value;
		}
	}


	[HideInInspector]
	public float effectProductionInternal = 0f;

	[HideInInspector]
	public float effectConsumptionInternal = 0;

	//How much am i stealing from other creatures
	[HideInInspector]
	public float effectProductionExternal;

	//How much damage are all predators inflicting on me?
	//Check with all Jaw cells eating on me they know and keep up to date
	public float effectConsumptionExternal {
		get {
			float loss = 0f;
			foreach (JawCell predator in predators) {
				loss += predator.GetPrayEatenEffect(this);
			}
			return loss;
		}
	}

	public float effectConsumption {
		get {
			return effectConsumptionInternal + effectConsumptionExternal;
		}
	}

	public float effectProduction {
		get {
			return effectProductionInternal + effectProductionExternal;
		}
	}

	//predatore vs pray
	public float effectExternal {
		get {
			return effectProductionExternal - effectConsumptionExternal;
		}
	}

	public float effect {
		get {
			return effectProduction - effectConsumption;
		}
	}

	public bool hasPlacentaSprings {
		get {
			return placentaSprings != null && placentaSprings.Length > 0;
		}
	}

	[HideInInspector]
	public float springFrequenzy = 5f;

	[HideInInspector]
	public float springDamping = 11f;

	[HideInInspector]
	public Creature m_creature;

	virtual public Creature creature {
		get {
			return m_creature;
		}
		set {
			m_creature = value;
		}
	}

	abstract public CellTypeEnum GetCellType();

	//  World space position
	public Vector2 position {
		get {
			return transform.position;
		}
	}

	public bool hasNeighbour {
		get {
			return neighbourCountAll > 0;
		}
	}

	public bool isOrigin {
		get {
			return mapPosition == new Vector2i(0, 0);
		}
	}

	[HideInInspector]
	public bool isPlacenta; // Note: a cell can be placenta of more than 1 child origins

	public void UpdatePlacentaSpringLengths() {
		foreach (SpringJoint2D placentaSpring in placentaSprings) {
			Cell motherPlacenta = placentaSpring.connectedBody.gameObject.GetComponent<Cell>();
			placentaSpring.distance = radius + motherPlacenta.radius;
		}		
	}

	// Total
	public int neighbourCountAll {
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

	public int neighbourCountOwn {
		get {
			int count = 0;
			for (int index = 0; index < 6; index++) {
				if (HasOwnNeighbourCell(index)) {
					count++;
				}
			}
			return count;
		}
	}

	public int neighbourCountConnectedRelatives {
		get {
			return neighbourCountAll - neighbourCountOwn;
		}
	}

	//Note: effecteConsumption external changes over time in the same manner for all cells but is integrated with different periods depending on cell
	public void UpdateEnergy(int deltaTicks, ulong worldTicks) {
		energy = Mathf.Min(energy + effect * deltaTicks * Time.fixedDeltaTime, maxEnergy);
		didUpdateEnergyThisFrame = 1;
	}

	virtual public void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		didUpdateFunctionThisFrame = 1; // Just for update visuals
	}

	virtual public void UpdateRadius(ulong fixedTime) { }

	virtual public void UpdateSpringLengths() { }

	virtual public void UpdateSpringFrequenzy() {
		if (placentaSprings != null) {
			for (int i = 0; i < placentaSprings.Length; i++) {
				SpringJoint2D spring = placentaSprings[i];
				spring.frequency = springFrequenzy;
				spring.dampingRatio = springDamping;
			}
		}
	}

	virtual public bool IsContracting() {
		return false;
	}

	public bool IsSameCreature(Cell other) {
		return other.creature == creature;
	}

	//Set only once, when adding cell to CellMap
	[HideInInspector]
	public Vector2 modelSpacePosition;

	public void Show(bool show) {
		for (int index = 0; index < transform.childCount; index++) {
			transform.GetChild(index).gameObject.SetActive(show);
		}
	}

	public void ShowTriangle(bool show) {
		triangleSprite.enabled = show;
	}

	public void SetTriangleColor(Color color) {
		triangleSprite.color = color;
	}

	public void ShowOpenCircle(bool show) {
		openCircleSprite.enabled = show;
	}

	public void ShowFilledCircle(bool show) {
		filledCircleSprite.enabled = show;
	}

	public void ShowOutline(bool show) {
		creatureSelectedSprite.gameObject.SetActive(show);
	}

	public void UpdateOutline(bool isHighlited) {
		if (isHighlited) {
			creatureSelectedSprite.color = ColorScheme.instance.outlineSelected;
		} else {
			if (creature != null) {
				creatureSelectedSprite.color = creature.phenotype.outlineClusterColor;
			} else {
				creatureSelectedSprite.color = ColorScheme.instance.outlineCluster;
			}
		}
	}

	public void SetOutlineColor(Color color) {
		creatureSelectedSprite.color = color;
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

	public Vector2 velocity {
		get {
			return GetComponent<Rigidbody2D>().velocity;
		}
	}

	[HideInInspector]
	public CellNeighbour northEastNeighbour =   new CellNeighbour(0);
	[HideInInspector]
	public CellNeighbour northNeighbour     =   new CellNeighbour(1);
	[HideInInspector]
	public CellNeighbour northWestNeighbour =   new CellNeighbour(2);
	[HideInInspector]
	public CellNeighbour southWestNeighbour =   new CellNeighbour(3); 
	[HideInInspector]
	public CellNeighbour southNeighbour     =   new CellNeighbour(4);
	[HideInInspector]
	public CellNeighbour southEastNeighbour =   new CellNeighbour(5);
   
	private Dictionary<int, CellNeighbour> cellNeighbourDictionary = new Dictionary<int, CellNeighbour>();


	public List<Cell> GetNeighbourCells() {
		List<Cell> cells = new List<Cell>();
		foreach (KeyValuePair<int, CellNeighbour> e in cellNeighbourDictionary) {
			if (e.Value.cell != null) {
				cells.Add(e.Value.cell);
			}
		}
		return cells;
	}

	private void Awake() {
		cellNeighbourDictionary.Add(0, northEastNeighbour);
		cellNeighbourDictionary.Add(1, northNeighbour);
		cellNeighbourDictionary.Add(2, northWestNeighbour);
		cellNeighbourDictionary.Add(3, southWestNeighbour);
		cellNeighbourDictionary.Add(4, southNeighbour);
		cellNeighbourDictionary.Add(5, southEastNeighbour);

		UpdateOutline(false);
	}

	public void RemoveCellNeighbours() {
		foreach (CellNeighbour neighbour in cellNeighbourDictionary.Values) {
			neighbour.cell = null;
		}
	}

	public void RemovePhysicsComponents() {

		SpringJoint2D[] springJoints = gameObject.GetComponents<SpringJoint2D>();
		foreach (SpringJoint2D springJoint in springJoints) {
			Destroy(springJoint);
		}
		Destroy(GetComponent<Rigidbody2D>());
	}

	public int GetDirectionOfOwnNeighbourCell(Creature me, Cell cell) {
		if (cell.creature != me) {
			Debug.LogError("Asking for neighbours on a cell outside of body");
		}
		for (int index = 0; index < 6; index++) {
			if (HasOwnNeighbourCell(index) && GetNeighbour(index).cell == cell) {
				return index;
			}
		}
		Debug.LogError("Could not find own of previous cell");
		return -1;
	}

	public void TurnHingeNeighboursInPlace() {
		//TODO Update turn springs only when nessesary 

		//CellNeighbour firstNeighbour = null;
		int springs = 0;
		Vector3 responceForce = new Vector3();
		for (int cardinalIndex = 0; cardinalIndex < 12; cardinalIndex++) {
			if(!HasNeighbourCell(cardinalIndex)) {
				continue;
			}
			if (!HasNeighbourCell(cardinalIndex + 1)) {
				//this(0)...empty(1)
				if (HasNeighbourCell(cardinalIndex + 2)) {
					//this(0)...empty(1)...full(2)
					responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex+2);
					springs++;
					//if (springs >= groups - 1) {
					if (springs >= groups) {
						break;
					}
					//jump to where spring was attached
					cardinalIndex++; //+1
					continue;
				}
				else {
					//this(0)...empty(1)...empty(2)
					if (HasNeighbourCell(cardinalIndex + 3)) {
						//this(0)...empty(1)...empty(2)...full(3)
						responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex + 3);
						springs++;
						//if (springs >= groups - 1) {
						if (springs >= groups)
						{
							break;
						}

						//jump to where spring was attached
						cardinalIndex += 2; //+1
						continue;
					}
					else {
						//this(0)...empty(1)...empty(2)...empty(3)
						if (HasNeighbourCell(cardinalIndex + 4))
						{
							//this(0)...empty(1)...empty(2)...empty(3)...full(4)
							responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex + 4);
							springs++;
							//if (springs >= groups - 1) {
							if (springs >= groups)
							{
								break;
							}

							//jump to where spring was attached
							cardinalIndex += 3; //+1
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
	private Vector3 ApplyTorqueToPair(int alphaIndex, int betaIndex) {
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
		if (HasOwnNeighbourCell(CardinalEnum.north) && askingCell == northNeighbour.cell) {
			return northSpring;
		}
		else if (HasOwnNeighbourCell(CardinalEnum.southEast) && askingCell == southEastNeighbour.cell) {
			return southEastSpring;
		}
		else if (HasOwnNeighbourCell(CardinalEnum.southWest) && askingCell == southWestNeighbour.cell) {
			return southWestSpring;
		}
		return null;
	}

	public void SetNeighbourCell(int cardinalIndex, Cell cell) {
		cellNeighbourDictionary[cardinalIndex].cell = cell;
	}

	public void RemoveNeighbourCell(Cell cell) {
		for (int i = 0; i < 6; i++) {
			if (cellNeighbourDictionary[i].cell == cell) {
				if (AngleUtil.CardinalIndexToCardinalEnum(i) == CardinalEnum.north) {
					if (northSpring != null) {
						northSpring.connectedBody = null;
						northSpring.enabled = false;
					}
				}
				else if (AngleUtil.CardinalIndexToCardinalEnum(i) == CardinalEnum.southWest) {
					if (southWestSpring != null) {
						southWestSpring.connectedBody = null;
						southWestSpring.enabled = false;
					}
				}
				else if (AngleUtil.CardinalIndexToCardinalEnum(i) == CardinalEnum.southEast) {
					if (southEastSpring != null) {
						southEastSpring.connectedBody = null;
						southEastSpring.enabled = false;
					}
				}

				cellNeighbourDictionary[i].cell = null;
			}
		}
	}

	public Cell GetNeighbourCell(int cardinalIndex) {
		return cellNeighbourDictionary[cardinalIndex % 6].cell;
	}

	private CellNeighbour GetNeighbour(int cardinalIndex) {
		return cellNeighbourDictionary[cardinalIndex % 6];
	}

	protected bool HasOwnNeighbourCell(CardinalEnum cardinalEnum) {
		return HasOwnNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(cardinalEnum));
	}

	public bool HasNeighbourCell(int cardinalIndex) {
		Cell neighbourCell = cellNeighbourDictionary[cardinalIndex % 6].cell;
		return neighbourCell != null;
	}

	private bool HasOwnNeighbourCell(int cardinalIndex) {
		Cell neighbourCell = cellNeighbourDictionary[cardinalIndex % 6].cell;
		return neighbourCell != null && neighbourCell.IsSameCreature(this);
	}

	////  Updates world space rotation (heading) derived from neighbour position relative to this
	public void UpdateRotation() {
		if (hasNeighbour) { // !(mapPosition == new Vector2i() && neighbourCount == 0)
			UpdateNeighbourAngles();

			float angleDiffFromBindpose = 0f; 
			for (int index = 0; index < 6; index++) {
				if (HasNeighbourCell(index)) {
					angleDiffFromBindpose = AngleUtil.GetAngleDifference(cellNeighbourDictionary[index].bindAngle, cellNeighbourDictionary[index].angle);
					break;
				}
			}
			heading = AngleUtil.CardinalIndexToAngle(bindCardinalIndex) + angleDiffFromBindpose;
		}

		//Debug.Log("Update arrow");
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

	public void UpdateNeighbourVectors() {
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index)) {
				GetNeighbour(index).coreToThis = GetNeighbourCell(index).position - position;
			}
		}
	}

	public void UpdateSpringConnectionsIntra() {
		// Intra creatures
		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.connectedBody = northNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			northSpring.enabled = true;
		}
		else {
			northSpring.connectedBody = null;
			northSpring.enabled = false;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.connectedBody = southWestNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			southWestSpring.enabled = true;
		}
		else {
			southWestSpring.connectedBody = null;
			southWestSpring.enabled = false;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.connectedBody = southEastNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
			southEastSpring.enabled = true;
		}
		else {
			southEastSpring.connectedBody = null;
			southEastSpring.enabled = false;
		}
	}

	//Phenotype
	public void UpdateSpringConnectionsInter(Creature creature) {
		// Here we connect origin cell to placenta of mother only
		if (placentaSprings != null) {
			for (int i = 0; i < placentaSprings.Length; i++) {
				Destroy(placentaSprings[i]);
			}
		}
		placentaSprings = new SpringJoint2D[0];

		if (this != creature.phenotype.originCell || !creature.IsAttachedToMother()) {
			return;
		}
		//This is creatures origin cell and creature has a connected mother

		List<Cell> placentaCells = new List<Cell>();
		for (int index = 0; index < 6; index++) {
			Cell neighbour = GetNeighbourCell(index);
			if (neighbour != null && neighbour.creature == creature.GetMother()) {
				placentaCells.Add(neighbour);
				neighbour.isPlacenta = true;
			}
		}

		placentaSprings = new SpringJoint2D[placentaCells.Count];
		for (int i = 0; i < placentaCells.Count; i++) {
			placentaSprings[i] = gameObject.AddComponent(typeof(SpringJoint2D)) as SpringJoint2D;
			placentaSprings[i].connectedBody = placentaCells[i].gameObject.GetComponent<Rigidbody2D>();
			placentaSprings[i].distance = 1f;
			placentaSprings[i].autoConfigureDistance = false; // Found ya! :)
		}
	}

	public void UpdateGroups(string motherId) {
		//TODO check if this cell is a hinge
		int groups = 0;
		bool lastWasNeighbor = false;
		bool neighbourFound = false;

		Creature lastHost = null;

		for (int index = 0; index < 7; index++) {
			if (HasNeighbourCell(index)) {
				Creature neighbourCreature = GetNeighbourCell(index).creature;

				if (lastWasNeighbor && isOrigin && ((neighbourCreature.id == motherId && lastHost == creature) || neighbourCreature == creature && lastHost.id == motherId)) {
					// When the mother origin is finding an adjacent child neighbour, she should just ignore it when it comes to groups
					groups++;
				}

				lastHost = neighbourCreature;

				neighbourFound = true;
				lastWasNeighbor = true;
			} else {
				if (lastWasNeighbor) { // down flank detected
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
				CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
			}
		}
	}

	// Save
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
		cellData.energy = energy;

		//Egg
		cellData.eggCellFertilizeThreshold =     eggCellFertilizeThreshold;
		cellData.eggCellDetatchMode =            eggCellDetatchMode;
		cellData.eggCellDetatchSizeThreshold =   eggCellDetatchSizeThreshold;
		cellData.eggCellDetatchEnergyThreshold = eggCellDetatchEnergyThreshold;

		// Origin
		cellData.originDetatchMode =            originDetatchMode;
		cellData.originDetatchSizeThreshold =   originDetatchSizeThreshold;
		cellData.originDetatchEnergyThreshold = originDetatchEnergyThreshold;

		return cellData;
	}

	// Load
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
		energy = cellData.energy;

		// Egg
		eggCellFertilizeThreshold =     cellData.eggCellFertilizeThreshold;
		eggCellDetatchMode =            cellData.eggCellDetatchMode;
		eggCellDetatchSizeThreshold =   cellData.eggCellDetatchSizeThreshold;
		eggCellDetatchEnergyThreshold = cellData.eggCellDetatchEnergyThreshold;

		// Origin
		originDetatchMode = cellData.originDetatchMode;
		originDetatchSizeThreshold = cellData.originDetatchSizeThreshold;
		originDetatchEnergyThreshold = cellData.originDetatchEnergyThreshold;

		this.creature = creature;
	}

	// Update

	//TODO: update cell graphics from here
	public void UpdateGraphics() {
		if (CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Phenotype) {
			if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.type) {
				if (creature.phenotype.visualTelefrag > 0) {
					filledCircleSprite.color = Color.white;
				} else {
					filledCircleSprite.color = ColorScheme.instance.ToColor(GetCellType());
				}
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.energy) {
				float life = energy / 100f;
				filledCircleSprite.color = ColorScheme.instance.cellGradientEnergy.Evaluate(life);
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.effect) {
				float effectValue = 0.5f + effect * 0.5f;
				filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.effectCreature) {
				float effectValue = 0.5f + (creature.phenotype.effect / creature.phenotype.cellCount) * 0.5f;
				filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.leafExposure) {
				if (GetCellType() == CellTypeEnum.Leaf) {
					float effectValue = (this as LeafCell).lowPassExposure;
					filledCircleSprite.color = ColorScheme.instance.cellGradientLeafExposure.Evaluate(effectValue);
				} else {
					filledCircleSprite.color = Color.blue;
				}
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.childCountCreature) {
				float value = 0.05f + creature.ChildrenCountIncDead() * 0.1f;
				filledCircleSprite.color = ColorScheme.instance.cellCreatureChildCount.Evaluate(value);
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.predatorPray) {
				float effectValue = 0.5f + effectExternal * 0.02f;
				filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
				if (effectExternal == 0f) {
					filledCircleSprite.color = Color.blue;
				} else if (GetCellType() == CellTypeEnum.Jaw && effectExternal < 0f) {
					filledCircleSprite.color = Color.white;
				} else {
					filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
				}
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.typeAndPredatorPray) {
				float effectValue = 0.5f + effectExternal * 0.02f;
				if (effectExternal == 0f) {
					filledCircleSprite.color = ColorScheme.instance.ToColor(GetCellType());
				} else if (GetCellType() == CellTypeEnum.Jaw && effectExternal < 0f) {
					filledCircleSprite.color = Color.white;
				} else {
					filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
				}
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.update) {
				filledCircleSprite.color = didUpdateFunctionThisFrame > 0 ? ColorScheme.instance.ToColor(GetCellType()) : Color.blue;
			}
			else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.creation) {
				if (creature.creation == CreatureCreationEnum.Born) {
					filledCircleSprite.color = Color.magenta;
				} else if (creature.creation == CreatureCreationEnum.Cloned) {
					filledCircleSprite.color = Color.yellow;
				} else {
					//forged
					filledCircleSprite.color = Color.cyan;
				}
			} else if (GlobalPanel.instance.graphicsCell == GlobalPanel.CellGraphicsEnum.individual) {
				filledCircleSprite.color = creature.phenotype.individualColor;
			}
		} else {
			filledCircleSprite.color = ColorScheme.instance.ToColor(GetCellType());
		}
		
	}

	public void UpdatePhysics() {
		//Optimize further
		transform.rotation = Quaternion.identity; //dont turn the cells

		UpdateNeighbourVectors(); //costy, update only if cell has direction and is in frustum
		if (groups > 1) {
			TurnHingeNeighboursInPlace(); //optimize further
		}
		UpdateRotation(); //costy, update only if cell has direction and is in frustum
		UpdateFlipSide();

		//Updated from muscle cells update instead
		//UpdateRadius(fixedTime);
		//UpdateSpringLengths(); // It is costy to update spring length

		//if (isTick && GlobalPanel.instance.effectsUpdateMetabolism.isOn) {
		//	UpdateMetabolism(deltaTickTime);
		//}

		didUpdateFunctionThisFrame--; //  Just for visuals
		didUpdateEnergyThisFrame--; //  Just for visuals
	}

	// ^ Update ^

	// Pooling
	//Phenotype only
	virtual public void OnRecycleCell() {
		//Predators, remembering me as pray
		foreach (JawCell predator in predators) {
			//Debug.Log("Removeing pray: " + this.creature.id + ", Cell: " + GetCellType() + " from " + predator);
			predator.RemovePray(this); // make jaw forget about me as a pray of his
									   //predators.Remove(predator);
		}

		//My own 3 springs to others
		if (northSpring != null) {
			northSpring.connectedBody = null;
			northSpring.enabled = false;
		}
		if (southEastSpring != null) {
			southEastSpring.connectedBody = null;
			southEastSpring.enabled = false;
		}
		if (southWestSpring != null) {
			southWestSpring.connectedBody = null;
			southWestSpring.enabled = false;
		}

		//My placenta springs
		if (placentaSprings != null) {
			foreach (SpringJoint2D placentaSpring in placentaSprings) {
				Destroy(placentaSpring);
			}
		}

		
		gene = null;
		id = "trash";
		predators.Clear();
		isPlacenta = false;
		groups = 0;

		lastTime = 0;
		timeOffset = 0;
		buildOrderIndex = 0;
			
	}

	// Pooling

	virtual public void OnBorrowToWorld() {}

	// ^ Pooling ^ 
}