using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Audio : MonoSingleton<Audio> {
	public AudioClip cellBirth;
	public AudioClip cellDeath;

	public AudioClip[] creatureAdd;
	public AudioClip creaturePlace;
	public AudioClip creatureBirth;
	public AudioClip creatureDetatch;
	public AudioClip creatureTeleport;
	public AudioClip[] creatureDeath;

	private Dictionary<AudioClip, AudioSource> audioSource = new Dictionary<AudioClip, AudioSource>();

	public AudioClip actionAbort;
	public AudioClip actionDenied;

	void Awake() {
		audioSource.Add(cellBirth, gameObject.AddComponent<AudioSource>());
		audioSource.Add(cellDeath, gameObject.AddComponent<AudioSource>());

		foreach (AudioClip a in creatureAdd) {
			audioSource.Add(a, gameObject.AddComponent<AudioSource>());
		}

		audioSource.Add(creaturePlace, gameObject.AddComponent<AudioSource>());
		audioSource.Add(creatureBirth, gameObject.AddComponent<AudioSource>());
		audioSource.Add(creatureDetatch, gameObject.AddComponent<AudioSource>());
		audioSource.Add(creatureTeleport, gameObject.AddComponent<AudioSource>());

		foreach (AudioClip a in creatureDeath) {
			audioSource.Add(a, gameObject.AddComponent<AudioSource>());
		}

		audioSource.Add(actionAbort, gameObject.AddComponent<AudioSource>());
		audioSource.Add(actionDenied, gameObject.AddComponent<AudioSource>());

		foreach (KeyValuePair<AudioClip, AudioSource> pair in audioSource) {
			pair.Value.clip = pair.Key;
		}
	}

	public void CellBirth(float volume) {
		Play(cellBirth, volume);
	}

	public void CellDeath(float volume) {
		Play(cellDeath, volume);
	}

	public void CreatureAdd(float volume) {
		Play(creatureAdd[Random.Range(0, creatureAdd.Length)], volume);
	}

	public void CreaturePlace(float volume) {
		Play(creaturePlace, volume);
	}

	public void CreatureBirth(float volume) {
		Play(creatureBirth, volume);
	}

	public void CreatureDetatch(float volume) {
		Play(creatureDetatch, volume);
	}

	public void CreatureTeleport(float volume) {
		Play(creatureTeleport, volume);
	}

	public void CreatureDeath(float volume) {
		Play(creatureDeath[Random.Range(0, creatureAdd.Length)], volume);
	}

	public void ActionAbort(float volume) {
		Play(actionAbort, volume);
	}

	public void ActionDenied(float volume) {
		Play(actionDenied, volume);
	}

	private void Play(AudioClip clip, float volume) {
		audioSource[clip].volume = volume;
		audioSource[clip].Play();
	}
}