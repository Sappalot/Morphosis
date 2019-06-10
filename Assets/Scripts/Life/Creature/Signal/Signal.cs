using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour {

	// Sensor
	public EffectSensor effectSensor;

	[HideInInspector]
	public SensorTypeEnum sensorType { get; private set; }

	private Sensor m_sensor;
	[HideInInspector]
	public Sensor sensor {
		get {
			return m_sensor;
		}
		set {
			m_sensor = value;
			sensorType = m_sensor.GetSensorType();
		}
	}

	// ^ Sensor ^

	public void Init(Cell cell) {
		sensor = effectSensor;
		sensorType = SensorTypeEnum.Effect;

		sensor.Init(cell);
	}

}
