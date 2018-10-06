public class IdGenerator {
	public long number = 0;

	public string GetUniqueId() {
		return "id" + number++;
	}
}