using System.Collections.Generic;
using UnityEngine;
public class JawCell : Cell {

	public JawCellMouth mouth;

	[HideInInspector]
	public Dictionary<Cell, Pray> prays = new Dictionary<Cell, Pray>(); //Who am i eating on? Me gain? other lose?

	//private List<Cell> praysToAdd = new List<Cell>();
	//private Dictionary<Cell, PredatorPrayPair> praysToRemove = new Dictionary<Cell, PredatorPrayPair>();

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

			////Add prays
			//foreach (Cell addMe in praysToAdd) {
			//	if (addMe != null) {
			//		PairPredatorPray(this, addMe);
			//	}
			//}
			//praysToAdd.Clear();

			////Remove prays (or at leas make them closer to being removed)
			//List<Cell> toRemoveFromRemoveList = new List<Cell>();
			//foreach (KeyValuePair<Cell, PredatorPrayPair> removeMe in praysToRemove) {
			//	if (removeMe.Key != null) {
			//		if (removeMe.Value.linger <= 0) {
			//			UnpairPredatorPray(this, removeMe.Key);
			//			toRemoveFromRemoveList.Add(removeMe.Key);
			//		} else {
			//			removeMe.Value.linger--;
			//		}
			//	}
			//}
			//foreach (Cell remove in toRemoveFromRemoveList) {
			//	praysToRemove.Remove(remove);
			//}

			JawCellPanel.instance.MakeDirty();

			//foreach (Pray pray in prays.Values) {
			//	pray.UpdateMetabolism(this);
			//}
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
		//World.instance.life.UpdateSoulReferences();

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

			//if (!praysToAdd.Contains(prayCell)) {
			//	if (praysToRemove.ContainsKey(prayCell)) {
			//		praysToRemove.Remove(prayCell);
			//	}
			//	praysToAdd.Add(prayCell);
			//}
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
			//if (!praysToRemove.ContainsKey(prayCell)) {
			//	PredatorPrayPair pair = new PredatorPrayPair(prayCell);
			//	pair.linger = GlobalSettings.instance.phenotype.jawCellEatLinger;
			//	praysToRemove.Add(prayCell, pair);
			//}			
			UnpairPredatorPray(this, prayCell);
		} else {
			//Debug.Log("Ooops!");
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
		//if (praysToAdd.Contains(prayCell)) {
		//	praysToAdd.Remove(prayCell);
		//}
	}

	public override void OnRecycleCell() {
		deleteFlagged = true;

		//Free all prays from me since i excint no more
		foreach (Cell prayCell in prays.Keys) {
			prayCell.RemovePredator(this);
		}
		prays.Clear();
		
		base.OnRecycleCell();
	}

	public override void OnBorrowToWorld() {
		deleteFlagged = false;
		BeforeKill();
		base.OnRecycleCell();
	}

	//--------

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
	}

	public override void BeforeKill() {
		base.BeforeKill();
		//Free all prays from me since i excint no more
		foreach (Cell prayCell in prays.Keys) {
			prayCell.RemovePredator(this);
		}
		prays.Clear();

		//praysToAdd.Clear();
		//praysToRemove.Clear();
	}

	//public override void UpdateSpringFrequenzy() {
	//	base.UpdateSpringFrequenzy();

	//	if (HasOwnNeighbourCell(CardinalEnum.north)) {
	//		northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
	//		northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
	//	}

	//	if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
	//		southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
	//		southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
	//	}

	//	if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
	//		southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
	//		southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
	//	}
	//}
}


//using System.Collections.Generic;
//using UnityEngine;
//public class JawCell : Cell {

//	public JawCellMouth mouth;
//	//public List<Cell> prays = new List<Cell>(); //Who am i eating on
//	public List<Pray> prays = new List<Pray>(); //Who am i eating on

//	public JawCell() : base() {
//		springFrequenzy = 5f;
//		springDamping = 11f;
//	}

//	//Todo: Take ram speed into account
//	public override void UpdateMetabolism(int deltaTicks, ulong worldTicks) {
//		effectConsumptionInternal = GlobalSettings.instance.phenotype.jawCellEffectCost;

