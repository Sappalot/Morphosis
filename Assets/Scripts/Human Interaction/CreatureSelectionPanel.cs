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
        for (int index = 0; index < selection.Count; index++) {
            selection[index].SetHighlite(false);
        }
        selection.Clear();
        UpdateGUI();

        UpdateGenotypePanel();
    }

    public void SelectOnly(Creature creature) {
        for (int index = 0; index < selection.Count; index++) {
            selection[index].SetHighlite(false);
        }
        selection.Clear();

        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateGUI();

        UpdateGenotypePanel();
    }

    public void AddToSelection(Creature creature) {
        creature.SetHighlite(true);
        selection.Add(creature);
        UpdateGUI();

        UpdateGenotypePanel();
    }

    public void RemoveFromSelection(Creature creature) {
        creature.SetHighlite(false);
        selection.Remove(creature);
        UpdateGUI();

        UpdateGenotypePanel();
    }

    private void UpdateGenotypePanel() {
        if (selection.Count == 1) {
            GenotypePanel.instance.genotype = selection[0].genotype;
        } else {
            GenotypePanel.instance.genotype = null;
        }
    }

    private void UpdateGUI() {
        if (selection.Count == 0) {
            //gameObject.SetActive(false);
            selectedCreatureText.text = "";
            //phenotypePanel.gameObject.SetActive(false);
        } else if (selection.Count == 1) {
            //gameObject.SetActive(true);
            selectedCreatureText.text = selection[0].nickname;
            //phenotypePanel.gameObject.SetActive(true);
        } else {
            //gameObject.SetActive(true);
            selectedCreatureText.text = selection.Count + " Creatures";
            //phenotypePanel.gameObject.SetActive(false);
        }            
    }

    //Buttons
    public void OnClickDelete() {
        for (int index = 0; index < selection.Count; index++) {
            life.DeleteCreature(selection[index]);
        }
        ClearSelection();
    }
}
