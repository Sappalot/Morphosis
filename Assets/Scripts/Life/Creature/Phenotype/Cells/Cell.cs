using UnityEngine;
using System.Collections.Generic;

public abstract class Cell : MonoBehaviour {
    public Gene gene;
    public Vector2i mapPosition = new Vector2i();
    public int buildOrderIndex = 0;

    //  The direction the cell is facing in creature space
    public int heading = 0;

    public string id;
    public int groups = 0;
    public float radius = 0.5f;

    public SpringJoint2D northSpring;
    public SpringJoint2D southEastSpring;
    public SpringJoint2D southWestSpring;
    public float springFrequenzy = 5f;

    public float timeOffset;

    public Creature creature;

    public Vector3 velocity {
        get { return this.GetComponent<Rigidbody2D>().velocity; }
        private set { }
    }

    [HideInInspector]
    public CellNeighbour northEastNeighbour =   new CellNeighbour(0);
    [HideInInspector]
    public CellNeighbour northNeighbour =       new CellNeighbour(1);
    [HideInInspector]
    public CellNeighbour northWestNeighbour =   new CellNeighbour(2);
    [HideInInspector]
    public CellNeighbour southWestNeighbour =   new CellNeighbour(3);
    [HideInInspector]
    public CellNeighbour southNeighbour =       new CellNeighbour(4);
    [HideInInspector]
    public CellNeighbour southEastNeighbour =   new CellNeighbour(5);
   
    private Dictionary<int, CellNeighbour> index_Neighbour = new Dictionary<int, CellNeighbour>();
    private List<SpringJoint2D> springList = new List<SpringJoint2D>();
    private Transform spriteTransform;

    public Cell( ) {
        index_Neighbour.Add(0, northEastNeighbour);
        index_Neighbour.Add(1, northNeighbour);
        index_Neighbour.Add(2, northWestNeighbour);
        index_Neighbour.Add(3, southWestNeighbour);
        index_Neighbour.Add(4, southNeighbour);
        index_Neighbour.Add(5, southEastNeighbour);

        springList.Add( northSpring );
        springList.Add( southEastSpring );
        springList.Add( southWestSpring );
    }

    public void EvoUpdate() {
        //spriteTransform.localRotation = Quaternion.Euler(0f, 0f, CardinalDirectionHelper.ToAngle(heading));
    }

    public void EvoFixedUpdate(float fixedTime) {
        UpdateRotation();
        TurnNeighboursInPlace();
        UpdateRadius(fixedTime);
        UpdateSpringLengths(); // It is costy to update spring length
    }



    // Use this for initialization
    private void Awake() {
        spriteTransform = transform.Find("Sprite");
    }

    public int GetDirectionOfNeighbourCell(Cell cell) {
        for (int index = 0; index < 6; index++) {
            if (HasNeighbourCell(index) && GetNeighbour(index).cell == cell) {
                return index;
            }
        }
        return -1;
    }

    virtual public void UpdateRadius(float fixedTime) { }

    virtual public void UpdateSpringLengths() { }

    virtual public void UpdateSpringFrequenzy() { }

