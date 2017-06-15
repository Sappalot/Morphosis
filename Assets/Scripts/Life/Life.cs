using UnityEngine;
using System.Collections.Generic;

public class Life : MonoBehaviour {

    public Creature creaturePrefab;

    private IdGenerator idGenerator = new IdGenerator();
    private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
    private List<Creature> creatureList = new List<Creature>();

    public void EvoUpdate() {
        for (int index = 0; index < creatureList.Count; index++) {
            creatureList[index].EvoUpdate();
        }
    }

    public void EvoFixedUpdate(float fixedTime) {
        for (int index = 0; index < creatureList.Count; index++) {
            creatureList[index].EvoFixedUpdate(fixedTime);
        }
    }

    int test = 0;
    //makes a minimal new creature with blank genotype
    public string SpawnCreatureEmbryo(Vector3 position) {
        // TODO creatre a copy

        Creature creature = (GameObject.Instantiate(creaturePrefab, position, Quaternion.identity) as Creature);
        string id = idGenerator.GetUniqueId();
        if (creatureDictionary.ContainsKey(id)) {
            throw new System.Exception("Generated ID was not unique.");
        }
        creature.id = id;
        creature.nickname = "Nick " + id;
        creature.transform.parent = this.transform;
        creature.transform.position = position;
        creatureDictionary.Add(id, creature);
        creatureList.Add(creature);

        creature.Generate();

        if (test == 0) {
            CreatureSelectionPanel.instance.SelectOnly(creature);
        }
        test++;
        return id;
    }

    public string SpawnCreatureClone(Vector3 position, float rotation, Creature original) {
        //genotype = same
        //phenotype = same everything, except velocities

        return "not done";
    }

    public string SpawnCreatureCrossbreed(Vector3 position, float rotation, Creature[] originals) {
        //genome = mix
        //phenotype = fully grown;

        return "not done";
    }

    public void DeleteCreature(Creature creature) {
        Destroy(creature.gameObject);

        creatureDictionary.Remove(creature.id);
        creatureList.Remove(creature);
    }
}
