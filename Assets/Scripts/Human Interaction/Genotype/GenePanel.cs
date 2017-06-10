using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenePanel : MonoSingleton<GenePanel> {

    public ArangementPanel primary;

    public void UpdateRepresentation() {

        //hack select
        List<Creature> selection = CreatureSelectionPanel.instance.selection;
        primary.arrangement = selection.Count == 1 ? selection[0].genotype.genome[0].arrangements[0] : null;

        primary.UpdateRepresentation();
    }
}
