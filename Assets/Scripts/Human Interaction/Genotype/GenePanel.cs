using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenePanel : MonoBehaviour {

    public ArangementPanel primary;

    private void Update() {
        List<Creature> selection = CreatureSelectionPanel.instance.selection;
        primary.arangement = selection.Count == 1 ? selection[0].genotype.genome[0].primary : null;
    }
}
