public class IdGenerator {
	public long worldNumber = 0;
	public long freezerNumber = 0;

	public string GetUniqueId() {
		return "id" + worldNumber++;
	}

	public string GetUniqueWorldId() {
		return "w" + worldNumber++;
	}

	public string GetUniqueFreezerId() {
		return "f" + freezerNumber++;
	}
}