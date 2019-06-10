using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Boo.Lang.Runtime;

public abstract class Cell : MonoBehaviour {

	//------- Inspector
	public Signal signal;

	public SpriteRenderer cellSelected; //transparent
	public SpriteRenderer triangleSprite;
	public SpriteRenderer openCircleSprite; //cell type colour
	public SpriteRenderer filledCircleSprite; //cell type colour
	public SpriteRenderer creatureSelectedSprite;
	public SpriteRenderer shadowSprite;
	public Text label;
	public Canvas labelCanvas;

	public Transform rotatedRoot; // All under this root will be rotated according to heading 0 = east, 90 = north

	public SpringJoint2D northSpring;
	public SpringJoint2D southEastSpring;
	public SpringJoint2D southWestSpring;
	//----- ^ Inspector ^

	[HideInInspector]
	public Rigidbody2D theRigidBody;

	[HideInInspector]
	public Gene gene;

	[HideInInspector]
	public FlipSideEnum flipSide;

	[HideInInspector]
	public Vector2i mapPosition = new Vector2i();

	[HideInInspector]
	public int buildIndex = 0; // The order in which geneCells were developed from origin (also read as steps from origin along gene arrows)

	// Signal ...

	[HideInInspector]
	public Sensor sensor {
		get {
			return signal.sensor;
		}
	}

	[HideInInspector]
	public SensorTypeEnum sensorType {
		get {
			return signal.sensorType;
		}
	}

	// ^ Signal ^


	[HideInInspector]
	public float buildPriority { // the cell with the lowest(number) buildPriority will be grown first
		get {
			return buildIndex + gene.buildPriorityBias;
		}
	}

	[HideInInspector]
	public float lastTime = 0; //Last time muscle cell was updated

	[HideInInspector]
	public float radius = 0.5f;

	[HideInInspector]
	public float timeOffset; // unused

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

	private PhenoGenoEnum phenoGeno = PhenoGenoEnum.Void;

	protected SpringJoint2D[] placentaSprings; //only if i am an origo cell, the springs go to placenta cells on my mother

	private List<JawCell> predators = new List<JawCell>(); //Who is eating on me

	private int didUpdateFunctionThisFrame = 0;
	private int didUpdateEnergyThisFrame = 0;

	// Egg
	[HideInInspector]
	public ChildDetatchModeEnum eggCellDetatchMode;

	[HideInInspector]
	public float eggCellDetatchSizeThreshold; //part of creature (* 100 to get  %)

	[HideInInspector]
	public float eggCellDetatchEnergyThreshold; //part of max energy (* 100 to get  %) 

	// Egg only

	//---- Origin only
	[HideInInspector]
	public ChildDetatchModeEnum originDetatchMode;
	[HideInInspector]
	public float originDetatchSizeThreshold;
	[HideInInspector]
	public float originDetatchEnergyThreshold; // J ==> part of max energy (* 100 to get  %), If we have more energy than this in the origin cell and it is attached with mother, it will separate
											   //   This amoutn is inherited from the mothers eggCell (eggCellSeparateThreshold), set at the moment of fertilization and can not be changed 

	// Origin only
	[HideInInspector]
	public int originPulseTick = 0;

	public float originPulsePeriod {
		get {
			Debug.Assert(isOrigin);
			return gene.originPulsePeriodTicks * Time.fixedDeltaTime;
		}
	}

	public float originPulseFequenzy {
		get {
			Debug.Assert(isOrigin);
			return 1f / originPulsePeriod;
		}
	}

	public float originPulseCompleteness {
		get {
			Debug.Assert(isOrigin);
			return (float)originPulseTick / (float)gene.originPulsePeriodTicks;
		}
	}
	// ^ Origin only ^

	// Axon
	public bool isAxonEnabled {
		get {
			return gene.axonIsEnabled;
		}
	}

