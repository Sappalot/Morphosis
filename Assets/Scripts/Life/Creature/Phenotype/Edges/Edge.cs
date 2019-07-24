using UnityEngine;
using System;

public class Edge : MonoBehaviour {
	public GameObject mainArrow;
	public GameObject normalArrow;
	public GameObject velocityArrow;
	public GameObject forceArrow;

	public float mEnergyTransfer = 0.5f; //directional ?

	private bool mIsWing = false;
	private EdgeAttachment attachmentParent;
	private EdgeAttachment attachmentChild;

	private float[] forceMagnitudeArray = new float[10];
	private int forceMagnitudeArrayPointer = 0;

	private Cell frontCell; //used with wings
	private Cell backCell; //used with wings
	private Rigidbody2D frontCellRB; //used with wings
	private Rigidbody2D backCellRB; //used with wings

	private Vector3 normal; //used with wings
	private Vector3 velocity; //used with wings
	private Vector3 force; //used with wings

	private float strength;

	public Cell parentCell {
		get {
			return attachmentParent.cell;
		}
	}

	public Cell childCell {
		get {
			return attachmentChild.cell;
		}

	}

	public float length {
		get {
			return Vector2.Distance(parentCell.position, childCell.position);
		}
	}

	private void Start() {
		mainArrow.SetActive(false);
		normalArrow.SetActive(false);
		velocityArrow.SetActive(false);
		forceArrow.SetActive(false);
	}

	public void Setup(Cell parentCell, Cell childCell, int directionChildToParentCell) {
		attachmentParent = new EdgeAttachment(parentCell, (directionChildToParentCell + 3 ) % 6);
		attachmentChild = new EdgeAttachment(childCell, directionChildToParentCell);
	}

	public bool isFin { 
		get { return mIsWing; }
		private set { }
	}

	public Cell GetOtherCell(Cell cell) {
		if (cell == attachmentParent.cell) {
			return attachmentChild.cell;
		}
		else if (cell == attachmentChild.cell) {
			return attachmentParent.cell;
		}
		return null;
	}

	public void MakeWing(Cell frontCell) {
		if (frontCell == attachmentParent.cell) {
			this.frontCell = frontCell;
			this.backCell = attachmentChild.cell;
		}
		else if (frontCell == attachmentChild.cell) {
			this.frontCell = frontCell;
			this.backCell = attachmentParent.cell;
		}
		else {
			throw new Exception("Trying to make a wing, with frontCell wich is not present in edge");
		}
		mIsWing = (frontCell.GetCellType() == CellTypeEnum.Muscle || backCell.GetCellType() == CellTypeEnum.Muscle);

		frontCellRB = this.frontCell.theRigidBody;
		backCellRB =  this.backCell.theRigidBody;
	}

	public void UpdateGraphics() {
		//TODO: If draw wings && inside frustum
		if (GlobalPanel.instance.graphicsMuscleForcesToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			if (frontCell != null && backCell != null) {

				float forceMagnitude = force.magnitude;
				forceMagnitudeArray[forceMagnitudeArrayPointer] = forceMagnitude;
				forceMagnitudeArrayPointer++;
				if (forceMagnitudeArrayPointer >= forceMagnitudeArray.Length) {
					forceMagnitudeArrayPointer = 0;
				}

				float sum = 0f;
				for (int pos = 0; pos < forceMagnitudeArray.Length; pos++) {
					sum += forceMagnitudeArray[pos];
				}
				float forceAverage = sum / forceMagnitudeArray.Length;


				//mainArrow.SetActive(true);
				//normalArrow.SetActive(true);
				//velocityArrow.SetActive(true);
				forceArrow.SetActive(true);

				////draw main
				bool sleeping = frontCell.theRigidBody.IsSleeping();
				mainArrow.GetComponent<LineRenderer>().startColor = sleeping ? Color.black : Color.white;
				mainArrow.GetComponent<LineRenderer>().endColor = sleeping ? Color.black : Color.white;

				mainArrow.GetComponent<LineRenderer>().SetPosition(1, frontCell.transform.position);
				mainArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position);

				////draw normal
				Vector3 wingSegmentHalf = (frontCell.transform.position - backCell.transform.position) * 0.5f;
				Vector3 midSegment = backCell.transform.position + wingSegmentHalf;
				//Vector3 normalPoint = midSegment + normal;
				//normalArrow.GetComponent<LineRenderer>().SetPosition(1, normalPoint);
				//normalArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);

				////draw velocity
				//velocityArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + velocity);
				//velocityArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);

				//draw force
				forceArrow.GetComponent<LineRenderer>().startColor = forceArrow.GetComponent<LineRenderer>().endColor = ColorScheme.instance.creatureFinWake.Evaluate(forceAverage * 2f);
				forceArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + normal * (0.6f + Mathf.Min(0.1f, forceAverage)));
				forceArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment + normal * 0.6f);
			}
		} else {
			//mainArrow.SetActive(false);
			//normalArrow.SetActive(false);
			//velocityArrow.SetActive(false);
			forceArrow.SetActive(false);
		}
	}

	public static float maxForce = 0f;

	//Use 2 cells to find normal, allways on wings RIGHT hand
	public void UpdateNormal() {
		//TODO Optimize in 2D, try to get rid of normalization
		Vector3 wingSegment = frontCell.transform.position - backCell.transform.position;
		normal = Vector3.Cross(wingSegment.normalized, new Vector3(0f, 0f, 1f));
	}

	//use 2 cells to find center velocity
	public void UpdateVelocity() {
		//TODO Get references once
		velocity = (frontCellRB.velocity + backCellRB.velocity) * 0.5f;
	}

	// use normal and velocity to calculate force
	public void UpdateForce(Creature creature, bool isFin, ulong worldTick) {
		if (!creature.phenotype.IsSliding(worldTick)) {
			float speedInNormalDirection = Math.Max(0f, Vector3.Dot(normal, velocity));
			force = (isFin ? 1f : GlobalSettings.instance.phenotype.nonFinForceFactor) * -normal * Math.Min(GlobalSettings.instance.phenotype.finForceMax, (GlobalSettings.instance.phenotype.finForceLinearFactor * speedInNormalDirection + GlobalSettings.instance.phenotype.finForceSquareFactor * Mathf.Pow(speedInNormalDirection, 2f)));
		} else {
			force = Vector3.zero;
		}
	}

	//Apply current force as an impulse on cells
	public void ApplyForce() {
		if (force.sqrMagnitude < 0.000001f) {
			return; // Sssssh, don't desturb bigid body sleep!!
		}
		// There is still a possibility ForceMode2D.Force will work better
		frontCellRB.AddForce(force * 0.5f, ForceMode2D.Impulse);
		backCellRB.AddForce(force * 0.5f, ForceMode2D.Impulse);
	}

	public void OnRecycle() {
		frontCell = null;
		backCell =  null;

		frontCellRB = null;
		backCellRB = null;

		attachmentChild =  null;
		attachmentParent = null;

		force =    Vector3.zero;
		velocity = Vector3.zero;

		forceArrow.SetActive(false);
		mainArrow.SetActive(false);
		normalArrow.SetActive(false);
		velocityArrow.SetActive(false);
	}
}