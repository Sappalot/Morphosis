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
			Destroy(veinList[index].gameObject);
		}
		veinList.Clear();
	}

	public void UpdateGraphics(bool showVeins) {
		for (int index = 0; index < veinList.Count; index++) {
			veinList[index].UpdateGraphics(showVeins);
		}
	}

	public void UpdateEffectAndEnergy(int deltaTicks) {
		foreach (Vein vein in veinList) {
			vein.UpdateEnergyFluxEffect();
		}

		foreach (Vein vein in veinList) {
			vein.UpdateEnergy(deltaTicks);
		}
	}

	public void GenerateVeins(Creature creature, CellMap cellMap) {
		Clear();

		foreach (Cell cell in creature.phenotype.cellList) {
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) { //Including attached
				if (cell.HasNeighbourCell(cardinalIndex)) {
					Cell neighbour = cell.GetNeighbourCell(cardinalIndex);
					// dont build veins to mother placnta, beacuse she builds to me and we dont want to double build
					if (creature.soul.motherSoulReference.id == neighbour.creature.id) {
						continue;
					}
					if (!HasVein(cell, neighbour)) {
						Vein vein = (Instantiate(veinPrefab, transform.position, Quaternion.identity) as Vein);
						vein.transform.parent = transform;
						vein.Setup(cell, neighbour, AngleUtil.CardinalIndexRawToSafe(cardinalIndex + 3));
						veinList.Add(vein);
					}
				}
			}
		}
	}
}