	public float GetAxonPulseValue(int distance) {
		float fromOriginOffset = (gene.axonFromOriginOffset + (gene.axonIsFromOriginPlus180 && flipSide == FlipSideEnum.WhiteBlack ? 180f : 0f)) / 360f;
		float fromMeOffest = (gene.axonFromMeOffset * distance) / 360f;
		if (!gene.axonIsReverse) {
			return Mathf.Cos((fromOriginOffset + fromMeOffest + creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + gene.axonRelaxContract;
		} else {
			return Mathf.Cos((fromOriginOffset + fromMeOffest - creature.phenotype.originCell.originPulseCompleteness) * 2f * Mathf.PI) + gene.axonRelaxContract; // is this really the right way of reversing????!!!!
		}
	}

	public bool IsAxonePulseContracting(int distance) {
		return isAxonEnabled && GetAxonPulseValue(distance) > 0;
	}

	// ^ Axon ^

	// Text ...

	public void SetLabelEnabled(bool enabled) {
		labelCanvas.gameObject.SetActive(enabled);
		label.gameObject.SetActive(enabled);
	}

	public void RemoveLabelCanvas() {
		Destroy(labelCanvas);
	}


	public void SetLabelText(string text) {
		label.text = text;
	}

	public void SetLabelColor(Color color) {
		label.color = color;
	}

	public void SetCorrectLabelOrientation() {
		labelCanvas.transform.rotation = Quaternion.identity;
	}

	// ^ Text ^

	// Buds...
	public CellBuds buds;

	// Only graphics, update only when cells has been built, removed or detatched
	public void UpdateBuds() {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			for (int worldCardinalIndex = 0; worldCardinalIndex < 6; worldCardinalIndex++) {
				int localCardinalIndex = AngleUtil.CardinalIndexRawToSafe(bindCardinalIndex + worldCardinalIndex - 1);
				Cell budCell = creature.genotype.GetCellAtGridPosition(CellMap.GetGridNeighbourGridPosition(mapPosition, localCardinalIndex));
				bool show = !HasNeighbourCell(localCardinalIndex) && budCell != null;
				buds.SetEnabledBud(worldCardinalIndex, show);
				buds.SetEnabledPriority(worldCardinalIndex, false);
				if (show) {
					CellNeighbour n = GetNeighbour(localCardinalIndex);
					if (n != null) {
						bool isPriorityBud = n.isPriorityBud; //Priority bud status should allready have been updated in phenotype
						buds.SetEnabledPriority(worldCardinalIndex, isPriorityBud);
						buds.SetColorOfBud(worldCardinalIndex, budCell.GetColor());
					}
				}
			}
		} else {
			for (int index = 0; index < 6; index++) {
				buds.SetEnabledBud(index, false);
				buds.SetEnabledPriority(index, false);
			}
		}
	}

	// ^ Buds ^


	// bleed particles...
	private List<ParticlesCellBleed> currentParticlesBleed = new List<ParticlesCellBleed>();

	// we don't want blood to linger with host while he is going in the recycling
	public void DetatchParticles() {
		Particles[] attachedParticles = GetComponentsInChildren<Particles>();
		foreach (Particles particles in attachedParticles) {
			particles.transform.position = Vector2.zero;
			particles.transform.parent = Morphosis.instance.transform;
		}
	}

	// ^ bleed particles ^

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
	public float energyFullness {
		get {
			return energy / GlobalSettings.instance.phenotype.cellMaxEnergy;
		}
	}

	// -------------------------------EFFECT -----------------------------------------

	public float GetEffect(bool production, bool stress, bool fluxSelf, bool fluxAttached) {
		return GetEffectUp(production, fluxSelf, fluxAttached) - GetEffectDown(production, stress, fluxSelf, fluxAttached);
	}

	public float GetEffectDown(bool production, bool stress, bool fluxSelf, bool fluxAttached) {
		// The effect lost by any cell when eate by jaw is counted under stress (effectProductionPredPrayDown)
		return (production ? effectProductionInternalDown : 0f) + (stress ? effectProductionPredPrayDown : 0f) + (fluxSelf ? effectFluxSelfDown : 0f) + (fluxAttached ? effectFluxAttachedDown : 0f);
	}

	public float GetEffectUp(bool production, bool fluxSelf, bool fluxAttached) {
		// The effect gained by jaw eating other is counted under production (effectProductionPredPrayUp)
		return (production ? effectProductionInternalUp + effectProductionPredPrayUp : 0f) + (fluxSelf ? effectFluxSelfUp : 0f) + (fluxAttached ? effectFluxAttachedUp : 0f);
	}

	[HideInInspector]
	public float effectProductionInternalUp = 0f; // production, excluding jaw

	[HideInInspector]
	public float effectProductionInternalDown = 0; // conumption, exclding jaw

	public float effectProductionPredPray {
		get {
			return effectProductionPredPrayUp - effectProductionPredPrayDown;
		}
	}

	//How much are my jaws stealing from other creatures, counts as production
	[HideInInspector]
	public float effectProductionPredPrayUp;

	//How much damage are all predators inflicting on me?
	//Check with all Jaw cells eating on me. They know and keep up to date
	public float effectProductionPredPrayDown {
		get {
			float loss = 0f;
			foreach (JawCell predator in predators) {
				loss += predator.GetPrayEatenEffect(this);
			}
			return loss;
		}
	}

	//net effect
	public float effectFluxSelf {
		get {
			return effectFluxFromSelf - effectFluxToSelf;
		}
	}

	[HideInInspector]
	public float effectFluxToSelf = 0; //energy i am giving to neighbours / time

	[HideInInspector]
	public float effectFluxFromSelf = 0f; //eneryg i am receiving from neighbours / time

	public float effectFluxSelfDown {
		get {
			return (effectFluxToSelf > 0f ? effectFluxToSelf : 0f) + (effectFluxFromSelf < 0f ? -effectFluxFromSelf : 0f); // Don't forget the brackets!!
		}
	}

	public float effectFluxSelfUp {
		get {
			return (effectFluxFromSelf > 0f ? effectFluxFromSelf : 0f) + (effectFluxToSelf < 0f ? -effectFluxToSelf : 0f);
		}
	}

	//--

	public float effectFluxAttached {
		get {
			return effectFluxFromMotherAttached - effectFluxToChildrenAttached;
		}
	}

	[HideInInspector]
	public float effectFluxFromMotherAttached = 0f; //eneryg i am receiving from mother / time

	[HideInInspector]
	public float effectFluxToChildrenAttached = 0; //energy i am giving to children / time

	// Will not be negative a negative value will appear in effectAttachedUp instead
	public float effectFluxAttachedDown {
		get {
			return (effectFluxToChildrenAttached > 0f ? effectFluxToChildrenAttached : 0f) + (effectFluxFromMotherAttached < 0f ? -effectFluxFromMotherAttached : 0f);
		}
	}

	// Will not be negative a negative value will appear in effectAttachedDown instead
	public float effectFluxAttachedUp {
		get {
			return (effectFluxToChildrenAttached < 0f ? -effectFluxToChildrenAttached : 0f) + (effectFluxFromMotherAttached > 0f ? effectFluxFromMotherAttached : 0f);
		}
	}
	// ^---------------- EFFECT ---------------------------^

	public virtual float armour {
		get {
			return gene.armour;
		}
	}

	public virtual float transparency {
		get {
			return 0f;
		}
	}

	public virtual void SetDefaultState() {
		energy = GlobalSettings.instance.phenotype.cellDefaultEnergy;
	}

	// Drag
	virtual public void SetNormalDrag() {
		theRigidBody.drag = GlobalSettings.instance.phenotype.normalDrag;
	}

	virtual public void SetSlideDrag() {
		theRigidBody.drag = GlobalSettings.instance.phenotype.slideDrag;
	}
	// ^ Drag ^

	public bool hasPlacentaSprings {
		get {
			return placentaSprings != null && placentaSprings.Length > 0;
		}
	}

	virtual public float springFrequenzy {
		get {
			return 5f;
		}
	}

	virtual public float springDamping {
		get {
			return 11f;
		}
	}

	virtual public Color GetColor(PhenoGenoEnum phenoGeno = PhenoGenoEnum.Genotype) {
		return ColorScheme.instance.ToColor(GetCellType());
	}

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
			placentaSpring.distance = radius + placentaSpring.connectedBody.gameObject.GetComponent<Cell>().radius;
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

	//production effect is updated by each cell type in their own way
	public void UpdateEnergy(int deltaTicks) {
		energy = Mathf.Clamp(energy + GetEffect(true, true, true, true) * deltaTicks * Time.fixedDeltaTime, -13f, GlobalSettings.instance.phenotype.cellMaxEnergy);
		didUpdateEnergyThisFrame = 2;
	}

	//Note function only called if extended cell type is enabled
	virtual public void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		didUpdateFunctionThisFrame = 5; // Just for update visuals
	}

