using System.Collections.Generic;
using UnityEngine;
public class JawCell : Cell {

	public JawCellMouth mouth;

	[HideInInspector]
	public Dictionary<Cell, Pray> prays = new Dictionary<Cell, Pray>(); //Who am i eating on? Me gain? other lose?
	private bool deleteFlagged;

	public new void Awake() {
		base.Init();
	}

	public override void Setup(PhenoGenoEnum phenoGeno) {
		if (phenoGeno == PhenoGenoEnum.Genotype) {
			mouth.gameObject.SetActive(false);
		}
		base.Setup(phenoGeno);
	}

	public override void UpdateCellWork(int deltaTicks, ulong worldTicks) {
		if (deleteFlagged) {
			return;
		}
		if (PhenotypePhysicsPanel.instance.functionJaw.isOn) {
			if (IsHibernating()) {
				mouth.gameObject.SetActive(false);
				effectProductionPredPrayUp = 0f;
				effectProductionInternalDown = GlobalSettings.instance.phenotype.cellHibernateEffectCost;
			} else {
				mouth.gameObject.SetActive(true);
				effectProductionInternalDown = GlobalSettings.instance.phenotype.jawCellEffectCost;

				//Hack release pray
				RemoveNullPrays(); //We need this one not to run into null refs once in a blue moon
				CellPanel.instance.MakeDirty();

				effectProductionPredPrayUp = eatEffect;

				// We need to update per frame, not just when we enter trigger, as ram speed might change during stay in the "trigger zone"
				foreach (Pray pray in prays.Values) {
					pray.UpdateMetabolism(this);
				}
			}

			base.UpdateCellWork(deltaTicks, worldTicks);
		} else {
			mouth.gameObject.SetActive(false);
			effectProductionPredPrayUp = 0f;
			effectProductionInternalDown = 0f;
		}
	}

	override public bool IsHibernating() {
		return false;
		//return (gene.jawCellHibernateWhenAttachedToMother && creature.IsAttachedToMotherAlive()) || (gene.jawCellHibernateWhenAttachedToChild && creature.IsAttachedToChildAlive());
	}

	private float eatEffect {
		get {
			float effect = 0f;
			foreach (Pray pray in prays.Values) {
				effect += pray.predatorEatEffect;
			}
			return effect;
		}
	}

	public float GetPrayEatenEffect(Cell prayCell) {
		return prays[prayCell].prayEatenEffect;
	}

	public int prayCount {
		get {
			return prays.Count;
		}
	}

	//Called from cell mouth
	public void TriggerEnter(Collider2D other) {
		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
			return;
		}
		if (deleteFlagged) {
			return;
		}
		if (creature.phenotype.isGrabbed) { //dont eat others if i'm being dragged around of user (copy / move)
			return;
		}
		if (IsHibernating()) {
			return;
		}
		//if (phenoGeno == PhenoGenoEnum.Genotype) {
		//	return;
		//}

		Cell prayCell = other.GetComponent<Cell>();

		if (prayCell != null && prayCell.creature != creature) {

			Creature pray = prayCell.creature;
			// spare mother
			if (!gene.jawCellCannibalizeMother && creature.HasMotherAlive() && creature.GetMotherAlive().id == pray.id) {
				return;
			}

			// spare siblings
			if (!gene.jawCellCannibalizeSiblings && creature.HasMotherDeadOrAlive() && pray.HasMotherDeadOrAlive() && creature.GetMotherIdDeadOrAlive() == pray.GetMotherIdDeadOrAlive()) {
				return;
			}

			// spare children
			if (!gene.jawCellCannibalizeChildren) {
				foreach (Creature child in creature.GetChildrenAlive()) { //Note: all references in children are not updated at this point
					if (pray == child) {
						return;
					}
				}
			}

			//TODO: spare Kin and fathers well

			//Add this one to list of prays to add at update
			PairPredatorPray(this, prayCell);
		}
	}

	//Called from cell mouth
	public void TriggerExit(Collider2D other) {
		if (other.gameObject.layer == 2) { //don't trigger other's mouth colliders, only on cells
			return;
		}

		Cell prayCell = other.GetComponent<Cell>();

		if (prayCell != null) {
			UnpairPredatorPray(this, prayCell);
		} else {
			RemoveNullPrays();
		}
	}

	public void RemoveNullPrays() {
		List<Pray> remove = new List<Pray>();
		foreach (Pray pray in prays.Values) {
			if (pray.cell == null) {
				remove.Add(pray);
			}
		}
		foreach (Pray pray in remove) {
			prays.Remove(pray.cell);
		}
	}

	private void AddPray(Pray pray) {
		if (!prays.ContainsKey(pray.cell)) {
			prays.Add(pray.cell, pray);
		}
	}

	private void PairPredatorPray(JawCell predatorCell, Cell prayCell) {
		Pray newPray = new Pray(prayCell);
		AddPray(newPray); //TODO update effect
		prayCell.AddPredator(predatorCell);
		newPray.UpdateMetabolism(this);
	}

	private void UnpairPredatorPray(JawCell predatorCell, Cell prayCell) {
		RemovePray(prayCell);
		prayCell.RemovePredator(predatorCell);
	}

	public void RemovePray(Cell prayCell) {
		if (prays.ContainsKey(prayCell)) {
			prays.Remove(prayCell);
		}
	}

	public override void OnRecycleCell() {
		base.OnRecycleCell();
		deleteFlagged = true;

		//Free all prays from me since i excist no more
		foreach (Cell prayCell in prays.Keys) {
			prayCell.RemovePredator(this);
		}
		prays.Clear();
	}

	public override void OnBorrowToWorld() {
		base.OnBorrowToWorld();
		deleteFlagged = false;
		//base.OnRecycleCell(); //is this one really needed? should have been done when recycling allready
	}

	//--------

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
	}

	override public Color GetColor(PhenoGenoEnum phenoGeno) {
		if (phenoGeno == PhenoGenoEnum.Genotype) {
			return ColorScheme.instance.ToColor(GetCellType());
		} else {
			return Color.Lerp(ColorScheme.instance.jaw, Color.white, Mathf.Min(0.5f, effectProductionPredPrayUp));
;		}
	}
}