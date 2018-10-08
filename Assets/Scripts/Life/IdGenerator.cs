public class IdGenerator {
	public long worldNumber = 0;
	public long freezerNumber = 0;

	public string GetUniqueWorldId() {
		return "id" + worldNumber++;
	}

	public string GetUniqueFreezerId() {
		return "f" + freezerNumber++;
	}
}