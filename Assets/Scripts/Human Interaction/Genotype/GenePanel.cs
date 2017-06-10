using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenePanel : MonoSingleton<GenePanel> {

    public ArangementPanel primary;

    private void Update() {
        List<Creature> selection = CreatureSelectionPanel.instance.selection;
        primary.arangement = selection.Count == 1 ? selection[0].genotype.genome[0].arrangements[0] : null;
    }

    public void UpdateRepresentation() {
        primary.UpdateRepresentation();
    }
}
