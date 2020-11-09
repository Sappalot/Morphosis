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

	public const int tileSide = 5; // the side of a tile in meters

	private bool isDirty;

	public void MakeDirty() {
		isDirty = true;
	}

	private const int defaultWidthDepartureExlusive = 120;
	private const int defaultHeightDepartureExclusive = 120;

	private const int minWidthDepartureExclusive = 60;
	private const int minHeightDepartureExclusive = 60;

	private const int maxWidthDepartureExclusive = 560;
	private const int maxHeightDepartureExclusive = 560;

	private const int departureAreaWidth = 20; // same as height

	private Vector2i sizeDepartureExclusiveTileCount {
		get {
			return new Vector2i(sizeDepartureExclusive.x / tileSide, sizeDepartureExclusive.y / tileSide);
		}
	}

	private Vector2i sizeDepartureInclusiveTileCount { 
		get {
			return new Vector2i(sizeDepartureInclusive.x / tileSide, sizeDepartureInclusive.y / tileSide);
		}
	}

	// Only what is used, so without the departure zones
	private Vector2i m_sizeDepartureExclusive = new Vector2i(defaultWidthDepartureExlusive, defaultHeightDepartureExclusive);
	public Vector2i sizeDepartureExclusive { // will clamp to legal values and truncated down to the nearest tile
		set {
			int widthClamped = Mathf.Clamp(value.x, minWidthDepartureExclusive, maxWidthDepartureExclusive);
			int heightClamped = Mathf.Clamp(value.y, minHeightDepartureExclusive, maxHeightDepartureExclusive);

			int widthClampedTruncated = (widthClamped / tileSide) * tileSide;
			int heightClampedTruncated = (heightClamped / tileSide) * tileSide;
			Vector2i sizeDepartureExclusiveTileCount = new Vector2i(widthClamped / tileSide, heightClamped / tileSide);
			
			m_sizeDepartureExclusive = new Vector2i(widthClampedTruncated, heightClampedTruncated);
			terrainPerimeter.liveZoneSize = new Vector2i(sizeDepartureInclusive.x, sizeDepartureInclusive.y);

			// graphics
			eastWall.transform.localPosition = new Vector2(sizeDepartureInclusive.x, 0f);
			southWall.transform.localPosition = new Vector2(0f, -sizeDepartureInclusive.y);
			southEastCorner.transform.localPosition = new Vector2(sizeDepartureInclusive.x, -sizeDepartureInclusive.y);

			// teleporters, calculated from transforms set above
			portals.UpdatePortalFlights();

			MakeDirty();
		}
		get {
			return m_sizeDepartureExclusive;
		}
	}

	private Vector2i sizeDepartureInclusive { // truncated down to the nearest tile, including departure areas
		get {
			return new Vector2i(sizeDepartureExclusive.x + departureAreaWidth * 2, sizeDepartureExclusive.y + departureAreaWidth * 2); 
		}
	}

	public void Restart() {
		sizeDepartureExclusive = new Vector2i(defaultWidthDepartureExlusive, defaultHeightDepartureExclusive);
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
		terrainData.sizeDepartureExclusive = sizeDepartureExclusive;
		return terrainData;
	}

	// Load
	public void ApplyData(TerrainData terrainData) {
		sizeDepartureExclusive = terrainData.sizeDepartureExclusive;
	}
}
//}
