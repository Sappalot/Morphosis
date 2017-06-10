public class Arrangement {
    public bool isEnabled = true;
    public ArrangementTypeEnum type = ArrangementTypeEnum.Side;

    public int referenceCount = 1; //ALL: negative value indicated cells on the white side (when arrangementtype = Side)
    public int angle; //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
    public int gap = 0; //MIRROR: number of cells in the gap
    public FlipTypeEnum flipType = FlipTypeEnum.Same; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 
    public Gene referenceGene;
}
