using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Do we need this interface????????
public interface IOutputPanel {

	bool IsUsedExternally(); // if yes ther is somebodey listening to this geneCell or other geneCell containing same gene
}
