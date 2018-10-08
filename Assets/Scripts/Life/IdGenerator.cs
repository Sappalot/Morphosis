public class IdGenerator {
	public long serialNumber = 0;
	private string prefix = ""; // todo prefix should not really be needed since creatures are allready separated between world and freezer

	public IdGenerator(string prefix) {
		this.prefix = prefix;
	}

	public string GetUniqueWorldId() {
		return prefix + serialNumber++;
	}
}