using System.Collections.Generic;
using UnityEngine;
public class JawCell : Cell {

	public JawCellMouth mouth;

	[HideInInspector]
	public Dictionary<Cell, Pray> prays = new Dictionary<Cell, Pray>(); //Who am i eating on? Me gain? other lose?
	private bool deleteFlagged;

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		if (deleteFlagged) {
			return;
		}
		if (GlobalPanel.instance.physicsJaw.isOn) {
			mouth.gameObject.SetActive(true);
			effectConsumptionInternal = GlobalSettings.instance.phenotype.jawCellEffectCost;

			//Hack release pray
			RemoveNullPrays(); //We need this one not to run into null refs once in a blue moon
			JawCellPanel.instance.MakeDirty();

			effectProductionExternal = eatEffect;

			base.UpdateCellFunction(deltaTicks, worldTicks);
		} else {
			mouth.gameObject.SetActive(false);
			effectConsumptionInternal = 0f;
			effectProductionExternal = 0f;
		}
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

		Cell prayCell = other.GetComponent<Cell>();

		if (prayCell != null && prayCell.creature != creature) {
			// dont eat mother, grandma is OK (what about other siblings and cousins in the same cluster?)
			if (creature.HasMother() && creature.GetMother().id == prayCell.creature.id) {
				return;
			}
			// don't eat children, grandchildren is OK (what about other siblings and cousins in the same cluster?)
			foreach (Creature child in creature.GetChildren()) { //Note: all references in children are not updated at this point
				if (prayCell.creature == child) {
					return;
				}
			}

			//Add this one to list of prays to add at update
			PairPredatorPray(this, prayCell);
		}

		//PhenotypePanel.instance.MakeDirty();
		//CellPanel.instance.MakeDirty();
		//JawCellPanel.instance.MakeDirty();
	}

	//Called from cell mouth
	public void TriggerExit(Collider2D other) {
		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
			return;
		}

		Cell prayCell = other.GetComponent<Cell>();

		if (prayCell != null) {
			UnpairPredatorPray(this, prayCell);
		} else {
			RemoveNullPrays();
		}

		//PhenotypePanel.instance.MakeDirty();
		//CellPanel.instance.MakeDirty();
		//JawCellPanel.instance.MakeDirty();
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
		Pray p = new Pray(prayCell);
		AddPray(p); //TODO update effect
		prayCell.AddPredator(predatorCell);
		p.UpdateMetabolism(this);
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

		//Free all prays from me since i excint no more
		foreach (Cell prayCell in prays.Keys) {
			prayCell.RemovePredator(this);
		}
		prays.Clear();
	}

	public override void OnBorrowToWorld() {
		deleteFlagged = false;
		//base.OnRecycleCell(); //is this one really needed? should have been done when recycling allready
	}

	//--------

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
	}
}