using UnityEngine;

public class Gene {
	public CellTypeEnum m_type = CellTypeEnum.Leaf; // + this vein cell's settings 
	public CellTypeEnum type {
		get {
			if (index == 0 && m_type == CellTypeEnum.Egg) {
				return CellTypeEnum.Vein;
			}
			return m_type;
		}
		set {
			if (index == 0 && value == CellTypeEnum.Egg) {
				m_type = CellTypeEnum.Vein;
			} else {
				m_type = value;
			}
			
		}
	}
	public int index;

	public readonly Arrangement[] arrangements = new Arrangement[3];

	public Gene(int index) {
		this.index = index;

		arrangements[0] = new Arrangement();
		arrangements[1] = new Arrangement();
		arrangements[2] = new Arrangement();
	}

	public void SetReferenceGeneFromReferenceGeneIndex(Gene[] genes) {
		arrangements[0].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[1].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[2].SetReferenceGeneFromReferenceGeneIndex(genes);
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float mut = Random.Range(0, gs.cellTypeLeave + gs.cellTypeRandom * strength);
		if (mut < gs.cellTypeRandom * strength) {
			type = (CellTypeEnum)Random.Range(0, 8);
		}
		arrangements[0].Mutate(strength);
		arrangements[1].Mutate(strength);
		arrangements[2].Mutate(strength);
	}

	public void Scramble() {
		type = (CellTypeEnum)Random.Range(0, 8);
		arrangements[0].Scramble();
		arrangements[1].Scramble();
		arrangements[2].Scramble();
	}

	public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum flipSide) {
		GeneReference first = null;
		for (int index = 0; index < arrangements.Length; index++) {
			GeneReference found = arrangements[index].GetFlippableReference(referenceCardinalIndex, flipSide);
			if (found != null && arrangements[index].isEnabled) {
				first = found;
				break;
			}
		}
		return first;
	}

	public void SetDefault(Gene[] genome) {
		arrangements[0].referenceGene = genome[1];
		arrangements[1].referenceGene = genome[1];
		arrangements[2].referenceGene = genome[1];

		arrangements[0].isEnabled = false;
		arrangements[1].isEnabled = false;
		arrangements[2].isEnabled = false;
	}

	public Gene GetClone() {
		Gene clone = new Gene(index);
		clone.ApplyData(UpdateData());
		return clone;
	}

	private GeneData geneData = new GeneData();
	public GeneData UpdateData() {
		geneData.type = type;
		geneData.index = index;

		geneData.arrangementData[0] = arrangements[0].UpdateData();
		geneData.arrangementData[1] = arrangements[1].UpdateData();
		geneData.arrangementData[2] = arrangements[2].UpdateData();

		return geneData;
	}

	public void ApplyData(GeneData geneData) {
		type = geneData.type;
		index = geneData.index;

		arrangements[0].ApplyData(geneData.arrangementData[0]);
		arrangements[1].ApplyData(geneData.arrangementData[1]);
		arrangements[2].ApplyData(geneData.arrangementData[2]);
	}
}

