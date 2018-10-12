using System.Collections.Generic;

public class IdGenerator {
	public long serialNumber = 0;

	public string GetUniqueId() {
		long proposedSerialNumber = serialNumber + 1;
		//while (World.instance.life.IsUsingId(GetId(proposedSerialNumber)) || Freezer.instance.HasCreature(GetId(proposedSerialNumber)) ) {
		//	proposedSerialNumber++;
		//}
		serialNumber = proposedSerialNumber;
		return GetId(serialNumber);
	}

	public string GetId(long serialNumber) {
		return "id" + serialNumber;
	}

	//Note: this is OK for creatures which are not known by anybodey else
	public void RenameToUniqueIds(List<Creature> creatures) {
		for (int freezerIndex = 0; freezerIndex < creatures.Count; freezerIndex++) {
			creatures[freezerIndex].id = GetUniqueId();
		}
	}

	public void Restart() {
		serialNumber = 0;
	}
}