using UnityEngine;
using SerializerFree;
using SerializerFree.Serializers;
public class Morphosis : MonoSingleton<Morphosis> {
	public new Camera camera;

	private void Start () {
		Freezer.instance.Load();
		World.instance.Init(); // Just 1 world, lots of work keeping several instances at once
		// TODO: load last world
	}

	public void OnExit() {
		Freezer.instance.Save();
	}

	// TODO: keep morphosis data in save file
	// camera position
	// last world played
}