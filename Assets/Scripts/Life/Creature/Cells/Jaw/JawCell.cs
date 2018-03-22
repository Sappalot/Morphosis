using System.Collections.Generic;
using UnityEngine;
public class JawCell : Cell {

	public JawCellMouth mouth;
	[HideInInspector]
	public Dictionary<Cell, Pray> prays = new Dictionary<Cell, Pray>(); //Who am i eating on? Me gain? other lose?

	public JawCell() : base() {
		springFrequenzy = 5f;
		springDamping = 11f;
	}

	public override void UpdateCellFunction(int deltaTicks, ulong worldTicks) {
		effectConsumptionInternal = GlobalSettings.instance.phenotype.jawCellEffectCost;

		//Hack release pray
		RemoveNullPrays(); //We need this one not to run into null refs once in a blue moon
		
		foreach (Pray pray in prays.Values) {
			pray.UpdateMetabolism(this);
		}
		effectProductionExternal = eatEffect;

		base.UpdateCellFunction(deltaTicks, worldTicks);
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

		Life.instance.UpdateSoulReferences();

		Cell prayCell = other.GetComponent<Cell>();

		if (!creature.hasSoul) {
			return;
		}

		if (prayCell != null && prayCell.creature != creature) {
			// dont eat mother, grandma is OK (what about other siblings and cousins in the same cluster?)
			if (creature.soul.motherSoulReference.id == prayCell.creature.id) {
				return;
			}
			// don't eat children, grandchildren is OK (what about other siblings and cousins in the same cluster?)
			foreach (Creature child in creature.children) { //Note: all references in children are not updated at this point
				if (prayCell.creature == child) {
					return;
				}
			}

			PairPredatorPray(this, prayCell);
		}

		//UpdateEffect();

		PhenotypePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		JawCellPanel.instance.MakeDirty();
	}

	//Called from cell mouth
	public void TriggerExit(Collider2D other) {
		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
			return;
		}
		//Life.instance.UpdateSoulReferences();

		Cell prayCell = other.GetComponent<Cell>();

		if (prayCell != null) {
			UnpairPredatorPray(this, prayCell);
		} else {
			//Debug.Log("Ooops!");
			RemoveNullPrays();
		}

		//UpdateEffect();

		PhenotypePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		JawCellPanel.instance.MakeDirty();
	}

	public void RemoveNullPrays() {
		List<Pray> remove = new List<Pray>();
		foreach (Pray pray in prays.Values) {
			if (pray.cell == null) {
				remove.Add(pray);
			}
		}
		foreach (Pray pray in remove) {
			pray.cell.ramSpeed = 0f;
			prays.Remove(pray.cell);
		}
	}

	private void AddPray(Pray pray) {
		if (!prays.ContainsKey(pray.cell)) {
			prays.Add(pray.cell, pray);
		}
	}

	private void PairPredatorPray(JawCell predatorCell, Cell prayCell) {
		AddPray(new Pray(prayCell)); //TODO update effect
		prayCell.AddPredator(predatorCell);
	}

	private void UnpairPredatorPray(JawCell predatorCell, Cell prayCell) {
		prayCell.ramSpeed = 0f;
		predatorCell.ramSpeed = 0f;
		RemovePray(prayCell);
		prayCell.RemovePredator(predatorCell);
	}

	public void RemovePray(Pray pray) {
		if (prays.ContainsKey(pray.cell)) {
			pray.cell.ramSpeed = 0f;
			prays.Remove(pray.cell);
		}
	}

	public void RemovePray(Cell prayCell) {
		prayCell.ramSpeed = 0f;
		if (prays.ContainsKey(prayCell)) {
			prays.Remove(prayCell);
		}
	}

	//--------

	public override CellTypeEnum GetCellType() {
		return CellTypeEnum.Jaw;
	}

	public override void UpdateSpringFrequenzy() {
		base.UpdateSpringFrequenzy();

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

//		Life.instance.UpdateSoulReferences();

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
//		//Life.instance.UpdateSoulReferences();

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