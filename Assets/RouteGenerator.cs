using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RouteGenerator : MonoBehaviour
{
    [SerializeField] private BoxCollider tank;
    [SerializeField] private Transform startTransform;
    private FishController fishController;
    private Transform[] routePoints = new Transform[12];


    private void Start()
    {
        fishController = GetComponent<FishController>();
        routePoints = new Transform[12];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRoute(startTransform.position);
        }
    }

    public void GenerateRoute(Vector3 startPos)                                             //Chiama questa
    {
        for (int i = 0; i < routePoints.Length; i++)
        {
            if (routePoints[i] == null)
            {
                routePoints[i] = new GameObject("Route node " + i).transform;
            }
        }

        //TwoRoutesGenerator(startPos);
        ThreeRoutesGenerate(startPos);

        fishController.routesToFollow[0].p1 = routePoints[0].position;
        fishController.routesToFollow[0].p2 = routePoints[1].position;
        fishController.routesToFollow[0].p3 = routePoints[2].position;
        fishController.routesToFollow[0].p4 = routePoints[3].position;
        fishController.routesToFollow[1].p1 = routePoints[4].position;
        fishController.routesToFollow[1].p2 = routePoints[5].position;
        fishController.routesToFollow[1].p3 = routePoints[6].position;
        fishController.routesToFollow[1].p4 = routePoints[7].position;
        fishController.routesToFollow[2].p1 = routePoints[8].position;
        fishController.routesToFollow[2].p2 = routePoints[9].position;
        fishController.routesToFollow[2].p3 = routePoints[10].position;
        fishController.routesToFollow[2].p4 = routePoints[11].position;


        fishController.OnRoutesGenerated();
    }

    //private void TwoRoutesGenerator(Vector3 startPos)
    //{
    //    routePoints[0].position = startPos;

    //    edgePositions = GetColliderBounds(tank);

    //    Vector3 oppositePos;
    //    float maxDistance = 0;
    //    float maxOverDistance = 0;
    //    float minOverDistance = float.PositiveInfinity;
    //    float maxUnderDistance = 0;
    //    float minUnderDistance = float.PositiveInfinity;
    //    foreach (var pos in edgePositions)
    //    {
    //        float positionDistance = Vector3.Distance(startPos, pos);
    //        if (pos.y >= startPos.y)                                            //SOPRA
    //        {
    //            if (positionDistance > maxOverDistance)
    //            {
    //                maxOverDistance = Vector3.Distance(startPos, pos);
    //                maxOverPos = pos;
    //            }
    //            if (positionDistance < minOverDistance)
    //            {
    //                minOverDistance = Vector3.Distance(startPos, pos);
    //                minOverPos = pos;
    //            }
    //        }
    //        else                                                                //SOTTO
    //        {
    //            if (positionDistance > maxUnderDistance)
    //            {
    //                maxUnderDistance = Vector3.Distance(startPos, pos);
    //                maxUnderPos = pos;
    //            }
    //            if (positionDistance < minUnderDistance)
    //            {
    //                minUnderDistance = Vector3.Distance(startPos, pos);
    //                minUnderPos = pos;
    //            }
    //        }


    //        if (positionDistance > maxDistance)
    //        {
    //            maxDistance = Vector3.Distance(startPos, pos);
    //            maxDistancePos = pos;
    //        }
    //    }
    //    oppositePos = Vector3.Lerp(startPos, maxDistancePos, 0.7f);
    //    routePoints[3].position = oppositePos;

    //    if (startPos.y <= tank.bounds.center.y)
    //    {
    //        routePoints[1].position = minUnderPos;
    //        routePoints[2].position = maxUnderPos;
    //        routePoints[4].position = maxOverPos;
    //        routePoints[5].position = minOverPos;
    //    }
    //    else
    //    {
    //        routePoints[1].position = minOverPos;
    //        routePoints[2].position = maxOverPos;
    //        routePoints[4].position = maxUnderPos;
    //        routePoints[5].position = minUnderPos;
    //    }
    //    routePoints[1].position = tank.ClosestPointOnBounds(startPos);

    //    if (routePoints[4].position.x != routePoints[5].position.x)
    //    {
    //        routePoints[4].position = new Vector3(routePoints[5].position.x, routePoints[4].position.y, routePoints[4].position.z);
    //    }
    //    else
    //    {
    //        routePoints[4].position = new Vector3(routePoints[4].position.x, routePoints[4].position.y, routePoints[5].position.z);
    //    }
    //}

    private void ThreeRoutesGenerate(Vector3 start)
    {
        Vector3[] nearEdges = new Vector3[4];
        Vector3[] farEdges = new Vector3[4];
        if (start.y < tank.bounds.center.y)
        {
            nearEdges = GetHalfColliderBounds(tank, true);
            farEdges = GetHalfColliderBounds(tank, false);
        }
        else
        {
            nearEdges = GetHalfColliderBounds(tank, false);
            farEdges = GetHalfColliderBounds(tank, true);
        }

        nearEdges = nearEdges.OrderBy((d) => (d - start).sqrMagnitude).ToArray();
        farEdges = farEdges.OrderBy((d) => (d - start).sqrMagnitude).ToArray();

        routePoints[0].position = start;
        routePoints[1].position = nearEdges[1];
        routePoints[2].position = nearEdges[3];
        routePoints[3].position = Vector3.Lerp(tank.bounds.center, nearEdges[2], 0.25f);

        routePoints[4].position = routePoints[3].position;
        routePoints[5].position = Vector3.Lerp(tank.bounds.center, farEdges[0], 0.9f);
        routePoints[6].position = Vector3.Lerp(tank.bounds.center, farEdges[3], 0.8f);
        routePoints[7].position = Vector3.Lerp(tank.bounds.center, farEdges[1], 0.25f);

        routePoints[8].position = routePoints[7].position;
        routePoints[9].position = nearEdges[0];
        routePoints[10].position = Vector3.Lerp(start, farEdges[2], 0.8f);
        routePoints[11].position = start;

    }

    private Vector3[] GetColliderBounds(BoxCollider b)
    {
        Vector3[] bounds = new Vector3[8];
        bounds[0] = b.bounds.center + new Vector3(b.bounds.extents.x, b.bounds.extents.y, b.bounds.extents.z);
        bounds[1] = b.bounds.center + new Vector3(b.bounds.extents.x, b.bounds.extents.y, -b.bounds.extents.z);
        bounds[2] = b.bounds.center + new Vector3(b.bounds.extents.x, -b.bounds.extents.y, b.bounds.extents.z);
        bounds[3] = b.bounds.center + new Vector3(b.bounds.extents.x, -b.bounds.extents.y, -b.bounds.extents.z);
        bounds[4] = b.bounds.center + new Vector3(-b.bounds.extents.x, b.bounds.extents.y, b.bounds.extents.z);
        bounds[5] = b.bounds.center + new Vector3(-b.bounds.extents.x, b.bounds.extents.y, -b.bounds.extents.z);
        bounds[6] = b.bounds.center + new Vector3(-b.bounds.extents.x, -b.bounds.extents.y, b.bounds.extents.z);
        bounds[7] = b.bounds.center + new Vector3(-b.bounds.extents.x, -b.bounds.extents.y, -b.bounds.extents.z);

        return bounds;
    }
    private Vector3[] GetHalfColliderBounds(BoxCollider b, bool isLowerSide)
    {
        Vector3[] bounds = new Vector3[4];

        if (isLowerSide)
        {
            bounds[0] = (b.bounds.center + new Vector3(-b.bounds.extents.x, -b.bounds.extents.y, b.bounds.extents.z));
            bounds[1] = (b.bounds.center + new Vector3(-b.bounds.extents.x, -b.bounds.extents.y, -b.bounds.extents.z));
            bounds[2] = (b.bounds.center + new Vector3(b.bounds.extents.x, -b.bounds.extents.y, b.bounds.extents.z));
            bounds[3] = (b.bounds.center + new Vector3(b.bounds.extents.x, -b.bounds.extents.y, -b.bounds.extents.z));
        }
        else
        {
            bounds[0] = (b.bounds.center + new Vector3(b.bounds.extents.x, b.bounds.extents.y, b.bounds.extents.z));
            bounds[1] = (b.bounds.center + new Vector3(b.bounds.extents.x, b.bounds.extents.y, -b.bounds.extents.z));
            bounds[2] = (b.bounds.center + new Vector3(-b.bounds.extents.x, b.bounds.extents.y, b.bounds.extents.z));
            bounds[3] = (b.bounds.center + new Vector3(-b.bounds.extents.x, b.bounds.extents.y, -b.bounds.extents.z));
        }

        return bounds;
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.white;
//        Gizmos.DrawWireSphere(routePoints[1].position, 2f);
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(routePoints[5].position, 2f);
//    }
//#endif

}
