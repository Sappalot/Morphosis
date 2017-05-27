public class IdGenerator {
    private long number = 0;

    public string GetUniqueId() {
        return "id" + number++;
    }
}

