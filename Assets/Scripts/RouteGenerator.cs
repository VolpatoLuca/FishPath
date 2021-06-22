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

    public void ButtonDebugStart()
    {
        GenerateRoute(startTransform.position);
    }

    public void GenerateRoute(Vector3 startPos)
    {
        for (int i = 0; i < routePoints.Length; i++)
        {
            if (routePoints[i] == null)
            {
                routePoints[i] = new GameObject("Route node " + i).transform;
            }
        }

        ThreeRoutesGenerate(startPos);

        //fishController.routesToFollow[0].p1 = routePoints[0].position;
        //fishController.routesToFollow[0].p2 = routePoints[1].position;
        //fishController.routesToFollow[0].p3 = routePoints[2].position;
        //fishController.routesToFollow[0].p4 = routePoints[3].position;
        //fishController.routesToFollow[1].p1 = routePoints[4].position;
        //fishController.routesToFollow[1].p2 = routePoints[5].position;
        //fishController.routesToFollow[1].p3 = routePoints[6].position;
        //fishController.routesToFollow[1].p4 = routePoints[7].position;
        //fishController.routesToFollow[2].p1 = routePoints[8].position;
        //fishController.routesToFollow[2].p2 = routePoints[9].position;
        //fishController.routesToFollow[2].p3 = routePoints[10].position;
        //fishController.routesToFollow[2].p4 = routePoints[11].position;

        fishController.OnRoutesGenerated(routePoints);
    }

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

        //First Curve
        routePoints[0].position = start;
        routePoints[1].position = nearEdges[1];
        routePoints[2].position = farEdges[3];
        routePoints[3].position = Vector3.Lerp(tank.bounds.center, nearEdges[2], 0.2f);

        //Second Curve
        routePoints[4].position = routePoints[3].position;
        routePoints[5].position = Vector3.Lerp(tank.bounds.center, nearEdges[0], 0.9f);
        routePoints[6].position = Vector3.Lerp(tank.bounds.center, farEdges[0], 0.8f);
        routePoints[7].position = Vector3.Lerp(tank.bounds.center, farEdges[1], 0.2f);

        //Third Curve
        routePoints[8].position = routePoints[7].position;
        routePoints[9].position = nearEdges[3];
        routePoints[10].position = farEdges[2];
        routePoints[11].position = start;

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


}
