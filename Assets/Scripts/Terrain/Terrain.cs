using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {
	public PidCircle pidCircle;
	
	// Update is called once per frame
	public void UpdatePhysics () {
		pidCircle.UpdatePhysics();
	}

	public void Restart() {
		pidCircle.Restart();
	}

	public void Init() {
		pidCircle.Init();
	}

	// Load / Sava

	private TerrainData terrainData = new TerrainData();

	// Save
	public TerrainData UpdateData() {
		terrainData.pidCircleData = pidCircle.UpdateData();
		return terrainData;
	}

	// Load
	public void ApplyData(TerrainData terrainData) {
		pidCircle.ApplyData(terrainData.pidCircleData);
	}
}
