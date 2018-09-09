using UnityEngine;
using UnityEngine.UI;

public abstract class MetabolismCellPanel : MonoBehaviour {
	public PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	protected bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}
}