    public void TurnNeighboursInPlace() {
        //TODO Update turn springs only when nessesary 
        if (groups < 2) {
            return;
        }

        UpdateNeighbourVectors();
        //CellNeighbour firstNeighbour = null;
        int springs = 0;
        Vector3 responceForce = new Vector3();
        for (int index = 0; index < 12; index++) {
            if(!HasNeighbourCell(index)) {
                continue;
            }
            if (!HasNeighbourCell(index + 1)) {
                //this(0)...empty(1)
                if (HasNeighbourCell(index + 2)) {
                    //this(0)...empty(1)...full(2)
                    responceForce += ApplyTorqueToPair(index, index+2);
                    springs++;
                    //if (springs >= groups - 1) {
                    if (springs >= groups)
                    {
                        break;
                    }
                    //jump to where spring was attached
                    index++; //+1
                    continue;
                }
                else {
                    //this(0)...empty(1)...empty(2)
                    if (HasNeighbourCell(index + 3)) {
                        //this(0)...empty(1)...empty(2)...full(3)
                        responceForce += ApplyTorqueToPair(index, index + 3);
                        springs++;
                        //if (springs >= groups - 1) {
                        if (springs >= groups)
                        {
                            break;
                        }
                        
                        //jump to where spring was attached
                        index += 2; //+1
                        continue;
                    }
                    else {
                        //this(0)...empty(1)...empty(2)...empty(3)
                        if (HasNeighbourCell(index + 4))
                        {
                            //this(0)...empty(1)...empty(2)...empty(3)...full(4)
                            responceForce += ApplyTorqueToPair(index, index + 4);
                            springs++;
                            //if (springs >= groups - 1) {
                            if (springs >= groups)
                            {
                                break;
                            }

                            //jump to where spring was attached
                            index += 3; //+1
                            continue;
                        }
                        else
                        {
                            //this(0)...empty(1)...empty(2)...empty(3)...empty(4)
                            break; //if there is a "this + 4" aka "this-1" it must be connected to "this". if not then "this" is the only neibour and we would not be here in the first place
                        }
                    }
                } 
            }
        }
        
        GetComponent<Rigidbody2D>().AddForce(responceForce, ForceMode2D.Impulse);
    }

    //Applies forces to neighbour and returns reaction force to this
    Vector3 ApplyTorqueToPair(int alphaIndex, int betaIndex) {
        CellNeighbour alphaNeighbour = GetNeighbour(alphaIndex);
        CellNeighbour betaNeighbour = GetNeighbour(betaIndex);

        Vector3 alphaVector = alphaNeighbour.coreToThis; //Allways after = lower angle
        Vector3 betaVector = betaNeighbour.coreToThis; //Allways ahead = lower angle
        //float angle = Vector3.Angle(alphaVector, betaVector);
        float angle = LongAngle(alphaVector, betaVector);

        float goalAngle = CardinalDirectionHelper.GetAngleBetween(alphaIndex, betaIndex);
        //Debug.Log(this.id + " Spring: " + alphaNeighbour.cell.id + "-" + betaNeighbour.cell.id + " = GoalA: " + goalAngle + " A: " + angle);
        float diff = (goalAngle - angle);


        float k1 = 0.00005f; //Works with RB mass 0.08
        float k2 = 0.00003f;
        //float k1 = 0.0005f;
        //float k2 = 0.0003f;
        float magnitude = k1 * diff + Mathf.Sign(diff) * k2 * diff * diff;
        Vector3 alphaForce = Vector3.Cross(alphaVector, new Vector3(0f, 0f, 1f)) * magnitude;
        Vector3 betaForce =  Vector3.Cross(betaVector, new Vector3(0f, 0f, -1f)) * magnitude;

        alphaNeighbour.cell.GetComponent<Rigidbody2D>().AddForce(alphaForce, ForceMode2D.Impulse);
        betaNeighbour.cell.GetComponent<Rigidbody2D>().AddForce(betaForce, ForceMode2D.Impulse);

        return -(alphaForce + betaForce);

    }

    float LongAngle(Vector3 after, Vector3 ahead) {
        float angle = Vector3.Angle(after, ahead);
        if (Vector3.Cross(after, ahead).z < 0) {
            angle = 360 - angle;
        }
        return angle;
    }

    public SpringJoint2D GetSpring(Cell askingCell) {
        if (HasNeighbour(CardinalDirection.north) && askingCell == northNeighbour.cell) {
            return northSpring;
        }
        else if (HasNeighbour(CardinalDirection.southEast) && askingCell == southEastNeighbour.cell) {
            return southEastSpring;
        }
        else if (HasNeighbour(CardinalDirection.southWest) && askingCell == southWestNeighbour.cell) {
            return southWestSpring;
        }
        return null;
    }

    public void SetNeighbourCell(CardinalDirection direction, Cell cell) {
        index_Neighbour[CardinalDirectionHelper.ToIndex(direction)].cell = cell;
    }

