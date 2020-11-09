using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ViewXput {
	public XputEnum xputType;
	public SignalUnitEnum signalUnitEnum;
	public int index;

	public ViewXput(XputEnum xputType, SignalUnitEnum signalUnitEnum, int index) {
		this.xputType = xputType;
		this.signalUnitEnum = signalUnitEnum;
		this.index = index;
	}
}
