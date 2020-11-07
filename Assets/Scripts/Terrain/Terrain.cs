using System.Collections.Generic;
using UnityEngine;

//namespace MorphosisTerrain { // Problem with conflict with unitys Terrain class

//[ExecuteInEditMode]
public class Terrain : MonoBehaviour {

	public Portals portals;
	public TerrainPerimeter terrainPerimeter;
	public Transform eastWall;
	public Transform southWall;
	public Transform southEastCorner;

	public Vector2i totalSize; // the size when trunkated to the nearest tile (floor)
	private Vector2i totalSizeTiled;

	public const int tileSide = 5; // the side of a tile in meters

	private bool isDirty;

	public void MakeDirty() {
		isDirty = true;
	}

	private Vector2i m_totalSizeWanted; // includes departure area
	public Vector2i totalSizeWanted {
		set {
			m_totalSizeWanted = value;
			totalSizeTiled = new Vector2i(value.x / tileSide, value.y / tileSide);
			totalSize = new Vector2i(totalSizeTiled.x * tileSide, totalSizeTiled.y * tileSide);
			terrainPerimeter.liveZoneSize = new Vector2i(totalSize.x, totalSize.y);

			// graphics
			eastWall.transform.localPosition = new Vector2(totalSize.x, 0f);
			southWall.transform.localPosition = new Vector2(0f, -totalSize.y);
			southEastCorner.transform.localPosition = new Vector2(totalSize.x, -totalSize.y);

			// teleporters, calculated from transforms set above
			portals.UpdatePortalFlights();

			MakeDirty();
		}
		get {
			return m_totalSizeWanted;
		}
	}

	public void Restart() {
		totalSizeWanted = new Vector2i(160, 160);
	}

	public bool IsInside(Vector2 position) {
		return terrainPerimeter.IsInside(position);
	}

	public bool IsCompletelyInside(Creature creature) {
		return terrainPerimeter.IsCompletelyInside(creature);
	}

	public void UpdatePhysics(ulong worldTicks) {
		if (PhenotypePhysicsPanel.instance.teleport.isOn) {
			portals.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}

		if (PhenotypePhysicsPanel.instance.killEscaping.isOn) {
			terrainPerimeter.UpdatePhysics(World.instance.life.creatures, worldTicks);
		}
	}

	// Load / Sava
	private TerrainData terrainData = new TerrainData();

	// Save
	public TerrainData UpdateData() {
		terrainData.totalSizeWanted = totalSizeWanted;
		return terrainData;
	}

	// Load
	public void ApplyData(TerrainData terrainData) {
		totalSizeWanted = terrainData.totalSizeWanted;
	}
}
//}