    public Cell GetNeighbourCell(CardinalDirection direction) {
        return GetNeighbourCell(CardinalDirectionHelper.ToIndex(direction));
    }

    public Cell GetNeighbourCell(int index) {
        return index_Neighbour[index%6].cell;
    }

    public CellNeighbour GetNeighbour(CardinalDirection direction) {
        return GetNeighbour(CardinalDirectionHelper.ToIndex(direction));
    }

    public CellNeighbour GetNeighbour(int index) {
        return index_Neighbour[index%6];
    }

    //  World space position
    public Vector3 GetPosition() {
        return transform.position;
    }

    public float GetRotation() {
        return transform.rotation.z;
    }

    public int GetNeighbourCount() {
        int count = 0;
        for (int index = 0; index < 6; index++) {
            if (HasNeighbourCell(index))
                count++;
        }
        return count;
    }

    public bool HasNeighbour(CardinalDirection direction) {
        return HasNeighbourCell(CardinalDirectionHelper.ToIndex(direction));
    }

    public bool HasNeighbourCell(int index) {
        return index_Neighbour[index % 6].cell != null;
    }

    ////  Updates world space rotation (heading) derived from neighbour position and localRotation
    public void UpdateRotation() {

        UpdateNeighbourVectors();
        UpdateNeighbourAngles();

        float angleSum = 0; // CardinalDirectionIndex.ToAngle(heading);

        for (int index = 0; index < 6; index++) {
            if (HasNeighbourCell(index)) {
                angleSum = index_Neighbour[index].angle;
                break;
            }
        }
        if (GetNeighbourCount() > 0) {
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, angleSum);
        }
        else {
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }
    }

    void UpdateNeighbourAngles() {
        for (int index = 0; index < 6; index++) {
            if (HasNeighbourCell(index)) {
                GetNeighbour(index).angle = FindAngle(GetNeighbour(index).coreToThis);
            }
        }
    }


    void UpdateNeighbourVectors() {
        for (int index = 0; index < 6; index++)
        {
            if (HasNeighbourCell(index))
            {
                GetNeighbour(index).coreToThis = GetNeighbourCell(index).GetPosition() - transform.position;
            }
        }
    }

    public void UpdateSpringConnections() {
        if (northNeighbour.cell) {
            northSpring.connectedBody = northNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
            northSpring.enabled = true;
        }
        else {
            northSpring.connectedBody = null;
            northSpring.enabled = false;
        }

        if (southWestNeighbour.cell) {
            southWestSpring.connectedBody = southWestNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
            southWestSpring.enabled = true;
        }
        else {
            southWestSpring.connectedBody = null;
            southWestSpring.enabled = false;
        }

        if (southEastNeighbour.cell) {
            southEastSpring.connectedBody = southEastNeighbour.cell.gameObject.GetComponent<Rigidbody2D>();
            southEastSpring.enabled = true;
        }
        else {
            southEastSpring.connectedBody = null;
            southEastSpring.enabled = false;
        }

    }

    public void UpdateGroups() {
        //TODO check if this cell is a hinge
        int groups = 0;
        bool lastWasNeighbor = false;
        bool neighbourFound = false;
        for (int index = 0; index < 7; index++) {
            if (HasNeighbourCell(index)) {
                neighbourFound = true;
                lastWasNeighbor = true;
            } else {
                if (lastWasNeighbor) { //down flank detected
                    groups++;
                }
                lastWasNeighbor = false;
            }
        }
        if (neighbourFound && groups == 0) {
            groups = 1;
        }
        this.groups = groups;
    }

    public static float FindAngle(Vector2 direction) {
        float angle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(direction.y) / Mathf.Abs(direction.x));

        if (direction.x > 0f) {
            if (direction.y < 0f)
                return 360f - angle;
            return angle;
        }
        if (direction.y > 0f)
            return 180f - angle;
        return 180f + angle;

    }

    private void OnMouseDown() {
        if (Input.GetKey("mouse 0")) {
            Debug.Log("click on Creature: " + creature.nickname);
        }
    }
}