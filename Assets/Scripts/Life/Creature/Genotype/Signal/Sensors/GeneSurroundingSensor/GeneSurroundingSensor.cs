using System.Collections.Generic;
using UnityEngine;

public class GeneSurroundingSensor : GeneSignalUnit {
	public GeneSurroundingSensor(SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.signalUnit = signalUnit;

		// create all the sensor
		for( int c = 1; c < 7; c++) {
			channelDictionaryAtChannel[c] = new Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>();
			channelDictionaryAtChannel[c].Add(SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov, new GeneSurroundingSensorChannelCreatureCellFovCov());
			channelDictionaryAtChannel[c].Add(SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov, new GeneSurroundingSensorChannelTerrainRockFovCov());
		}
		Defaultify();
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	// The type of sensor that is coosen to operate at each channel
	// The values should match the ones int the SurroundingSensor type dropdown AND SurroundingSensorChannelSensorTypeEnum
	private SurroundingSensorChannelSensorTypeEnum[] sensorTypeAtChannel = new SurroundingSensorChannelSensorTypeEnum[7]; // index: 0 = UNUSED!!! , 1 = Channel, .... | value 0 = creature cell covCov > , value 1 = terrain rock coverage >
	public SurroundingSensorChannelSensorTypeEnum SensorTypeAtChannel(int channel) { // index: 0 is ditched,  1 = output at A, ....
		return sensorTypeAtChannel[channel];
	}
	public void SetSensorTypeAtChannel(int channel, SurroundingSensorChannelSensorTypeEnum sensor) { // a = 1
		sensorTypeAtChannel[channel] = sensor;
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// All the geneSensors at each of the 6 channel
	private Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>[] channelDictionaryAtChannel = new Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>[7];
	public GeneSurroundingSensorChannel GetGeneSensorChannel(int channel, SurroundingSensorChannelSensorTypeEnum type) {
		return channelDictionaryAtChannel[channel][type];
	}

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
		for (int c = 1; c < 7; c++) {
			GetGeneSensorChannel(c, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Defaultify();
			GetGeneSensorChannel(c, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Defaultify();
		}
		SetSensorTypeAtChannel(1, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		SetSensorTypeAtChannel(2, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		SetSensorTypeAtChannel(3, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		SetSensorTypeAtChannel(4, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);
		SetSensorTypeAtChannel(5, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);
		SetSensorTypeAtChannel(6, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);

		direction = 0f;
		fieldOfView = 90f;
		rangeFar = 8f;
		rangeNear = 0.6f;

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Randomize() {
		for (int channel = 1; channel < 7; channel++) {
			GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Randomize();
			GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Randomize();
		}
		// randomize eye properties
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Mutate(float strength) {
		for (int channel = 1; channel < 7; channel++) {
			GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Mutate(strength);
			GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Mutate(strength);
		}
		// mutate eye properties
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// Save
	private GeneSurroundingSensorData data = new GeneSurroundingSensorData();
	public GeneSurroundingSensorData UpdateData() {
		for (int channel = 1; channel < 7; channel++) {
			data.sensorTypeAtChannel[channel] = SensorTypeAtChannel(channel);
		}

		for (int channel = 1; channel < 7; channel++) {
			data.creatureCellFovCovDataAtChannel[channel] = ((GeneSurroundingSensorChannelCreatureCellFovCov)GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).UpdateData();
			data.terrainRockFovCovDataAtChannel[channel] = ((GeneSurroundingSensorChannelTerrainRockFovCov)GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).UpdateData();
		}

		data.direction = direction;
		data.fieldOfView = fieldOfView;
		data.rangeFar = rangeFar;
		data.rangeNear = rangeNear;
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorData data) {
		for (int channel = 1; channel < 7; channel++) {
			SetSensorTypeAtChannel(channel, data.sensorTypeAtChannel[channel]);
		}

		for (int channel = 1; channel < 7; channel++) {
			((GeneSurroundingSensorChannelCreatureCellFovCov)GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).ApplyData(data.creatureCellFovCovDataAtChannel[channel]);
			((GeneSurroundingSensorChannelTerrainRockFovCov)GetGeneSensorChannel(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).ApplyData(data.terrainRockFovCovDataAtChannel[channel]);
		}

		direction = data.direction;
		fieldOfView = data.fieldOfView;
		rangeFar = data.rangeFar;
		rangeNear = data.rangeNear;
	}
}
