using System.Collections;
using UnityEngine;

public class Route
{
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;
    public Vector3 p4;
}

public class FishController : MonoBehaviour
{
    public Route[] routesToFollow;
    [Range(0, 1)]
    [SerializeField] private float moveSpeed = .1f;
    [SerializeField] private float rotationSpeed = .1f;
    private int currentRoute;
    private int routesAmount;
    [HideInInspector] public float tParam;
    private Vector3 newPos;
    private bool isInCoroutine;
    private bool canSwim;
    private float totalRouteDistance;
    private bool GizmosDrew;
    private Color[] curveColors;

    private void Start()
    {
        currentRoute = 0;
        tParam = 0;
        isInCoroutine = false;
        routesAmount = 3;
        routesToFollow = new Route[routesAmount];
        for (int i = 0; i < routesAmount; i++)
        {
            routesToFollow[i] = new Route();
        }
    }


    private void Update()
    {
        if (!isInCoroutine && canSwim)
        {
            StartCoroutine(FollowRoute(currentRoute));
        }
    }

    private IEnumerator FollowRoute(int route)
    {
        isInCoroutine = true;
        Vector3 p1 = routesToFollow[route].p1;
        Vector3 p2 = routesToFollow[route].p2;
        Vector3 p3 = routesToFollow[route].p3;
        Vector3 p4 = routesToFollow[route].p4;

        float routeLenght = Vector3.Distance(p1, p2) + Vector3.Distance(p3, p4);
        float speedNormalizer = totalRouteDistance / routeLenght;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * (moveSpeed / 30) * speedNormalizer;

            newPos = CalculateBezierPoint(p1, p2, p3, p4, tParam);

            transform.position = newPos;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CalculateBezierPoint(p1, p2, p3, p4, Mathf.Clamp(tParam + 0.05f, 0, 1)) - transform.position), rotationSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        tParam = 0;

        currentRoute++;
        if (currentRoute >= routesAmount)
            currentRoute = 0;
        isInCoroutine = false;

    }

    private Vector3 CalculateBezierPoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        return p1 * Mathf.Pow(1 - t, 3) + 3 * p2 * t * Mathf.Pow(1 - t, 2) + 3 * p3 * t * t * (1 - t) + p4 * Mathf.Pow(t, 3);
    }

    public void OnRoutesGenerated(Transform[] routePoints)
    {
        routesToFollow[0].p1 = routePoints[0].position;
        routesToFollow[0].p2 = routePoints[1].position;
        routesToFollow[0].p3 = routePoints[2].position;
        routesToFollow[0].p4 = routePoints[3].position;
        routesToFollow[1].p1 = routePoints[4].position;
        routesToFollow[1].p2 = routePoints[5].position;
        routesToFollow[1].p3 = routePoints[6].position;
        routesToFollow[1].p4 = routePoints[7].position;
        routesToFollow[2].p1 = routePoints[8].position;
        routesToFollow[2].p2 = routePoints[9].position;
        routesToFollow[2].p3 = routePoints[10].position;
        routesToFollow[2].p4 = routePoints[11].position;

        canSwim = true;
        GizmosDrew = false;
        StopAllCoroutines();
        tParam = 0;
        currentRoute = 0;
        isInCoroutine = false;
        totalRouteDistance = 0;
        for (int i = 0; i < routesAmount; i++)
        {
            totalRouteDistance += Vector3.Distance(routesToFollow[i].p1, routesToFollow[i].p2) + Vector3.Distance(routesToFollow[i].p3, routesToFollow[i].p4);
        }
    }

    private void Grabbed()
    {
        canSwim = false;
    }

    private void OnDrawGizmos()
    {

        if (canSwim)
        {
            if (!GizmosDrew)
            {
                curveColors = new Color[routesAmount];
                for (int i = 0; i < routesAmount; i++)
                {
                    curveColors[i] = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
                }
            }
            GizmosDrew = true;
            for (int i = 0; i < routesAmount; i++)
            {
                float t = 0;
                while (t <= 1)
                {
                    Gizmos.color = curveColors[i];
                    Gizmos.DrawWireSphere(CalculateBezierPoint(routesToFollow[i].p1, routesToFollow[i].p2, routesToFollow[i].p3, routesToFollow[i].p4, t), 0.2f);
                    t += 0.01f;
                }
            }
        }
    }
}

