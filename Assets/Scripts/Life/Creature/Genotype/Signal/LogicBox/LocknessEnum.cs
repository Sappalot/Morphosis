using System;

[Serializable]
public enum LocknessEnum {
	Unlocked,
	SemiLocked, // Applicable on input only: valve is unlocked, nerve reference is locked
	Locked,
}