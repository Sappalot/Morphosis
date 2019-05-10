using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoSingleton<ParticlePool> {
	public ParticlesCellBirth particlesCellBirthPrefab;
	public ParticlesCellBleed particlesCellBleedPrefab;
	public ParticlesCellScatter particlesCellScatterPrefab;
	public ParticlesCellTeleport particlesCellTeleportPrefab;
	

	private List<bool> vacantPositions = new List<bool>();

	//what we are able to lend out without need to create new ones
	public int GetStoredParticlesCount(ParticleTypeEnum type) {
		return storedQueues[type].Count;
	}

	// This one should be 0 when no particles are in use
	public int GetLoanedParticlesCount(ParticleTypeEnum type) {
		return loanedCount[type];
	}

	private Queue<Particles> storedCellBirth = new Queue<Particles>();
	private Queue<Particles> storedCellBleed = new Queue<Particles>();
	private Queue<Particles> storedCellScatter = new Queue<Particles>();
	private Queue<Particles> storedCellTeleport = new Queue<Particles>();

	private Dictionary<ParticleTypeEnum, Queue<Particles>> storedQueues = new Dictionary<ParticleTypeEnum, Queue<Particles>>();
	private Dictionary<ParticleTypeEnum, int> loanedCount = new Dictionary<ParticleTypeEnum, int>(); //How many cells of a specific type are we expected to get back, if they ere all deleted from the world
	private Dictionary<ParticleTypeEnum, int> serialNumber = new Dictionary<ParticleTypeEnum, int>();

	private void Awake() {
		storedQueues.Add(ParticleTypeEnum.cellBirth, storedCellBirth);
		storedQueues.Add(ParticleTypeEnum.cellBleed, storedCellBleed);
		storedQueues.Add(ParticleTypeEnum.cellScatter, storedCellScatter);
		storedQueues.Add(ParticleTypeEnum.cellTeleport, storedCellTeleport);

		loanedCount.Add(ParticleTypeEnum.cellBirth, 0);
		loanedCount.Add(ParticleTypeEnum.cellBleed, 0);
		loanedCount.Add(ParticleTypeEnum.cellScatter, 0);
		loanedCount.Add(ParticleTypeEnum.cellTeleport, 0);

		serialNumber.Add(ParticleTypeEnum.cellBirth, 0);
		serialNumber.Add(ParticleTypeEnum.cellBleed, 0);
		serialNumber.Add(ParticleTypeEnum.cellScatter, 0);
		serialNumber.Add(ParticleTypeEnum.cellTeleport, 0);
	}

	public Particles Borrow(ParticleTypeEnum type) {
		Particles borrowParticles = null;
		Particles poppedParticles = PopParticles(storedQueues[type]);


		if (poppedParticles != null) {
			borrowParticles = poppedParticles;
		} else {
			borrowParticles = Instantiate(type);
		}

		borrowParticles.OnBorrow();
		loanedCount[type]++;
		return borrowParticles;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(Particles particles) {
		particles.OnRecycle();

		particles.transform.parent = transform;
		particles.gameObject.SetActive(false);
		storedQueues[particles.GetParticlesType()].Enqueue(particles);
		loanedCount[particles.GetParticlesType()]--;
	}

	private Particles PopParticles(Queue<Particles> queue) {
		if (queue.Count > 0) {
			Particles particles = queue.Dequeue();
			particles.gameObject.SetActive(true);
			return particles;
		}
		return null;
	}

	private Particles Instantiate(ParticleTypeEnum type) {
		Particles particles = null;
		if (type == ParticleTypeEnum.cellBirth) {
			particles = (Instantiate(particlesCellBirthPrefab, Vector3.zero, Quaternion.identity) as Particles);
		} else if (type == ParticleTypeEnum.cellBleed) {
			particles = (Instantiate(particlesCellBleedPrefab, Vector3.zero, Quaternion.identity) as Particles);
		} else if (type == ParticleTypeEnum.cellScatter) {
			particles = (Instantiate(particlesCellScatterPrefab, Vector3.zero, Quaternion.identity) as Particles);
		} else if (type == ParticleTypeEnum.cellTeleport) {
			particles = (Instantiate(particlesCellTeleportPrefab, Vector3.zero, Quaternion.identity) as Particles);
		}

		particles.name = type.ToString() + " " + serialNumber[type]++;
		particles.transform.parent = transform;

		return particles;
	}

	private int FirstVacantPosition() {
		for (int pos = 0; pos < vacantPositions.Count; pos++) {
			if (vacantPositions[pos]) {
				return pos;
			}
		}
		vacantPositions.Add(true);

		return vacantPositions.Count - 1;
	}

	private void OccupyPosition(int pos) {
		vacantPositions[pos] = false;
	}

	private void FreePosition(int pos) {
		vacantPositions[pos] = true;
	}
}