﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoSingleton<HUD> {

    public bool isEdgesEnabled = false;
    public int timeControllValue = 1;

    //Global
    public void OnTimeControllChanged(float value) {
        timeControllValue = (int)value;
    }

    public void OnClickToggleEdges(bool isEnabled) {
        isEdgesEnabled = isEnabled;
    }
}