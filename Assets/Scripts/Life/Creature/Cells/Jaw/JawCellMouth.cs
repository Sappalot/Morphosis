using System.Collections.Generic;
using UnityEngine;

public class JawCellMouth : MonoBehaviour {
	[HideInInspector]
	public Creature creature;

	private List<Cell> prays = new List<Cell>();

	public int prayCount {
		get {
			return prays.Count;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		Cell prayCell = other.GetComponent<Cell>();
		Cell predatorCell = transform.parent.GetComponent<Cell>();
		if (prayCell != null && prayCell.creature != creature) {
			PairPredatorPray(predatorCell, prayCell);

			PhenotypePanel.instance.MakeDirty();
			CellPanel.instance.MakeDirty();
			JawCellPanel.instance.MakeDirty();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		Cell prayCell = other.GetComponent<Cell>();
		Cell predatorCell = transform.parent.GetComponent<Cell>();
		if (prayCell != null && prayCell.creature != creature) {
			UnpairPredatorPray(predatorCell, prayCell);

			PhenotypePanel.instance.MakeDirty();
			CellPanel.instance.MakeDirty();
			JawCellPanel.instance.MakeDirty();
		}
	}

	private void PairPredatorPray(Cell predator, Cell pray) {
		AddPray(pray);
		pray.AddPredator(predator);
	}

	private void UnpairPredatorPray(Cell predator, Cell pray) {
		RemovePray(pray);
		pray.RemovePredator(predator);
	}

	private void AddPray(Cell pray) {
		if (!prays.Contains(pray)) {
			prays.Add(pray);
		}
	}

	private void RemovePray(Cell pray) {
		if (prays.Contains(pray)) {
			prays.Remove(pray);
		}
	}
}
