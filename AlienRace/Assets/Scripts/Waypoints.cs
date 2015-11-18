using UnityEngine;
using System.Collections;

public class Waypoints : MonoBehaviour
{

    public Transform[] Points;

    void OnDrawGizmos()
    {
        for (int i = 0; i < Points.Length; ++i)
        {
            Vector3 thisPosition = Points[i].position;
            Vector3 nextPosition;
            if (i == Points.Length - 1)
            {
                nextPosition = Points[0].position;
            }
            else
            {
                nextPosition = Points[i + 1].position;
            }
            Gizmos.DrawLine(thisPosition, nextPosition);
        }
    }

    public Vector2 GetNearestPoint(ref int lastPoint, Vector3 position)
    {
        Vector3 nearest = Points[LoopArray(lastPoint - 1, Points.Length)].position;
        Vector3 next = Points[(lastPoint) % Points.Length].position;
        float nearestDistSqr = (nearest - position).sqrMagnitude;
        float nextDistSqr = (next - position).sqrMagnitude;
        while (nextDistSqr < nearestDistSqr)
        {
            ++lastPoint;
            nearest = next;
            nearestDistSqr = nextDistSqr;
            next = Points[(lastPoint) % Points.Length].position;
            nextDistSqr = (next - position).sqrMagnitude;
        }
        Debug.DrawLine(nearest, next, Color.red);

        return NearestPointOnLine(new Vector2(position.x, position.z), new Vector2(nearest.x, nearest.z), new Vector2(next.x, next.z));
    }

    int LoopArray(int index, int length)
    {
        if (index < 0)
        {
            return length + index;
        }
        return index % length;
    }

    public Vector2 NearestPointOnLine(Vector2 p, Vector2 start, Vector2 end)
    {
        float l2 = Vector2.SqrMagnitude(end - start); // |end-start|^2
        if (l2 == 0.0f) return start; // edge case where start == end;
        Vector2 lineVector = end - start;
        Vector2 pointVector = p - start;
        float t = Vector2.Dot(pointVector, lineVector) / l2;
        if (t < 0.0f)
        {
            return start;
        }
        else if (t > 1.0f)
        {
            return end;
        }
        else
        {
            return start + t * lineVector;
        }
    }

    public Vector3 GetDropOffPoint(Vector3 carLoc)
    {
        float minDistanceSqr = float.PositiveInfinity;
        Vector3 dropOffPoint = Vector3.zero;
        for (int i = 0; i < Points.Length; ++i)
        {
            float distSqr = (carLoc - Points[i].transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                dropOffPoint = Points[i].transform.position;
            }
        }
        return dropOffPoint;
    }
}
