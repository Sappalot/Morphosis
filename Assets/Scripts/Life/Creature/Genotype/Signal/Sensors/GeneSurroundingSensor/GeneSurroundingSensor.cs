using System.Collections.Generic;
using UnityEngine;

public class GeneSurroundingSensor : GeneSignalUnit {
	public GeneSurroundingSensor(SignalUnitEnum signalUnit, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		this.signalUnit = signalUnit;

		// create all the sensor
		for( int c = 0; c < 6; c++) {
			channelDictionaryAtChannel[c] = new Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>();
			channelDictionaryAtChannel[c].Add(SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov, new GeneSurroundingSensorChannelCreatureCellFovCov());
			channelDictionaryAtChannel[c].Add(SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov, new GeneSurroundingSensorChannelTerrainRockFovCov());
		}
		Defaultify();
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	// The type of sensor that is coosen to operate at each channel
	// The values should match the ones int the SurroundingSensor type dropdown AND SurroundingSensorChannelSensorTypeEnum
	private SurroundingSensorChannelSensorTypeEnum[] sensorTypeAtChannel = new SurroundingSensorChannelSensorTypeEnum[6]; // index: 0 = A
	public SurroundingSensorChannelSensorTypeEnum OperatingSensorAtChannel(int channel) { // index: 0 is ditched,  1 = output at A, ....
		return sensorTypeAtChannel[channel];
	}
	public void SetSensorTypeAtChannel(int channel, SurroundingSensorChannelSensorTypeEnum sensor) { // a = 0
		sensorTypeAtChannel[channel] = sensor;
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// All the geneSensors at each of the 6 channel
	private Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>[] channelDictionaryAtChannel = new Dictionary<SurroundingSensorChannelSensorTypeEnum, GeneSurroundingSensorChannel>[6];
	public GeneSurroundingSensorChannel GeneSensorAtChannelByType(int channel, SurroundingSensorChannelSensorTypeEnum type) {
		return channelDictionaryAtChannel[channel][type];
	}

	private float m_directionLocal = 0;
	// In which angle the eye is looking, 0 same as cells heading (in the direction of the black-white arrow)
	// possitive number is the angle from the heading towards the black side, negative is the white side
	// If cell is flipped (black | white) and cells heding is north (90). directio = 90 ==> looking left (black side). direction -90 ==> looking right (white side)
	// If cell is flipped (white | black) and cells heding is north (90). directio = 90 ==> looking right (black side). direction -90 ==> looking left (white side)
	// range [-180, 180] degrees
	public float directionLocal {
		get {
			return m_directionLocal;
		}
		set {
			m_directionLocal = value;
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
		for (int c = 0; c < 6; c++) {
			GeneSensorAtChannelByType(c, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Defaultify();
			GeneSensorAtChannelByType(c, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Defaultify();
		}
		SetSensorTypeAtChannel(0, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov); // A
		SetSensorTypeAtChannel(1, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);
		SetSensorTypeAtChannel(2, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		SetSensorTypeAtChannel(3, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);
		SetSensorTypeAtChannel(4, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
		SetSensorTypeAtChannel(5, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov); // F

		directionLocal = 0f;
		fieldOfView = 90f;
		rangeFar = 8f;
		rangeNear = 0.6f;

		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Randomize() {
		for (int channel = 0; channel < 6; channel++) {
			GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Randomize();
			GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Randomize();
		}
		// randomize eye properties
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	public void Mutate(float strength) {
		for (int channel = 0; channel < 6; channel++) {
			GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov).Mutate(strength);
			GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov).Mutate(strength);
		}
		// mutate eye properties
		genotypeDirtyfy.ReforgeCellPatternAndForward();
	}

	// Save
	private GeneSurroundingSensorData data = new GeneSurroundingSensorData();
	public GeneSurroundingSensorData UpdateData() {
		for (int channel = 0; channel < 6; channel++) {
			data.sensorTypeAtChannel[channel] = OperatingSensorAtChannel(channel);
		}

		for (int channel = 0; channel < 6; channel++) {
			data.creatureCellFovCovDataAtChannel[channel] = ((GeneSurroundingSensorChannelCreatureCellFovCov)GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).UpdateData();
			data.terrainRockFovCovDataAtChannel[channel] = ((GeneSurroundingSensorChannelTerrainRockFovCov)GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).UpdateData();
		}

		data.directionLocal = directionLocal;
		data.fieldOfView = fieldOfView;
		data.rangeFar = rangeFar;
		data.rangeNear = rangeNear;
		return data;
	}

	// Load
	public void ApplyData(GeneSurroundingSensorData data) {
		for (int channel = 0; channel < 6; channel++) {
			SetSensorTypeAtChannel(channel, data.sensorTypeAtChannel[channel]);
		}

		for (int channel = 0; channel < 6; channel++) {
			GeneSurroundingSensorChannelCreatureCellFovCovData creatureCellData = data.creatureCellFovCovDataAtChannel[channel];
			// They might be missing in old freezer
			if (creatureCellData == null) {
				creatureCellData = new GeneSurroundingSensorChannelCreatureCellFovCovData();
			}
			((GeneSurroundingSensorChannelCreatureCellFovCov)GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov)).ApplyData(creatureCellData);

			GeneSurroundingSensorChannelTerrainRockFovCovData terrainRockData = data.terrainRockFovCovDataAtChannel[channel];
			if (terrainRockData == null) {
				terrainRockData = new GeneSurroundingSensorChannelTerrainRockFovCovData();
			}
			((GeneSurroundingSensorChannelTerrainRockFovCov)GeneSensorAtChannelByType(channel, SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov)).ApplyData(terrainRockData);
		}

		directionLocal = data.directionLocal;
		fieldOfView = data.fieldOfView;
		rangeFar = data.rangeFar;
		rangeNear = data.rangeNear;
	}
}
