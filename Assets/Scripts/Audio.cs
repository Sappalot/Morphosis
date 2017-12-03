using UnityEngine;

public class Audio : MonoSingleton<Audio> {
	public AudioSource audioSource;

	public AudioClip[] placeCreature;
	public AudioClip cellBirth;
	public AudioClip cellDeath;
	public AudioClip creatureDetatch;
	public AudioClip eggCellFertilize;

	public void PlaceCreature(float volume) {
		audioSource.clip = placeCreature[Random.Range(0, placeCreature.Length)];
		audioSource.volume = volume;
		audioSource.Play();
	}

	public void CellBirth() {
		audioSource.clip = cellBirth;
		audioSource.Play();
	}

	public void CellDeath(float volume) {
		audioSource.clip = cellDeath;
		audioSource.volume = volume;
		audioSource.Play();
	}

	public void CreatureDetatch(float volume) {
		audioSource.clip = creatureDetatch;
		audioSource.volume = volume;
		audioSource.Play();
	}

	public void EggCellFertilize(float volume) {
		audioSource.clip = eggCellFertilize;
		audioSource.volume = volume;
		audioSource.Play();
	}

}