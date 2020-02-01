using UnityEngine;
using System;

public class Edge : MonoBehaviour {
	public GameObject mainArrow;
	public GameObject normalArrow;
	public GameObject velocityArrow;
	public GameObject forceArrow;

	public float mEnergyTransfer = 0.5f; //directional ?

	private bool mIsFin = false;
	private EdgeAttachment attachmentParent; // current
	private EdgeAttachment attachmentChild; // next

	//private float[] forceMagnitudeArray = new float[10];
	//private int forceMagnitudeArrayPointer = 0;

	//private Vector3[] forceArray = new Vector3[1];
	//private int forceArrayPointer = 0;

	public bool wasDoupletChecked;

	private Cell frontCell; //used with wings
	private Cell backCell; //used with wings
	private Cell previousCell; //used woth wings apexEdge 60 & 120 
	private Rigidbody frontCellRB; //used with wings
	private Rigidbody backCellRB; //used with wings


	private Vector3 normal; //used with wings
	private Vector3 velocity; //used with wings
	private Vector3 force; //used with wings

	private Vector2 forceAverage;

	private float strength;

	//------
	public Cell.ApexAngle apexAngle;
	public bool isApex {
		get {
			return apexAngle == Cell.ApexAngle.apex0 || apexAngle == Cell.ApexAngle.apex60 || apexAngle == Cell.ApexAngle.apex120;
		}
	}
	public bool isDoubleSided;

	public float width {
		get {
			if (apexAngle == Cell.ApexAngle.apex0) {
				return 0.9f;
			} else if(apexAngle == Cell.ApexAngle.apex60) {
				return 0.5f;
			} else if (apexAngle == Cell.ApexAngle.apex120) {
				return 0.3f;
			} else {
				return 1f;
			}
		}
	}
	//------
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

