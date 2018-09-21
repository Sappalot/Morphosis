using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PidCircle : MonoBehaviour {

	private bool m_isOn;
	public bool isOn {
		get {
			return m_isOn;
		}
		set {
			if (!m_isOn && value) {
				//was off, now turning it on


				radius = radiusMin;
				pidTicks = 0;
				fpsError = 0f;
				fpsErrorI = 0f;
				fpsErrorD = 0f;

				UpdateCircle();
			}


			m_isOn = value;
		}
	}

	public Text text;
	public SpriteRenderer spriteRenderer;
	public CircleCollider2D circleCollider;

	public float kP;
	public float kI;
	public float kD;

	public float radiusMax = 60f;
	public float radiusMin = 5f;


	public float fpsGoal = 25f;
	private float fpsError;
	private float fpsErrorI;
	private float fpsErrorD;

	private float fpsErrorOld;

	private short pidTicks;
	private float radius;

	public void Init() {
		Restart();
	}

	public void UpdatePhysics() {
		//time

		pidTicks++;
		if (pidTicks >= GlobalSettings.instance.quality.pidTickPeriod) {
			if (isOn) {

				float fps = Mathf.Clamp(GlobalPanel.instance.frameRate, 0f, 200f);
				fpsErrorOld = fpsError;

				//p
				fpsError = fpsGoal - fps; //How much should we increse fps to be on the spot

				//i
				if ((radius < radiusMax - 0.1f && fpsError > 0) || (radius > radiusMin + 0.1f && fpsError < 0)) {
					fpsErrorI += fpsError;
				}

				//d
				fpsErrorD = fpsError - fpsErrorOld;

				float sum = kP * fpsError + kI * fpsErrorI + kD * fpsErrorD;
				radius = Mathf.Lerp(radiusMin, radiusMax, sum);



				pidTicks = 0;
			} else {
				radius = radiusMin;
			}
			UpdateCircle();
		}
	}

	private void UpdateCircle() {
		//text.text = string.Format(" P: {0:0.0}, I: {1:0.0}, D: {2:0.0}, Radius : {3:0.0}", kP * fpsError, kI * fpsErrorI, kD * fpsErrorD, radius);
		text.text = isOn ? "On" : "Off";
		spriteRenderer.size = new Vector2(radius * 2f, radius * 2f);
		circleCollider.radius = radius;
	}

	public void Restart() {
		isOn = false;

		text.text = "Reset";
		radius =    radiusMin;
		pidTicks =  0;
		fpsGoal =  25f;
		fpsError =  0f;
		fpsErrorI = 0f;
		fpsErrorD = 0f;

		UpdateCircle();
		PhenotypePhysicsPanel.instance.UpdateSliderAndToggleValue(); //sets it to the fpsGoal of this class
	}

	// Load / Sava

	private PidCircleData pidCircleData = new PidCircleData();
	
	// Save
	public PidCircleData UpdateData() {
		pidCircleData.isOn =      isOn;
		pidCircleData.radius =    radius;
		pidCircleData.pidTicks =  pidTicks;
		pidCircleData.fpsGoal =   fpsGoal;
		pidCircleData.fpsError =  fpsError;
		pidCircleData.fpsErrorI = fpsErrorI;
		pidCircleData.fpsErrorD = fpsErrorD;

		return pidCircleData;
	}

	// Load
	public void ApplyData(PidCircleData pidCircleData) {
		isOn =      pidCircleData.isOn;
		radius =    pidCircleData.radius;
		pidTicks =  pidCircleData.pidTicks;
		fpsGoal =   pidCircleData.fpsGoal;
		fpsError =  pidCircleData.fpsError;
		fpsErrorI = pidCircleData.fpsErrorI;
		fpsErrorD = pidCircleData.fpsErrorD;

		UpdateCircle();
		PhenotypePhysicsPanel.instance.UpdateSliderAndToggleValue();
	}
}
