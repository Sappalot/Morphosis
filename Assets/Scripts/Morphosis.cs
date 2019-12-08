using UnityEngine;
using System;

public class Morphosis : MonoSingleton<Morphosis> {
	public IdGenerator idGenerator = new IdGenerator();
	public new Camera camera;

	// TODO: Move to Morphosis, since they are used in freezer as well
	public CreaturePool creaturePool;
	public CellPool cellPool;
	public GeneCellPool geneCellPool;
	public VeinPool veinPool;
	public EdgePool edgePool;
	public RelationArrows relationArrows;

	public static string savePath {
		get {
			return Application.persistentDataPath + "/";
		}
	}

	private void Start () {
		Debug.Log(Application.persistentDataPath);
		Application.runInBackground = true;
		CellMap.Init();

		// Creature id's will be set from file
		World.instance.Init(); // Just 1 world, lots of work keeping several instances at once

		MouseAction.instance.actionState = MouseActionStateEnum.loadingFreezer;

		ProgressBar.instance.gameObject.SetActive(true);
		ProgressBar.instance.heading.text = "Preparing Morphosis";
		FreezerData freezerData = Freezer.instance.LoadFreezerData();
		ProgressBar.instance.ResetForStartup(freezerData.creatureList.Count);

		Restart(freezerData, () => {
			ProgressBar.instance.gameObject.SetActive(false);
			MouseAction.instance.actionState = MouseActionStateEnum.free;
		});
	}

	public Cell GetCellAtPosition(Vector2 pickPosition) {
		if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype) {
			if (TerrainPerimeter.instance.IsInside(pickPosition)) {
				return World.instance.life.GetCellAtPosition(pickPosition);
			} else if (Freezer.instance.IsInside(pickPosition)) {
				return Freezer.instance.GetCellAtPosition(pickPosition);
			}
		} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			if (TerrainPerimeter.instance.IsInside(pickPosition)) {
				return World.instance.life.GetGeneCellAtPosition(pickPosition, CreatureSelectionPanel.instance.soloSelected);
			} else if (Freezer.instance.IsInside(pickPosition)) {
				return Freezer.instance.GetGeneCellAtPosition(pickPosition);
			}
		}
		return null;
	}


	private void Update() {
		World.instance.UpdateGraphics();
		Freezer.instance.UpdateGraphics();
	}

	private void FixedUpdate() {
		World.instance.UpdatePhysics();
		Freezer.instance.UpdatePhysics();
	}

	public void OnExit() {
		Freezer.instance.Save();
	}

	public void MoveFreezerCreatureIdsToFreeRange() {
		idGenerator.RenameToUniqueIds(Freezer.instance.creatures);
	}

	public void Restart(Action onDone) {
		idGenerator.Restart();
		Freezer.instance.Load(() => {
			World.instance.Restart(() => {
				onDone();
			});
		});
	}

	public void Restart(FreezerData freezerData, Action onDone) {
		idGenerator.Restart();
		Freezer.instance.Load(freezerData, () => {
			World.instance.Restart(() => {
				onDone();
			});
		});
	}

	public void Restart(FreezerData freezerData, WorldData worldData, Action onDone) {
		idGenerator.Restart();
		Freezer.instance.Load(freezerData, () => {
			World.instance.Restart(() => {
				onDone();
			});
		});
	}

	public void LoadWorld(string filename, Action onDone) {
		GlobalPanel.instance.SelectPausePhysics();

		Freezer.instance.Save();
		Restart(() => {
			World.instance.Load(filename, () => {
				instance.MoveFreezerCreatureIdsToFreeRange();
				onDone();
			});
		});
	}

	public void LoadWorld(WorldData worldData, Action onDone) {
		GlobalPanel.instance.SelectPausePhysics();

		Freezer.instance.Save();
		Restart(() => {
			World.instance.Load(worldData, () => {
				instance.MoveFreezerCreatureIdsToFreeRange();
				onDone();
			});
		});
	}

	public static bool isInterferredByOtheActions() {
		return MouseAction.instance.actionState != MouseActionStateEnum.free || AlternativeToolModePanel.instance.isOn || !World.instance.creatureSelectionController.IsIdle;
	}
}