		//for (int i = 0; i < forceArray.Length; i++) {
		//	forceArray[i] = Vector3.zero;
		//}
	}

	// parent cell = current
	// child cell = next
	public void Setup(Cell parentCell, Cell childCell, int directionChildToParentCell, Cell.ApexAngle apexAngle) {
		attachmentParent = new EdgeAttachment(parentCell, (directionChildToParentCell + 3 ) % 6);
		attachmentChild = new EdgeAttachment(childCell, directionChildToParentCell);
		this.apexAngle = apexAngle;

		isDoubleSided = false;
		wasDoupletChecked = false;
	}

	public bool isFin { 
		get {
			if (apexAngle == Cell.ApexAngle.apex0) {
				return backCell.GetCellType() == CellTypeEnum.Muscle;
			} else {
				return mIsFin;
			}
		 } // mIsWing is true if either end is muscle
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

	public void MakeWing(Cell frontCell, Cell previousCell) {
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
		mIsFin = (frontCell.GetCellType() == CellTypeEnum.Muscle || backCell.GetCellType() == CellTypeEnum.Muscle);

		this.previousCell = previousCell;

		frontCellRB = this.frontCell.theRigidBody;
		backCellRB =  this.backCell.theRigidBody;

	}

	public void UpdateGraphics() {
		//TODO: If draw wings && inside frustum
		if (GlobalPanel.instance.graphicsMuscleForcesToggle.isOn && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			if (frontCell != null && backCell != null) {

				//float forceMagnitude = force.magnitude;
				//forceMagnitudeArray[forceMagnitudeArrayPointer] = forceMagnitude;
				//forceMagnitudeArrayPointer++;
				//if (forceMagnitudeArrayPointer >= forceMagnitudeArray.Length) {
				//	forceMagnitudeArrayPointer = 0;
				//}

				//float sum = 0f;
				//for (int pos = 0; pos < forceMagnitudeArray.Length; pos++) {
				//	sum += forceMagnitudeArray[pos];
				//}
				//float forceAverage = sum / forceMagnitudeArray.Length;

				mainArrow.SetActive(true);
				normalArrow.SetActive(true);
				//velocityArrow.SetActive(true);
				forceArrow.SetActive(true);

				//draw main fin surface
				if (apexAngle == Cell.ApexAngle.blunt) {
					// normal edge
					Color color = isDoubleSided ? Color.white : Color.gray;
					mainArrow.GetComponent<LineRenderer>().startColor = color;
					mainArrow.GetComponent<LineRenderer>().endColor = color;
					mainArrow.GetComponent<LineRenderer>().SetPosition(1, frontCell.transform.position);
					mainArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position);
				} else {
					mainArrow.GetComponent<LineRenderer>().startColor = Color.red;
					mainArrow.GetComponent<LineRenderer>().endColor = Color.red;
					Vector3 right = Vector3.Cross(normal, new Vector3(0f, 0f, 1f));
					Vector3 rightSide = backCell.transform.position + right * 0.5f * width;
					Vector3 leftSide = backCell.transform.position - right * 0.5f * width;
					mainArrow.GetComponent<LineRenderer>().SetPosition(1, rightSide);
					mainArrow.GetComponent<LineRenderer>().SetPosition(0, leftSide);
				}

				//draw normal
				Vector3 wingSegmentHalf;
				Vector3 midSegment = Vector3.zero;
				if (apexAngle == Cell.ApexAngle.blunt) {
					wingSegmentHalf = (frontCell.transform.position - backCell.transform.position) * 0.5f;
					midSegment = backCell.transform.position + wingSegmentHalf;

					Vector3 normalPoint = midSegment + normal;
					normalArrow.GetComponent<LineRenderer>().SetPosition(1, normalPoint);
					normalArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);
				} else {
					midSegment = backCell.transform.position;
					normalArrow.GetComponent<LineRenderer>().SetPosition(1, backCell.transform.position + normal);
					normalArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position);
				}

				////draw velocity
				//velocityArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + velocity);
				//velocityArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);

				//draw force ... Wake
				if (isApex) {
					forceArrow.GetComponent<LineRenderer>().startColor = forceArrow.GetComponent<LineRenderer>().endColor = ColorScheme.instance.creatureFinWake.Evaluate(force.magnitude * 2f);
					forceArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + normal * (0.6f + Mathf.Min(0.5f, force.magnitude)));
					forceArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment + normal * 0.6f);
				} else {
					float sign = Mathf.Sign(speedInNormalDirection);
					forceArrow.GetComponent<LineRenderer>().startColor = forceArrow.GetComponent<LineRenderer>().endColor = ColorScheme.instance.creatureFinWake.Evaluate(force.magnitude * 2f);
					forceArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + normal * sign * (0.6f + Mathf.Min(0.5f, force.magnitude)));
					forceArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment + normal * sign * 0.6f);
				}

			}
		} else {
			mainArrow.SetActive(false);
			normalArrow.SetActive(false);
			//velocityArrow.SetActive(false);
			forceArrow.SetActive(false);
		}
	}

	public static float maxForce = 0f;

	//Use 2 cells to find normal, allways on wings RIGHT hand
	public void UpdateNormal() {
		//TODO Optimize in 2D, try to get rid of normalization
		if (apexAngle == Cell.ApexAngle.blunt) {
			Vector3 wingSegment = frontCell.transform.position - backCell.transform.position;
			normal = Vector3.Cross(wingSegment.normalized, new Vector3(0f, 0f, 1f));
		} else if (apexAngle == Cell.ApexAngle.apex0) {
			normal = Vector3.Normalize(backCell.transform.position - frontCell.transform.position);
		} else if (apexAngle == Cell.ApexAngle.apex60 || apexAngle == Cell.ApexAngle.apex120) {
			if (previousCell != null) {
				normal = Vector3.Normalize(Vector3.Cross(frontCell.transform.position - (previousCell.transform.position), new Vector3(0f, 0f, 1f)));
			}
		}
	}

	//use 2 cells to find center velocity
	public void UpdateVelocity() {
		if (apexAngle == Cell.ApexAngle.blunt) {
			velocity = (frontCellRB.velocity + backCellRB.velocity) * 0.5f;
		} else {
			velocity = backCellRB.velocity;
		}
	}

	// use normal and velocity to calculate force
	private float speedInNormalDirection;
	public void UpdateForce(Creature creature, bool isFin, ulong worldTick) {
		if (!creature.phenotype.IsSliding(worldTick)) {
			speedInNormalDirection = isDoubleSided ? Vector3.Dot(normal, velocity) : Math.Max(0f, Vector3.Dot(normal, velocity));
			force = width * Mathf.Sign(speedInNormalDirection) * (isFin ? 1f : GlobalSettings.instance.phenotype.nonFinForceFactor) * -normal * Mathf.Clamp((GlobalSettings.instance.phenotype.finForceLinearFactor * Math.Abs(speedInNormalDirection) + GlobalSettings.instance.phenotype.finForceSquareFactor * Mathf.Pow(Math.Abs(speedInNormalDirection), 2f)), -GlobalSettings.instance.phenotype.finForceMax, GlobalSettings.instance.phenotype.finForceMax);
		} else {
			force = Vector3.zero;
		}

		////average
		//forceArray[forceArrayPointer] = force;
		//forceArrayPointer++;
		//if (forceArrayPointer >= forceArray.Length) {
		//	forceArrayPointer = 0;
		//}

		//Vector3 sum = Vector3.zero;
		//for (int pos = 0; pos < forceArray.Length; pos++) {
		//	sum += forceArray[pos];
		//}
		//forceAverage = sum / forceArray.Length;

	}

	//Apply current force as an impulse on cells
	public void ApplyForce() {
		//if (force.sqrMagnitude < 0.000001f) {
		//	return; // Sssssh, don't desturb bigid body sleep!!
		//}
		// There is still a possibility that ForceMode2D.Force will work better
		if (isApex) {
			backCellRB.AddForce(force, ForceMode.Impulse);
		} else {
			frontCellRB.AddForce(force * 0.5f, ForceMode.Impulse);
			backCellRB.AddForce(force * 0.5f, ForceMode.Impulse);
		}
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