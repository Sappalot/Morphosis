using System;
using UnityEngine;

public class Wing : MonoBehaviour{
    public GameObject mainArrow;
    public GameObject normalArrow;
    public GameObject velocityArrow;
    public GameObject forceArrow;

    public Cell frontCell;
    public Cell backCell;

    public Vector3 normal;
    public Vector3 velocity;
    public Vector3 force;

    public void Update() {
        if (frontCell != null && backCell != null) {
            //draw main
            mainArrow.GetComponent<LineRenderer>().SetPosition(1, frontCell.transform.position);
            mainArrow.GetComponent<LineRenderer>().SetPosition(0, backCell.transform.position);

            //draw normal
            Vector3 wingSegmentHalf = (frontCell.transform.position - backCell.transform.position) * 0.5f;
            Vector3 midSegment = backCell.transform.position + wingSegmentHalf;
            Vector3 normalPoint = midSegment + normal;
            normalArrow.GetComponent<LineRenderer>().SetPosition(1, normalPoint);
            normalArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);

            //draw velocity
            velocityArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + velocity);
            velocityArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);

            //draw force
            forceArrow.GetComponent<LineRenderer>().SetPosition(1, midSegment + force*100f);
            forceArrow.GetComponent<LineRenderer>().SetPosition(0, midSegment);
        }
    }

    public static float maxForce = 0f;

    //Use 2 cells to find normal, allways on wings RIGHT hand
    public void UpdateNormal() {
        Vector3 wingSegment = frontCell.transform.position - backCell.transform.position;
        normal = Vector3.Cross(wingSegment.normalized, new Vector3(0f, 0f, 1f));
    }

    //static float f1 = 0.0025f;
    //static float f2 = 0.05f;

    static float f1 = 0f;
    static float f2 = 1f;    

    //use 2 cells to find center velocity
    public void UpdateVelocity() {
        velocity = (frontCell.GetComponent<Rigidbody2D>().velocity + backCell.GetComponent<Rigidbody2D>().velocity) / 2f;
    }

    // use normal and velocity to calculate force
    public void UpdateForce(Vector3 creatureVelocity) {
        float drag = 0.15f;
        float velocityInNormalDirection = Math.Max(0f, Vector3.Dot(normal, velocity-creatureVelocity*(1f-drag)));
        //float velocityInNormalDirection = Vector3.Dot(normal, velocity - creatureVelocity * (1f - drag));
        force = -normal * (f1 * velocityInNormalDirection +  f2 * Mathf.Pow(velocityInNormalDirection, 2f) );

        //float velocityInNormalDirection = Math.Max(0f, Vector3.Dot(normal, velocity));
        //force = -normal * (f1 * velocityInNormalDirection +  f2 * Mathf.Pow(velocityInNormalDirection, 40f) );
        //maxForce = Math.Max(maxForce, force.magnitude);
    }

    //Apply current force as an impulse on cells
    public void ApplyForce() {
        frontCell.GetComponent<Rigidbody2D>().AddForce(force * 0.5f, ForceMode2D.Impulse);
        backCell.GetComponent<Rigidbody2D>().AddForce(force * 0.5f, ForceMode2D.Impulse);
    }
    
      
}

