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
		float jawEatEffect = GlobalSettings.instance.phenotype.jawCellEatEffectAtSpeed.Evaluate(Mathf.Max(0f, ramSpeed));

		if (cell.GetCellType() == CellTypeEnum.Jaw) {
			prayEatenEffect =   jawEatEffect;
			predatorEatEffect = jawEatEffect * GlobalSettings.instance.phenotype.jawCellMutualEatKindness;
		} else if (cell.GetCellType() == CellTypeEnum.Shell) {
			prayEatenEffect = predatorEatEffect = jawEatEffect * GlobalSettings.instance.phenotype.shellCellWeaknessFactor;
		} else {
			prayEatenEffect = predatorEatEffect = jawEatEffect;
		}
	}

	//1.7 m/s (Delta Jaw 15) ramming as a rhino, 0.2 m/s ramming as a rabbit 
	private float GetRamSpeed(Cell predatorCell, Cell prayCell) {
		Vector2 predatorToPrayVector = (prayCell.position - predatorCell.position).normalized;
		float ramVelocity = Vector2.Dot(predatorCell.velocity, predatorToPrayVector);
		return ramVelocity;
	}
}
