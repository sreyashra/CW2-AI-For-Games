using System.IO;
using UnityEngine;
using System.Globalization;
using System;
using System.Linq;


public class SOM : MonoBehaviour
{
    [SerializeField]
    int Iterations = 100000;
    [SerializeField]
    float LearningRate = 0.8f;
    float LearningRateDecay = 0.99997f;
    float PopulationDecay = 0.9997f;

    [SerializeField] private GameObject cityPrefab;
    private LineRenderer routeRenderer;

    void Start()
    {
        routeRenderer = gameObject.AddComponent<LineRenderer>();
        routeRenderer.material.color=Color.blue;
        routeRenderer.widthMultiplier = 0.05f;
        Vector2[] PointLocations = ReadFile();
        Vector2[] Route = SelfOM(PointLocations, Iterations, LearningRate);
    }

    Vector2[] ReadFile()
    {
        string[] filelines = File.ReadAllLines("C:/Users/AchalSharma/Desktop/som-tsp/assets/qa194.tsp");
        int DimVar = filelines.Length;
        int i, j;

        for (i = 0; i < filelines.Length; i++)
        {
            if (filelines[i].StartsWith("DIMENSION :"))
            {
                DimVar = Convert.ToInt32(filelines[i].Split().Last());
            }
            if (filelines[i].StartsWith("NODE_COORD_SECTION"))
            {
                i++;
                break;
            }
        }
        Vector2[] PointLocations = new Vector2[DimVar];
        for (j = 0; i < filelines.Length; i++, j++)
        {
            if (filelines[i] == "EOF")
            {
                break;
            }
            string[] parts = filelines[i].Split(" "[0]);
            float coordinate1 = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float coordinate2 = float.Parse(parts[2], CultureInfo.InvariantCulture);
            PointLocations[j] = new Vector2(coordinate2, coordinate1);
        }
        return PointLocations;
    }

    Vector2[] SelfOM(Vector2[] PointLocations, int Iterations, float LearningRate)
    {        
        Vector2[] UpdatedCitiesRoute = new Vector2[PointLocations.Length];
        Vector2[] Cities = Normalize(PointLocations);
        int[] Route = new int[PointLocations.Length];
        float Population = PointLocations.Length * 6; //Taking Population 6 times the size of the total number of cities.
        Vector2[] Network = GetTheNetwork(Population);

        for (int i = 0; i < Iterations; i++)
        {
            if (i % 100 != 0)
            {
                print("\tIteration {i}/{Iterations}");
            }
            int randomIndex = UnityEngine.Random.Range(0, Cities.Length);
            Vector2 city = Cities[randomIndex];
            int winnerIndex = GetClosestCity(Network, city);
            float[] gaussian = GaussianConvert(winnerIndex, Population / 10.0f, Network.Length);
            Network = UpdatedNetwork(gaussian, city, Network);
            LearningRate = (float)(LearningRate * LearningRateDecay);
            Population *= PopulationDecay;
        }
        //Spawns the circle based on cities data
        Route = GetRoute(Cities, Network);
        Array.Sort(Route, Cities);
        //plot points and lines
        Vector2[] citiesCopy = Cities;
        for (int i = 0; i < Cities.Length; i++)
        {
            citiesCopy[i].x *= 9;
            citiesCopy[i].y *= 9;
        }
        for (int i = 0; i < Cities.Length; i++)
        {
            GameObject temp = Instantiate(cityPrefab);
            temp.transform.position = citiesCopy[i];
            temp.transform.SetParent(this.transform);
            temp.name = citiesCopy[i].ToString();
            //Debug.Log(Cities[i]);
        }

        //Generates lines or route among the cities
        routeRenderer.positionCount = citiesCopy.Length;
        DrawRoute(citiesCopy);
        
        return UpdatedCitiesRoute;
    }

    void DrawRoute(Vector2[] pos)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            routeRenderer.SetPosition(i,pos[i]);
        }
    }
    
    Vector2[] Normalize(Vector2[] PointLocations)
    {
        float[] PointLocationsX = new float[PointLocations.Length];
        float[] PointLocationsY = new float[PointLocations.Length];
        for (int i = 0; i < PointLocations.Length; i++)
        {
            PointLocationsX[i] = PointLocations[i].x;
            PointLocationsY[i] = PointLocations[i].y;
        }
        float ratio = (PointLocationsX.Max() - PointLocationsX.Min()) / (PointLocationsY.Max() - PointLocationsY.Min());
        Vector2[] Points = new Vector2[PointLocations.Length];
        for(int i = 0; i < PointLocations.Length; i++)
        {
            Points[i].x = ((PointLocations[i].x - PointLocationsX.Min()) / (PointLocationsX.Max() - PointLocationsX.Min())) * ratio;
            Points[i].y = (PointLocations[i].y - PointLocationsY.Min()) / (PointLocationsY.Max() - PointLocationsY.Min());
        }
        return Points;
    }

    Vector2[] GetTheNetwork(float temp)
    {
        Vector2[] RandomVector = new Vector2[(int)temp];
        for (int i = 0; i < temp; i++)
        {
            RandomVector[i] = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
        }
        return RandomVector;
    }

    int GetClosestCity(Vector2[] Network, Vector2 city)
    {
        float[] distance = new float[Network.Length];
        for (int i = 0; i < Network.Length; i++)
        {
            distance[i] = Vector2.Distance(Network[i], city);
        }
        return Array.IndexOf(distance, distance.Min());
    }

    float[] GaussianConvert(int center, float radix, int domain)
    {
        float[] dist = new float[domain];
        float[] gaussian = new float[domain];
        if (radix < 1)
        {
            radix = 1;
        }
        for(int i = 0; i < domain; i++)
        {
            dist[i] = Mathf.Min(Mathf.Abs(center - i), domain - Mathf.Abs(center - i));
            gaussian[i] = Mathf.Exp(( -(dist[i] * dist[i])) / (2 * (radix * radix)));
        }        
        return gaussian;
    }

    Vector2[] UpdatedNetwork(float[] gaussian, Vector2 city, Vector2[] Network)
    {
        for(int i = 0; i < Network.Length; i++)
        {
            Network[i] += gaussian[i] * LearningRate * (city - Network[i]);
        }
        return Network;
    }

    int[] GetRoute(Vector2[] Cities, Vector2[] Network)
    {
        int[] Route = new int[Cities.Length];
        for(int i = 0; i < Cities.Length; i++)
        {
            Route[i] = GetClosestCity(Network, Cities[i]);
        }
        return Route;
    }
}