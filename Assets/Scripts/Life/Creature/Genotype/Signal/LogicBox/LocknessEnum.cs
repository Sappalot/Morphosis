using System;

[Serializable]
public enum LocknessEnum {
	Unlocked,
	SemiLocked, // valve is unlocked, input reference is locked
	Locked,
}