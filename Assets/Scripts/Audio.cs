using UnityEngine;

public class Audio : MonoSingleton<Audio> {
	public AudioSource audioSource;

	public AudioClip[] placeCreature;
	public AudioClip cellBirth;
	public AudioClip cellDeath;
	public AudioClip creatureDetatch;
	public AudioClip eggCellFertilize;

	public void PlaceCreature() {
		audioSource.clip = placeCreature[Random.Range(0, placeCreature.Length)];
		audioSource.Play();
	}

	public void CellBirth() {
		audioSource.clip = cellBirth;
		audioSource.Play();
	}

	public void CellDeath() {
		audioSource.clip = cellDeath;
		audioSource.Play();
	}

	public void CreatureDetatch() {
		audioSource.clip = creatureDetatch;
		audioSource.Play();
	}

	public void EggCellFertilize() {
		audioSource.clip = eggCellFertilize;
		audioSource.Play();
	}

}