	//Origin only
	public void UpdatePulse() {
		originPulseTick++;
		if (originPulseTick >= gene.originPulsePeriodTicks) {
			originPulseTick = 0;
		}
	}

	virtual public void UpdateRadius(ulong fixedTime) { }

	virtual public void UpdateSpringLengths() { }


	public void UpdateSpringFrequenzy() {
		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
			northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
			southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
			southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
		}
	}

	public void UpdateSpringsBreakingForce() {
		if (northSpring != null) {
			Cell neighbourCell = GetNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.north));
			if (GetCellType() == CellTypeEnum.Muscle || (neighbourCell != null && neighbourCell.GetCellType() == CellTypeEnum.Muscle)) {
				northSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForceMuscle;
			} else {
				northSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForce;
			}
		}
		if (southWestSpring != null) {
			Cell neighbourCell = GetNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.southWest));
			if (GetCellType() == CellTypeEnum.Muscle || (neighbourCell != null && neighbourCell.GetCellType() == CellTypeEnum.Muscle)) {
				southWestSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForceMuscle;
			} else {
				southWestSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForce;
			}
		}
		if (southEastSpring != null) {
			Cell neighbourCell = GetNeighbourCell(AngleUtil.CardinalEnumToCardinalIndex(CardinalEnum.southEast));
			if (GetCellType() == CellTypeEnum.Muscle || (neighbourCell != null && neighbourCell.GetCellType() == CellTypeEnum.Muscle)) {
				southEastSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForceMuscle;
			} else {
				southEastSpring.breakForce = GlobalSettings.instance.phenotype.springBreakingForce;
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
		//creatureSelectedSprite.gameObject.SetActive(show);
		creatureSelectedSprite.enabled = show;
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
		cellSelected.enabled = on;
	}

	private bool isOnTop = false;
	public void ShowOnTop(bool onTop) {
		shadowSprite.enabled = onTop;
		SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>(true);
		foreach (SpriteRenderer s in allRenderers) {
			if (isOnTop && !onTop) {
				s.sortingOrder = s.sortingOrder - 10;
			} else if (!isOnTop && onTop) {
				s.sortingOrder = s.sortingOrder + 10;
			}
		}
		if (isOnTop && !onTop) {
			labelCanvas.sortingOrder = labelCanvas.sortingOrder - 10;
		} else if (!isOnTop && onTop) {
			labelCanvas.sortingOrder = labelCanvas.sortingOrder + 10;
		}

		if (!onTop) {
			transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		} else {
			transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
		}
		isOnTop = onTop;
	}

	public void SetTringleHeadingAngle(float angle) {
		rotatedRoot.rotation = Quaternion.Euler(0, 0, angle);
	}

	public void SetTringleFlipSide(FlipSideEnum flip) {
		triangleSprite.flipX = (flip == FlipSideEnum.WhiteBlack);
	}

	public Vector2 velocity {
		get {
			Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
			if (rb != null) {
				return gameObject.GetComponent<Rigidbody2D>().velocity;
			} else {
				return Vector2.zero;
			}

		}
	}

	[HideInInspector]
	public CellNeighbour northEastNeighbour = new CellNeighbour(0);
	[HideInInspector]
	public CellNeighbour northNeighbour = new CellNeighbour(1);
	[HideInInspector]
	public CellNeighbour northWestNeighbour = new CellNeighbour(2);
	[HideInInspector]
	public CellNeighbour southWestNeighbour = new CellNeighbour(3);
	[HideInInspector]
	public CellNeighbour southNeighbour = new CellNeighbour(4);
	[HideInInspector]
	public CellNeighbour southEastNeighbour = new CellNeighbour(5);

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
		Init();
	}

	public void Init() {
		theRigidBody = GetComponent<Rigidbody2D>();

		cellNeighbourDictionary.Add(0, northEastNeighbour);
		cellNeighbourDictionary.Add(1, northNeighbour);
		cellNeighbourDictionary.Add(2, northWestNeighbour);
		cellNeighbourDictionary.Add(3, southWestNeighbour);
		cellNeighbourDictionary.Add(4, southNeighbour);
		cellNeighbourDictionary.Add(5, southEastNeighbour);

		UpdateOutline(false);
		if (buds != null) {
			buds.Init();
		}

		// Sensors...
		signal.Init(this);

		// ^ Sensors ^
	}

	public void Setup(PhenoGenoEnum phenoGeno) {
		this.phenoGeno = phenoGeno;
		SetLabelEnabled(phenoGeno == PhenoGenoEnum.Genotype);
		//if (phenoGeno == PhenoGenoEnum.Phenotype) {
		//	Destroy(labelCanvas.gameObject);
		//}
	}

	public void RemoveCellNeighbours() {
		foreach (CellNeighbour neighbour in cellNeighbourDictionary.Values) {
			neighbour.cell = null;
		}
	}

	public void DisablePhysicsComponents() {
		//My own 3 springs to others
		if (northSpring != null) {
			northSpring.enabled = false;
		}
		if (southEastSpring != null) {
			southEastSpring.enabled = false;
		}
		if (southWestSpring != null) {
			southWestSpring.enabled = false;
		}

		//My placenta springs
		if (hasPlacentaSprings) {
			foreach (SpringJoint2D placentaSpring in placentaSprings) {
				placentaSpring.enabled = false;
			}
		}

		theRigidBody.simulated = false;
		//theRigidBody.bodyType = RigidbodyType2D.Static;
	}

	public void EnablePhysicsComponents() {
		//theRigidBody.simulated = true;
		//theRigidBody.bodyType = RigidbodyType2D.Dynamic;
	}

	public void RemovePhysicsComponents() {
		CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
		Destroy(collider);

		//Destroy hexagonal springs and placenta springs
		SpringJoint2D[] springJoints = gameObject.GetComponents<SpringJoint2D>();
		foreach (SpringJoint2D springJoint in springJoints) {
			Destroy(springJoint);
		}
		placentaSprings = new SpringJoint2D[0];

		Destroy(theRigidBody);
	}

	public int GetDirectionOfOwnNeighbourCell(Creature me, Cell cell) {
		if (cell.creature != me) {
			throw new RuntimeException("Asking for neighbours on a cell outside of body");
		}
		for (int index = 0; index < 6; index++) {
			if (HasOwnNeighbourCell(index) && GetNeighbour(index).cell == cell) {
				return index;
			}
		}
		// Once in a blue moon we end up here without finding 
		throw new RuntimeException("Could not find own of previous cell");
	}

	public void TurnHingeNeighboursInPlace() {
		//TODO Update turn springs only when nessesary 

		//CellNeighbour firstNeighbour = null;
		int springs = 0;
		Vector3 responceForce = new Vector3();
		for (int cardinalIndex = 0; cardinalIndex < 12; cardinalIndex++) {
			if (!HasNeighbourCell(cardinalIndex)) {
				continue;
			}
			if (!HasNeighbourCell(cardinalIndex + 1)) {
				//this(0)...empty(1)
				if (HasNeighbourCell(cardinalIndex + 2)) {
					//this(0)...empty(1)...full(2)
					responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex + 2);
					springs++;
					//if (springs >= groups - 1) {
					if (springs >= groups) {
						break;
					}
					//jump to where spring was attached
					cardinalIndex++; //+1
					continue;
				} else {
					//this(0)...empty(1)...empty(2)
					if (HasNeighbourCell(cardinalIndex + 3)) {
						//this(0)...empty(1)...empty(2)...full(3)
						responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex + 3);
						springs++;
						//if (springs >= groups - 1) {
						if (springs >= groups) {
							break;
						}

						//jump to where spring was attached
						cardinalIndex += 2; //+1
						continue;
					} else {
						//this(0)...empty(1)...empty(2)...empty(3)
						if (HasNeighbourCell(cardinalIndex + 4)) {
							//this(0)...empty(1)...empty(2)...empty(3)...full(4)
							responceForce += ApplyTorqueToPair(cardinalIndex, cardinalIndex + 4);
							springs++;
							//if (springs >= groups - 1) {
							if (springs >= groups) {
								break;
							}

							//jump to where spring was attached
							cardinalIndex += 3; //+1
							continue;
						} else {
							//this(0)...empty(1)...empty(2)...empty(3)...empty(4)
							break; //if there is a "this + 4" aka "this-1" it must be connected to "this". if not then "this" is the only neibour and we would not be here in the first place
						}
					}
				}
			}
		}

		theRigidBody.AddForce(responceForce, ForceMode2D.Impulse);
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
		Vector3 betaForce = Vector3.Cross(betaVector, new Vector3(0f, 0f, -1f)) * magnitude;

		alphaNeighbour.cell.theRigidBody.AddForce(alphaForce, ForceMode2D.Impulse);
		betaNeighbour.cell.theRigidBody.AddForce(betaForce, ForceMode2D.Impulse);

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
		} else if (HasOwnNeighbourCell(CardinalEnum.southEast) && askingCell == southEastNeighbour.cell) {
			return southEastSpring;
		} else if (HasOwnNeighbourCell(CardinalEnum.southWest) && askingCell == southWestNeighbour.cell) {
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
				} else if (AngleUtil.CardinalIndexToCardinalEnum(i) == CardinalEnum.southWest) {
					if (southWestSpring != null) {
						southWestSpring.connectedBody = null;
						southWestSpring.enabled = false;
					}
				} else if (AngleUtil.CardinalIndexToCardinalEnum(i) == CardinalEnum.southEast) {
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

	public CellNeighbour GetNeighbour(int cardinalIndex) {
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
	public void UpdateHeading() {
		//Need neighbour vectors to be updated

		if (hasNeighbour) {
			UpdateNeighbourAngles(); // These need to be updated all att once, dont calculate them inline, when calculating angleDiffFromBindpose

			float angleDiffFromBindpose = 0f;

			//check cell parent first
			int parentCardinalIndex = AngleUtil.CardinalIndexRawToSafe(bindCardinalIndex + 3);
			if (HasNeighbourCell(parentCardinalIndex)) {
				angleDiffFromBindpose = AngleUtil.GetAngleDifference(cellNeighbourDictionary[parentCardinalIndex].bindAngle, cellNeighbourDictionary[parentCardinalIndex].angle);
			} else {
				// We have some flickering +180 degrees issue, mabeee only when using an average of several angles
				for (int index = 0; index < 6; index++) {
					if (HasNeighbourCell(index)) {
						angleDiffFromBindpose += AngleUtil.GetAngleDifference(cellNeighbourDictionary[index].bindAngle, cellNeighbourDictionary[index].angle);
						break;
					}
				}
			}

			heading = AngleUtil.CardinalIndexToAngle(bindCardinalIndex) + angleDiffFromBindpose;
		}

		//Debug.Log("Update arrow");
		rotatedRoot.localRotation = Quaternion.Euler(0f, 0f, heading);
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
			if (HasNeighbourCell(index)/* && GetNeighbour(index).isAngleDirty*/) {
				GetNeighbour(index).angle = FindAngle(GetNeighbour(index).coreToThis);

				//Update other creatures neighbour (that is me) since we have the angle
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

	void OnJointBreak2D(Joint2D brokenJoint) {
		World.instance.life.KillCreatureByBreaking(creature, true);
	}

	public void RepairBrokenSprings() {
		if (northSpring == null) {
			northSpring = CreateSpring();
		}
		if (southWestSpring == null) {
			southWestSpring = CreateSpring();
		}
		if (southEastSpring == null) {
			southEastSpring = CreateSpring();
		}
	}
	private SpringJoint2D CreateSpring() {
		SpringJoint2D spring = gameObject.AddComponent(typeof(SpringJoint2D)) as SpringJoint2D;
		spring.enableCollision = false;
		spring.autoConfigureConnectedAnchor = false;
		spring.autoConfigureDistance = false;
		spring.distance = 1f;
		spring.connectedBody = null;
		return spring;
	}

	public void UpdateSpringConnectionsIntra() {
		// Intra creatures
		if (HasOwnNeighbourCell(CardinalEnum.north)) {
			northSpring.connectedBody = northNeighbour.cell.theRigidBody;
			northSpring.enabled = true;
		} else {
			northSpring.connectedBody = null;
			northSpring.enabled = false;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
			southWestSpring.connectedBody = southWestNeighbour.cell.theRigidBody;
			southWestSpring.enabled = true;
		} else {
			southWestSpring.connectedBody = null;
			southWestSpring.enabled = false;
		}

		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
			southEastSpring.connectedBody = southEastNeighbour.cell.theRigidBody;
			southEastSpring.enabled = true;
		} else {
			southEastSpring.connectedBody = null;
			southEastSpring.enabled = false;
		}
	}

	//Phenotype
	public void UpdatePlacentaSpringConnections(Creature creature) {
		// Here we connect origin cell to placenta of mother only
		if (placentaSprings != null) {
			for (int i = 0; i < placentaSprings.Length; i++) {
				Destroy(placentaSprings[i]);
			}
		}
		placentaSprings = new SpringJoint2D[0];

		if (this != creature.phenotype.originCell || !creature.IsAttachedToMotherAlive()) {
			return;
		}
		//This is creatures origin cell and creature has a connected mother

		List<Cell> placentaCells = new List<Cell>();
		for (int index = 0; index < 6; index++) {
			Cell neighbour = GetNeighbourCell(index);
			if (neighbour != null && neighbour.creature == creature.GetMotherAlive()) {
				placentaCells.Add(neighbour);
				neighbour.isPlacenta = true;
			}
		}

		placentaSprings = new SpringJoint2D[placentaCells.Count];
		for (int i = 0; i < placentaCells.Count; i++) {
			placentaSprings[i] = gameObject.AddComponent(typeof(SpringJoint2D)) as SpringJoint2D;
			placentaSprings[i].connectedBody = placentaCells[i].theRigidBody;
			placentaSprings[i].distance = 1f;
			placentaSprings[i].autoConfigureDistance = false; // Found ya! :)

			placentaSprings[i].frequency = (springFrequenzy + placentaCells[i].springFrequenzy) / 2f;
			placentaSprings[i].dampingRatio = (springDamping + placentaCells[i].springDamping) / 2f;
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

				if (lastWasNeighbor && lastHost != neighbourCreature && lastHost != creature && neighbourCreature != creature) {
					// A neighbour child was found which should not stick with previous neighbour child
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

	//public void SetEnabledAllGraphics(bool enabled) {
	//	cellSelected.gameObject.SetActive(enabled); //transparent
	//	triangleSprite.gameObject.SetActive(enabled);
	//	openCircleSprite.gameObject.SetActive(enabled); //cell type colour
	//	filledCircleSprite.gameObject.SetActive(enabled); //cell type colour
	//	creatureSelectedSprite.gameObject.SetActive(enabled);
	//	shadowSprite.gameObject.SetActive(enabled);

	//	buds.gameObject.SetActive(enabled);

	//	labelCanvas.gameObject.SetActive(enabled);
	//}

	// Update
	private bool graphicsWasDisabled;


	//TODO: update cell graphics from here
	public void UpdateGraphics(bool mayBeSelected) {
		//if (!graphicsWasDisabled) {
		//	DisableGraphics();
		//	buds.DisableGraphics();
		//	graphicsWasDisabled = true;
		//}
		//return;

		// Selector spin
		if (mayBeSelected) {
			cellSelected.transform.Rotate(0f, 0f, -Time.unscaledDeltaTime * 90f);
		}

		openCircleSprite.color = GetColor();

		// Main 2 Circles
		if (phenoGeno == PhenoGenoEnum.Phenotype) {
			SetLabelEnabled(false);

			if (creature.creation == CreatureCreationEnum.Frozen) {
				filledCircleSprite.color = GetColor();
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.type) {
				if (creature.phenotype.visualTelepoke > 0) {
					filledCircleSprite.color = Color.white;
				} else {
					if (IsHibernating()) {
						filledCircleSprite.color = Color.black;
					} else {
						if (GetCellType() != CellTypeEnum.Jaw) {
							filledCircleSprite.color = GetColor(PhenoGenoEnum.Phenotype);
						} else {
							filledCircleSprite.color = GetColor(PhenoGenoEnum.Phenotype);
						}
					}
				}
				openCircleSprite.color = Color.Lerp(GetColor(PhenoGenoEnum.Phenotype), Color.red, Mathf.Min(1f, predatorCount));
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.energy) {
				filledCircleSprite.color = ColorScheme.instance.cellGradientEnergy.Evaluate(energyFullness);
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.flux) {
				float intensity = 0.2f;
				float red = Mathf.Min(GetEffectDown(false, false, true, true) * intensity, 1f);
				float green = Mathf.Min(GetEffectUp(false, true, true) * intensity, 1f);
				filledCircleSprite.color = new Color(red, green, 0f);
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.effect) {
				float effectValue = 0f;

				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal) {
					effectValue = 0.5f + GetEffect(true, true, true, true) * 0.1f;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction) {
					effectValue = 0.5f + GetEffect(true, false, false, false) * 0.1f;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux) {
					effectValue = 0.5f + GetEffect(false, false, true, true) * 0.1f;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					effectValue = 0.5f + creature.phenotype.GetEffectPerCell(true, true, true) * 0.1f;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					effectValue = 0.5f + creature.phenotype.GetEffectPerCell(true, false, false) * 0.1f;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					effectValue = 0.5f + creature.phenotype.GetEffectPerCell(false, false, true) * 0.1f;
				}
				filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.leafExposure) {
				Color color = Color.black;
				if (GetCellType() == CellTypeEnum.Leaf) {
					color = ColorScheme.instance.cellGradientLeafExposure.Evaluate((this as LeafCell).lowPassExposure);
				}
				filledCircleSprite.color = color;
				openCircleSprite.color = color;
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.childCountCreature) {
				float value = 0.05f + creature.ChildrenCountDeadOrAlive() * 0.1f;
				filledCircleSprite.color = ColorScheme.instance.cellCreatureChildCount.Evaluate(value);
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.predatorPray) {
				filledCircleSprite.color = Color.blue;
				if (GetCellType() == CellTypeEnum.Jaw) {
					if ((this as JawCell).prayCount > 0) {
						filledCircleSprite.color = Color.green;
					}
				}
				if (predatorCount > 0) {
					filledCircleSprite.color = Color.red;
				}
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.typeAndPredatorPray) {
				float effectValue = 0.5f + effectProductionPredPray * 0.02f;
				if (effectProductionPredPray == 0f) {
					filledCircleSprite.color = ColorScheme.instance.ToColor(GetCellType());
				} else if (GetCellType() == CellTypeEnum.Jaw && effectProductionPredPray < 0f) {
					filledCircleSprite.color = Color.white;
				} else {
					filledCircleSprite.color = ColorScheme.instance.cellGradientEffect.Evaluate(effectValue);
				}
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.update) {
				filledCircleSprite.color = didUpdateFunctionThisFrame > 0 ? ColorScheme.instance.ToColor(GetCellType()) : Color.blue;
				//openCircleSprite.color = didUpdateEnergyThisFrame > 0 ? ColorScheme.instance.ToColor(GetCellType()) : Color.blue;
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.creation) {
				if (creature.creation == CreatureCreationEnum.Born) {
					filledCircleSprite.color = Color.magenta;
				} else if (creature.creation == CreatureCreationEnum.Cloned) {
					filledCircleSprite.color = Color.yellow;
				} else {
					//forged
					filledCircleSprite.color = Color.cyan;
				}
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.individual) {
				filledCircleSprite.color = creature.phenotype.individualColor;
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.pulse) {
				//filledCircleSprite.color = isOrigin && originPulseTick == 0 ? ColorScheme.instance.ToColor(GetCellType()) : Color.blue;
				if (isAxonEnabled) {
					float red = 0f;
					float green = 0f;
					float blue = 0f;
					green = blue = 0.5f + GetAxonPulseValue(0) * 0.5f;
					filledCircleSprite.color = new Color(red, green, blue);
				} else if (GetCellType() == CellTypeEnum.Muscle) {
					if (((MuscleCell)this).masterAxonGridPosition == null) {
						filledCircleSprite.color = Color.black; // has no masterAxonGridPosition, should have
					} else {
						Cell masterAxon = creature.phenotype.cellMap.GetCell(((MuscleCell)this).masterAxonGridPosition);
						if (masterAxon == null) {
							filledCircleSprite.color = Color.gray; // has a masterAxonGridPosition, but there is no cell there (could be killed or unborn)
						} else {
							if (((MuscleCell)this).masterAxoneDistance != null) {
								float red = 0f;
								float green = 0f;
								float blue = 0f;
								red = green = 0.5f + masterAxon.GetAxonPulseValue((int)((MuscleCell)this).masterAxoneDistance) * 0.5f;
								filledCircleSprite.color = new Color(red, green, blue);
							}
						}
					}
				} else {
					filledCircleSprite.color = Color.blue;
				}
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.age) {
				filledCircleSprite.color = ColorScheme.instance.creatureAgeGradient.Evaluate(creature.GetAgeNormalized(World.instance.worldTicks));
			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.shell) {
				if (GetCellType() == CellTypeEnum.Shell) {
					filledCircleSprite.color = (this as ShellCell).GetStrongerColor();
					openCircleSprite.color = (this as ShellCell).GetStrongerColor();
				} else {
					filledCircleSprite.color = Color.black;
					openCircleSprite.color = Color.black;
				}

			} else if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.buildPriority) {
				if (CreatureSelectionPanel.instance.IsSelected(creature)) {
					SetLabelEnabled(true);
					if (isOrigin) {
						SetLabelText("-");
					} else {
						SetLabelText(buildPriority.ToString());
					}

					if (gene.buildPriorityBias < 0) {
						SetLabelColor(Color.green);
					} else if (gene.buildPriorityBias > 0) {
						SetLabelColor(Color.red);
					} else {
						SetLabelColor(Color.gray);
					}

					filledCircleSprite.color = Color.black;
					SetCorrectLabelOrientation();
				} else {
					filledCircleSprite.color = GetColor();
					SetLabelEnabled(false);
				}
			}
		} else { // Genotype...
			if (GenotypeGraphicsPanel.instance.graphicsGeneCell == GenotypeGraphicsPanel.CellGraphicsEnum.type) {
				if (CreatureSelectionPanel.instance.IsSelected(creature)) {
					SetLabelEnabled(true);
					SetLabelText(gene.index.ToString());
					SetLabelColor(new Color(1f - GetColor().r, 1f - GetColor().g, 1f - GetColor().b, 1f));
					filledCircleSprite.color = GetColor();
					SetCorrectLabelOrientation();
				} else {
					filledCircleSprite.color = GetColor();
					SetLabelEnabled(false);
				}
			} else if (GenotypeGraphicsPanel.instance.graphicsGeneCell == GenotypeGraphicsPanel.CellGraphicsEnum.buildIndex) {
				//TODO: show geneCells blocked by twin, show horizon of creature, show blocked by lower build order
				if (CreatureSelectionPanel.instance.IsSelected(creature)) {
					SetLabelEnabled(true);
					SetLabelText(buildIndex.ToString());
					SetLabelColor(Color.gray);
					filledCircleSprite.color = Color.black;
					SetCorrectLabelOrientation();
				} else {
					filledCircleSprite.color = GetColor();
					SetLabelEnabled(false);
				}
			} else if (GenotypeGraphicsPanel.instance.graphicsGeneCell == GenotypeGraphicsPanel.CellGraphicsEnum.buildPriority) {
				if (CreatureSelectionPanel.instance.IsSelected(creature)) {
					SetLabelEnabled(true);
					if (isOrigin) {
						SetLabelText("-");
					} else {
						SetLabelText(buildPriority.ToString());
					}

					if (gene.buildPriorityBias < 0) {
						SetLabelColor(Color.green);
					} else if (gene.buildPriorityBias > 0) {
						SetLabelColor(Color.red);
					} else {
						SetLabelColor(Color.gray);
					}

					filledCircleSprite.color = Color.black;
					SetCorrectLabelOrientation();
				} else {
					filledCircleSprite.color = GetColor();
					SetLabelEnabled(false);
				}
			}
		}
	}

	public void UpdateTwistAndTurn() {
		//Optimize further
		transform.rotation = Quaternion.identity; // Cell should never be rotated. Rotate rotated node in cell instead! We need this one so we reset the cell rotation after being rotated via rotate creature

		UpdateNeighbourVectors(); //costy, update only if cell has direction and is in frustum
		if (groups > 1) {
			TurnHingeNeighboursInPlace(); //optimize further
		}
		UpdateHeading(); //costy, update only if cell has direction and is in frustum
		UpdateFlipSide();

		didUpdateFunctionThisFrame--; //  Just for visuals
		didUpdateEnergyThisFrame--; //  Just for visuals
	}
	// ^ Update ^

	public void MakeAllNeighbourAnglesDirty() {
		for (int index = 0; index < 6; index++) {
			if (HasNeighbourCell(index)) {
				cellNeighbourDictionary[index].isAngleDirty = true;
			}
		}
	}

	virtual public bool IsHibernating() {
		return false;
	}

	//Phenotype only
	virtual public void OnRecycleCell() {
		theRigidBody.simulated = true; //physics seem to have problem when borrowing cells and then enabling RB, it should be ok to enable it here since cell is disabled anyway

		foreach (JawCell predator in predators) {
			predator.RemovePray(this); // make jaw forget about me as a pray of his
		}
		predators.Clear();

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
		placentaSprings = new SpringJoint2D[0];

		ShowOnTop(false);

		gene = null;
		id = "trash";
		predators.Clear();
		isPlacenta = false;
		groups = 0;

		lastTime = 0;
		timeOffset = 0;
		buildIndex = 0;

		radius = 0.5f;
		transform.localScale = new Vector3(1f, 1f, 1f);

		originPulseTick = 0;

		if (northSpring != null) {
			northSpring.distance = 1f;
		}
		if (southWestSpring != null) {
			southWestSpring.distance = 1f;
		}
		if (southEastSpring != null) {
			southEastSpring.distance = 1f;
		}

		effectFluxFromMotherAttached = 0f;
		effectFluxToChildrenAttached = 0f;
		effectFluxToSelf = 0f;
		effectFluxFromSelf = 0f;
	}

	virtual public void OnBorrowToWorld() { }

	// Save
	private CellData cellData = new CellData();
	public CellData UpdateData() {
		cellData.position = transform.position;
		cellData.heading = heading;
		cellData.bindCardinalIndex = bindCardinalIndex;
		cellData.geneIndex = gene.index;
		cellData.mapPosition = mapPosition;
		cellData.buildIndex = buildIndex;
		cellData.flipSide = flipSide;
		cellData.timeOffset = timeOffset;
		cellData.lastTime = lastTime;
		cellData.radius = radius;
		cellData.velocity = theRigidBody.velocity;
		cellData.energy = energy;

		//Egg
		cellData.eggCellDetatchMode = eggCellDetatchMode;
		cellData.eggCellDetatchSizeThreshold = eggCellDetatchSizeThreshold;
		cellData.eggCellDetatchEnergyThreshold = eggCellDetatchEnergyThreshold;

		// Leaf ... might be time to move this save / load stuff to respective cell
		if (GetCellType() == CellTypeEnum.Leaf) {
			cellData.leafCellLowPassExposure = (this as LeafCell).lowPassExposure;
		}
		// Leaf ^

		// Origin
		cellData.originDetatchMode = originDetatchMode;
		cellData.originDetatchSizeThreshold = originDetatchSizeThreshold;
		cellData.originDetatchEnergyThreshold = originDetatchEnergyThreshold;
		cellData.originPulseTick = originPulseTick;

		return cellData;
	}

	// Load
	public void ApplyData(CellData cellData, Creature creature) {
		transform.position = cellData.position;
		heading = cellData.heading;
		bindCardinalIndex = cellData.bindCardinalIndex;
		gene = creature.genotype.genome[cellData.geneIndex];
		mapPosition = cellData.mapPosition;
		buildIndex = cellData.buildIndex;
		flipSide = cellData.flipSide;
		timeOffset = cellData.timeOffset;
		lastTime = cellData.lastTime;
		radius = cellData.radius;
		theRigidBody.velocity = cellData.velocity;
		energy = Mathf.Min(cellData.energy, GlobalSettings.instance.phenotype.cellMaxEnergy);

		// Egg
		// detatch mode
		eggCellDetatchMode = cellData.eggCellDetatchMode;

		// detatch size
		if (cellData.eggCellDetatchSizeThreshold > GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax) {
			eggCellDetatchSizeThreshold = cellData.eggCellDetatchSizeThreshold / 30f;
		} else {
			eggCellDetatchSizeThreshold = cellData.eggCellDetatchSizeThreshold;
		}

		// detatch energy
		if (cellData.eggCellDetatchEnergyThreshold > GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax) { // if more than 100% must be old (where we measured cell energy)
			eggCellDetatchEnergyThreshold = cellData.eggCellDetatchEnergyThreshold / 100f;
		} else {
			eggCellDetatchEnergyThreshold = cellData.eggCellDetatchEnergyThreshold;
		}

		// egg ^

		// Leaf
		if (GetCellType() == CellTypeEnum.Leaf) {
			(this as LeafCell).lowPassExposure = cellData.leafCellLowPassExposure;
		}
		// Leaf ^

		// Origin
		originDetatchMode = cellData.originDetatchMode;

		// detatch size
		if (cellData.originDetatchSizeThreshold > GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax) {
			originDetatchSizeThreshold = cellData.originDetatchSizeThreshold / 30f;
		} else {
			originDetatchSizeThreshold = cellData.originDetatchSizeThreshold;
		}

		// detatch energy
		if (cellData.originDetatchEnergyThreshold > GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax) { // if more than 100% must be old (where we measured cell energy)
			originDetatchEnergyThreshold = cellData.originDetatchEnergyThreshold / 100f;
		} else {
			originDetatchEnergyThreshold = cellData.originDetatchEnergyThreshold;
		}

		originPulseTick = cellData.originPulseTick;

		this.creature = creature;
	}

	//----------Signal--------------------------------


}