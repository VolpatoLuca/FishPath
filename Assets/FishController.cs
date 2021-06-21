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
    public float tParam;
    private Vector3 newPos;
    private bool isInCoroutine;
    private bool canSwim;
    private float totalRouteDistance;

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
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(CalculateBezierPoint(p1, p2, p3, p4, Mathf.Clamp(tParam * 1.1f, 0, 1)) - transform.position), rotationSpeed * Time.deltaTime);

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
        Vector3 pos;

        pos = p1 * Mathf.Pow(1 - t, 3) + 3 * p2 * t * Mathf.Pow(1 - t, 2) + 3 * p3 * t * t * (1 - t) + p4 * Mathf.Pow(t, 3);

        return pos;
    }

    public void OnRoutesGenerated()
    {
        canSwim = true;
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
}

