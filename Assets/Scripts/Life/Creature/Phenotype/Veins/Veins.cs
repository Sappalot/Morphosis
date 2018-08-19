using System.Collections.Generic;
using UnityEngine;


public class Veins : MonoBehaviour {
	public Vein veinPrefab;

	private List<Vein> veinList = new List<Vein>();

	private bool HasVein(Cell cellA, Cell cellB) {
		return veinList.Find(v => v.IsPair(cellA, cellB)) != null;
	}

	private void Clear() {
		for (int index = 0; index < veinList.Count; index++) {
			//Destroy(veinList[index].gameObject);
			World.instance.life.veinPool.Recycle(veinList[index]);
		}
		veinList.Clear();
	}

	public void UpdateGraphics(bool showVeins) {
		for (int index = 0; index < veinList.Count; index++) {
			veinList[index].UpdateGraphics(showVeins);
		}
	}

	public void UpdateEffect(int deltaTicks) {
		foreach (Vein vein in veinList) {
			vein.UpdateFluxEffect();
		}

		foreach (Vein vein in veinList) {
			if (vein.isPlacentaVein) {
				continue;
			}

			vein.frontCell.effectFluxFromSelf = 0f;
			vein.frontCell.effectFluxToSelf = 0f;

			vein.backCell.effectFluxFromSelf = 0f;
			vein.backCell.effectFluxToSelf = 0f;
		}

		foreach (Vein vein in veinList) {
			if (vein.isPlacentaVein) {
				continue;
			}

			vein.frontCell.effectFluxFromSelf += vein.flowEffectFrontToBack;
			vein.backCell.effectFluxToSelf += vein.flowEffectFrontToBack;
		}
	}

	private Dictionary<Cell, float> myGivingCells = new Dictionary<Cell, float>();
	private Dictionary<Cell, float> childrenReceivingCells = new Dictionary<Cell, float>();

	public void UpdateCellsPlacentaEffects() {
		myGivingCells.Clear();
		childrenReceivingCells.Clear();

		// Let all mothers update their giving (placenta effect minus) and the child's receiving (placenta effect plus)
		foreach (Vein vein in veinList) {
			if (vein.isPlacentaVein) {
				//this vein is connecting mother placenta (front) to child root (back)

				//Add to mother cells giving
				if (!myGivingCells.ContainsKey(vein.frontCell)) {
					myGivingCells.Add(vein.frontCell, 0f);
				}
				myGivingCells[vein.frontCell] += vein.flowEffectFrontToBack;

				//Add to child cells receiving
				if (!childrenReceivingCells.ContainsKey(vein.backCell)) {
					childrenReceivingCells.Add(vein.backCell, 0f);
				}
				childrenReceivingCells[vein.backCell] += vein.flowEffectFrontToBack;
			}
		}

		foreach (Cell cell in myGivingCells.Keys) {
			cell.effectFluxToChildrenAttached = myGivingCells[cell];
		}

		foreach (Cell cell in childrenReceivingCells.Keys) {
			cell.effectFluxFromMotherAttached = childrenReceivingCells[cell];
		}
	}

	public void GenerateVeins(Creature creature, CellMap cellMap) {
		Clear();

		foreach (Cell cell in creature.phenotype.cellList) {
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) { //Including attached
				if (cell.HasNeighbourCell(cardinalIndex)) {
					Cell neighbour = cell.GetNeighbourCell(cardinalIndex);
					// dont build veins to mother placnta, beacuse she builds to me and we don't want to double build
					if (creature.HasMotherAlive() && creature.GetMotherAlive().id == neighbour.creature.id) {
						continue;
					}
					if (!HasVein(cell, neighbour)) {
						//Vein vein = (Instantiate(veinPrefab, transform.position, Quaternion.identity) as Vein);
						Vein vein = World.instance.life.veinPool.Borrow();

						vein.transform.parent = transform;
						vein.transform.position = transform.position;
						vein.Setup(cell, neighbour, AngleUtil.CardinalIndexRawToSafe(cardinalIndex + 3));
						veinList.Add(vein);
					}
				}
			}
		}
	}

	public void OnRecycle() {
		Clear();
	}
}