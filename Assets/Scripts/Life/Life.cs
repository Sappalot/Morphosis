using UnityEngine;
using System.Collections.Generic;

public class Life : MonoBehaviour {

    public Creature creaturePrefab;

    private IdGenerator idGenerator = new IdGenerator();
    private Dictionary<string, Creature> creatureDictionary = new Dictionary<string, Creature>();
    private List<Creature> creatureList = new List<Creature>();

    public void EvoUpdate() {
        foreach (Creature creature in creatureList) {
            creature.EvoUpdate();
        }
    }

    public void EvoFixedUpdate(float fixedTime) {
        foreach (Creature creature in creatureList) {
            creature.EvoFixedUpdate(fixedTime);
        }
    }

    //makes a minimal new creature with blank genotype
    public string SpawnCreatureEmbryo(Vector3 position) {
        // TODO creatre a copy

        Creature creature = (GameObject.Instantiate(creaturePrefab, position, Quaternion.identity) as Creature);
        string id = idGenerator.GetUniqueId();
        if (creatureDictionary.ContainsKey(id)) {
            throw new System.Exception("Generated ID was not unique.");
        }
        creature.id = id;
        creature.nickname = "nick" + id;
        creature.transform.parent = this.transform;
        creature.transform.position = position;
        creatureDictionary.Add(id, creature);
        creatureList.Add(creature);

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
}
