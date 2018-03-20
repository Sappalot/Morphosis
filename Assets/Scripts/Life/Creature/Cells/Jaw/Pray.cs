using UnityEngine;

public class Pray {
	public Cell cell;
	public float prayEatenEffect;
	public float predatorEatEffect;

	public Pray(Cell cell) {
		this.cell = cell;
	}

	public void UpdateMetabolism(Cell predatorCell) {

		float ramSpeedFactor = 1f;

		if (GlobalPanel.instance.physicsTeleport.isOn) {
			float ramSpeed = GetRamFactor(predatorCell, cell);
			cell.ramSpeed = 0f;
			predatorCell.ramSpeed = ramSpeed;
			ramSpeedFactor = 0.2f + Mathf.Max(0, ramSpeed * 1.5f);
		}

		if (cell.GetCellType() == CellTypeEnum.Jaw) {
			prayEatenEffect = ramSpeedFactor * GlobalSettings.instance.phenotype.jawCellEatEffect;
			predatorEatEffect = ramSpeedFactor * GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.jawCellMutualEatKindness;
		} else if (cell.GetCellType() == CellTypeEnum.Shell) {
			prayEatenEffect = predatorEatEffect = ramSpeedFactor * GlobalSettings.instance.phenotype.jawCellEatEffect * GlobalSettings.instance.phenotype.jawCellEatShellSellFactor;
		} else {
			prayEatenEffect = predatorEatEffect = ramSpeedFactor * GlobalSettings.instance.phenotype.jawCellEatEffect;
		}
	}

	//1 ramming as a rhino, 0.2 ramming as a rabbit 
	public float GetRamFactor(Cell predatorCell, Cell prayCell) {
		Vector2 predatorToPrayVector = (prayCell.position - predatorCell.position).normalized;
		//Vector2 relativeVelocity = predatorCell.velocity - prayCell.velocity;
		
		float ramVelocity = Vector2.Dot(predatorCell.velocity, predatorToPrayVector);

		return ramVelocity;
	}
}
