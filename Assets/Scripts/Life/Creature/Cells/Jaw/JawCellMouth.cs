using System.Collections.Generic;
using UnityEngine;

public class JawCellMouth : MonoBehaviour {
	public List<Cell> prays = new List<Cell>(); //Who am i eating on

	public int prayCount {
		get {
			return prays.Count;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
			return;
		}

		Life.instance.UpdateSoulReferences();

		Cell prayCell = other.GetComponent<Cell>();
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();

		Creature creature = predatorCell.creature;
		if (!creature.hasSoul) {
			return;
		}

		if (prayCell != null && prayCell.creature != creature) {
			// dont eat mother, grandma is OK 
			if (creature.soul.motherSoulReference.id == prayCell.creature.id) {
				return;
			}
			// don't eat children, grandchildren is OK
			foreach (Creature child in creature.children) { //Note: all references in children are not updated at this point
				if (prayCell.creature == child) {
					return;
				}
			}

			PairPredatorPray(predatorCell, prayCell);
		}
		PhenotypePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		JawCellPanel.instance.MakeDirty();
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == 2) { //dont trigger other's mouth colliders, only on cells
			return;
		}
		//Life.instance.UpdateSoulReferences();

		Cell prayCell = other.GetComponent<Cell>();
		JawCell predatorCell = transform.parent.GetComponent<JawCell>();

		Creature creature = predatorCell.creature;

		if (prayCell != null) {
			UnpairPredatorPray(predatorCell, prayCell);
		} else {
			//Debug.Log("Ooops!");
			RemoveNullPrays();
		}
		PhenotypePanel.instance.MakeDirty();
		CellPanel.instance.MakeDirty();
		JawCellPanel.instance.MakeDirty();
	}

	public void RemoveNullPrays() {
		List<Cell> keep = new List<Cell>();
		foreach (Cell pray in prays) {
			if (pray != null) {
				keep.Add(pray);
			}
		}
		prays.Clear();
		prays.AddRange(keep);
	}

	private void PairPredatorPray(JawCell predator, Cell pray) {
		AddPray(pray);
		pray.AddPredator(predator);
	}

	private void UnpairPredatorPray(JawCell predator, Cell pray) {
		RemovePray(pray);
		pray.RemovePredator(predator);
	}

	private void AddPray(Cell pray) {
		if (!prays.Contains(pray)) {
			prays.Add(pray);
		}
	}

	public void RemovePray(Cell pray) {
		if (prays.Contains(pray)) {
			prays.Remove(pray);
		}
	}
}
