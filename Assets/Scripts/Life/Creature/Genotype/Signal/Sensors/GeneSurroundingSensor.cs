using UnityEngine;

public class GeneSurroundingSensor : GeneSignalUnit {
	public GeneSurroundingSensor(SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.signalUnit = signalUnit;
		Defaultify();
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	private float m_direction = 0;
	// In which angle the eye is looking, 0 same as cells heading (in the direction of the black-white arrow)
	// possitive number is the angle from the heading towards the black side, negative is the white side
	// range [-180, 180] degrees
	public float direction {
		get {
			return m_direction;
		}
		set {
			m_direction = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	// The angle in which the eye can see stuff from one side to the other
	// range [0, 360] degrees
	private float m_fieldOfView = 0;
	public float fieldOfView {
		get {
			return m_fieldOfView;
		}
		set {
			m_fieldOfView = value;
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	// The furthest range at which we can see stuff
	// [0.6, 10] meters, should be higher than tangeNear though
	private float m_rangeFar = 0.6f;
	public float rangeFar {
		get {
			return m_rangeFar;
		}
		set {
			m_rangeFar = value;
			if (m_rangeFar < rangeNear) {
				rangeNear = m_rangeFar;
			}
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}

	// the closes range at which we can see stuff
	// [0.6, 10] meters, should be lower than rangeFar though
	private float m_rangeNear = 0.6f;
	public float rangeNear {
		get {
			return m_rangeNear;
		}
		set {
			m_rangeNear = value;
			if (m_rangeNear > rangeFar) {
				rangeFar = m_rangeNear;
			}
			genotypeDirtyfy.ReforgeCellPatternAndForward();
		}
	}



	public void Defaultify() {
		direction = 0f;
		fieldOfView = 90f;
		rangeFar = 8f;
		rangeNear = 0.6f;

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Randomize() {
		// TODO: randomize them!!
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Mutate(float strength) {
		// TODO: mutate stuff

		//GlobalSettings gs = GlobalSettings.instance;
		//float rnd;

		//rnd = Random.Range(0, gs.mutation.energySensorAreaRadiusChange * strength + 1000f);
		//if (rnd < gs.mutation.energySensorAreaRadiusChange * strength) {
		//	areaRadius = (int)Mathf.Clamp(areaRadius + gs.mutation.energySensorAreaRadiusChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, Creature.maxRadiusHexagon - 1);
		//}

		//rnd = Random.Range(0, gs.mutation.energySensorThresholdChange * strength + 1000f);
		//if (rnd < gs.mutation.energySensorThresholdChange * strength) {
		//	threshold = Mathf.Clamp(threshold + gs.mutation.energySensorThresholdChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, gs.phenotype.cellMaxEnergy);
		//}
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// Save
	private GeneSurroundingSensorData data = new GeneSurroundingSensorData();
	public GeneSurroundingSensorData UpdateData() {
		data.direction = direction;
		data.fieldOfView = fieldOfView;
		data.rangeFar = rangeFar;
		data.rangeNear = rangeNear;
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorData data) {
		direction = data.direction;
		fieldOfView = data.fieldOfView;
		rangeFar = data.rangeFar;
		rangeNear = data.rangeNear;
	}
}
