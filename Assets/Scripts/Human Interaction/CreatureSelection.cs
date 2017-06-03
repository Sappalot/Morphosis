using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelection : MonoSingleton<CreatureSelection> {

    public Life life;
    public Text selectedCreatureText;

    public List<Creature> selection { get; private set; }

    public override void Init() {
        base.Init();
        selection = new List<Creature>();

        ClearSelection();
    }

    public void ClearSelection() {
        foreach (Creature c in selection) {
            c.SetHighlite(false);
        } 
        selection.Clear();
        UpdateselectedCreatureText();
    }

    public void SelectOnly(Creature creature) {
        foreach (Creature c in selection) {
            c.SetHighlite(false);
        }
        selection.Clear();

        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateselectedCreatureText();
    }

    public void AddToSelection(Creature creature) {
        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateselectedCreatureText();
    }

    public void RemoveFromSelection(Creature creature) {
        creature.SetHighlite(false);
        selection.Remove(creature);
        UpdateselectedCreatureText();
    }

    private void UpdateselectedCreatureText() {
        if (selection.Count == 0) {
            selectedCreatureText.text = ""; 
        } else if (selection.Count == 1) {
            selectedCreatureText.text = selection[0].nickname;
        } else {
            selectedCreatureText.text = selection.Count + " Creatures";
        }
            
    }

    //Buttons
    public void OnClickDelete() {
        foreach (Creature c in selection) {
            life.DeleteCreature(c);
        }
        ClearSelection();
    }


}
