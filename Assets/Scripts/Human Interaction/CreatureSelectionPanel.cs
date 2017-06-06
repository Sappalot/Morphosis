using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelectionPanel : MonoSingleton<CreatureSelectionPanel> {

    public Life life;
    public PhenotypePanel phenotypePanel;

    public Text selectedCreatureText;

    public List<Creature> selection { get; private set; }

    public bool IsSelected(Creature creature) {
        return selection.Contains(creature);
    }

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
        UpdateGUI();
    }

    public void SelectOnly(Creature creature) {
        foreach (Creature c in selection) {
            c.SetHighlite(false);
        }
        selection.Clear();

        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateGUI();
    }

    public void AddToSelection(Creature creature) {
        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateGUI();
    }

    public void RemoveFromSelection(Creature creature) {
        creature.SetHighlite(false);
        selection.Remove(creature);
        UpdateGUI();
    }

    private void UpdateGUI() {
        if (selection.Count == 0) {
            gameObject.SetActive(false);
            selectedCreatureText.text = "";
            phenotypePanel.gameObject.SetActive(false);
        } else if (selection.Count == 1) {
            gameObject.SetActive(true);
            selectedCreatureText.text = selection[0].nickname;
            phenotypePanel.gameObject.SetActive(true);
        } else {
            gameObject.SetActive(true);
            selectedCreatureText.text = selection.Count + " Creatures";
            phenotypePanel.gameObject.SetActive(false);
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
