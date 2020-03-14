using UnityEngine;

public class Pray {
	public Cell cell;
	public float prayEatenEffect;
	public float predatorEatEffect;

	public float ramSpeed;

	public Pray(Cell cell) {
		this.cell = cell;
	}

	public void UpdateMetabolism(Cell predatorCell) {
		ramSpeed = GetRamSpeed(predatorCell, cell);
		float jawEatEffect = GlobalSettings.instance.phenotype.jawCell.effectProductionUpAtSpeed.Evaluate(Mathf.Max(0f, ramSpeed));

		prayEatenEffect = jawEatEffect / cell.armour;
		predatorEatEffect = (jawEatEffect / cell.armour) * GlobalSettings.instance.phenotype.jawCell.effectProductionUpKeepFactor;
	}

	//1.7 m/s (Delta Jaw 15) ramming as a rhino, 0.2 m/s ramming as a rabbit 
	private float GetRamSpeed(Cell predatorCell, Cell prayCell) {
		Vector2 predatorToPrayVector = (prayCell.position - predatorCell.position).normalized;
		float ramSpeed = Vector2.Dot(predatorCell.velocity + prayCell.velocity, predatorToPrayVector); // Is the predator approaching the pray faster than the pray is approaching ther predator?
		return ramSpeed;
	}
}
