using UnityEngine;

public class Pray {
	public Cell cell;
	public float prayEatenEffect;
	public float predatorEatEffect;

	public Pray(Cell cell) {
		this.cell = cell;
	}

	public void UpdateMetabolism(Cell predatorCell) {
		float ramSpeed = GetRamSpeed(predatorCell, cell);
		//float jawEatEffect = 50f; // GlobalSettings.instance.phenotype.jawCellEatEffectAtSpeed.Evaluate(Mathf.Max(0f, ramSpeed));

		bool isShell = cell.GetCellType() == CellTypeEnum.Shell;
		//foreach (Cell c in cell.GetNeighbourCells()) {
		//	if (c.GetCellType() == CellTypeEnum.Shell) {
		//		isShell = true;
		//	}
		//}

		if (cell.GetCellType() == CellTypeEnum.Jaw) {
			prayEatenEffect = GlobalSettings.instance.phenotype.jawCellEatEffect;
			predatorEatEffect = GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.jawCellMutualEatKindness;
		} else if (isShell && GlobalPanel.instance.physicsShell) {
			prayEatenEffect = GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.shellCellWeaknessFactor;
			predatorEatEffect = GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.shellCellWeaknessFactor * GlobalSettings.instance.phenotype.jawCellEatEarnFactor;
		} else {
			prayEatenEffect = GlobalSettings.instance.phenotype.jawCellEatEffect;
			predatorEatEffect = GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.jawCellEatEarnFactor;
		}
	}

	//1.7 m/s (Delta Jaw 15) ramming as a rhino, 0.2 m/s ramming as a rabbit 
	private float GetRamSpeed(Cell predatorCell, Cell prayCell) {
		Vector2 predatorToPrayVector = (prayCell.position - predatorCell.position).normalized;
		float ramSpeed = Vector2.Dot(predatorCell.velocity, predatorToPrayVector);
		return ramSpeed;
	}
}