//		float weightedPrayCount = 0f;
//		foreach (Cell pray in prays) {
//			if (pray.GetCellType() == CellTypeEnum.Jaw) {
//				weightedPrayCount += GlobalSettings.instance.phenotype.jawCellEatJawCellFactor;
//			} else if (pray.GetCellType() == CellTypeEnum.Shell) {
//				weightedPrayCount += GlobalSettings.instance.phenotype.jawCellEatShellSellFactor;
//			} else {
//				weightedPrayCount += 1f;
//			}
//		}
//		effectProduction = weightedPrayCount * GlobalSettings.instance.phenotype.jawCellEatEffect;

//		//Hack release pray
//		RemoveNullPrays();

//		base.UpdateMetabolism(deltaTicks, worldTicks);
//	}

//	public int prayCount {
//		get {
//			return prays.Count;
//		}
//	}

//	//Called from cell mouth
//	public void TriggerEnter(Collider2D other) {
//		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
//			return;
//		}

//		World.instance.life.UpdateSoulReferences();

//		Cell prayCell = other.GetComponent<Cell>();

//		if (!creature.hasSoul) {
//			return;
//		}

//		if (prayCell != null && prayCell.creature != creature) {
//			// dont eat mother, grandma is OK (what about other siblings and cousins in the same cluster?)
//			if (creature.soul.motherSoulReference.id == prayCell.creature.id) {
//				return;
//			}
//			// don't eat children, grandchildren is OK (what about other siblings and cousins in the same cluster?)
//			foreach (Creature child in creature.children) { //Note: all references in children are not updated at this point
//				if (prayCell.creature == child) {
//					return;
//				}
//			}

//			PairPredatorPray(this, prayCell);
//		}
//		PhenotypePanel.instance.MakeDirty();
//		CellPanel.instance.MakeDirty();
//		JawCellPanel.instance.MakeDirty();
//	}

//	//Called from cell mouth
//	public void TriggerExit(Collider2D other) {
//		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
//			return;
//		}
//		//World.instance.life.UpdateSoulReferences();

//		Cell prayCell = other.GetComponent<Cell>();

//		if (prayCell != null) {
//			UnpairPredatorPray(this, prayCell);
//		} else {
//			//Debug.Log("Ooops!");
//			RemoveNullPrays();
//		}
//		PhenotypePanel.instance.MakeDirty();
//		CellPanel.instance.MakeDirty();
//		JawCellPanel.instance.MakeDirty();
//	}

//	public void RemoveNullPrays() {
//		List<Cell> keep = new List<Cell>();
//		foreach (Cell pray in prays) {
//			if (pray != null) {
//				keep.Add(pray);
//			}
//		}
//		prays.Clear();
//		prays.AddRange(keep);
//	}

//	private void AddPray(Cell pray) {
//		if (!prays.Contains(pray)) {
//			prays.Add(pray);
//		}
//	}

//	private void PairPredatorPray(JawCell predator, Cell pray) {
//		AddPray(pray);
//		pray.AddPredator(predator);
//	}

//	private void UnpairPredatorPray(JawCell predator, Cell pray) {
//		RemovePray(pray);
//		pray.RemovePredator(predator);
//	}

//	public void RemovePray(Cell pray) {
//		if (prays.Contains(pray)) {
//			prays.Remove(pray);
//		}
//	}

//	//--------

//	public override CellTypeEnum GetCellType() {
//		return CellTypeEnum.Jaw;
//	}

//	public override void UpdateSpringFrequenzy() {
//		base.UpdateSpringFrequenzy();

//		if (HasOwnNeighbourCell(CardinalEnum.north)) {
//			northSpring.frequency = (this.springFrequenzy + northNeighbour.cell.springFrequenzy) / 2f;
//			northSpring.dampingRatio = (this.springDamping + northNeighbour.cell.springDamping) / 2f;
//		}

//		if (HasOwnNeighbourCell(CardinalEnum.southWest)) {
//			southWestSpring.frequency = (this.springFrequenzy + southWestNeighbour.cell.springFrequenzy) / 2f;
//			southWestSpring.dampingRatio = (this.springDamping + southWestNeighbour.cell.springDamping) / 2f;
//		}

//		if (HasOwnNeighbourCell(CardinalEnum.southEast)) {
//			southEastSpring.frequency = (this.springFrequenzy + southEastNeighbour.cell.springFrequenzy) / 2f;
//			southEastSpring.dampingRatio = (this.springDamping + southEastNeighbour.cell.springDamping) / 2f;
//		}
//	}
//}