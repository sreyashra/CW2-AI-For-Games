using System.IO;
using UnityEngine;
using System.Globalization;
using System;
using System.Linq;

public class SOM : MonoBehaviour
{
    [SerializeField]
    int Iterations;
    [SerializeField]
    float LearningRate;

    void Start()
    {
        var PointLocations = ReadFile();
        var Path = SelfOM(PointLocations, Iterations, LearningRate);
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
        Vector2[] Path = Normalize(PointLocations);
        int Population = PointLocations.Length * 6; //Taking Population 6 times the size of the total number of cities.
        Vector2[] NetworkSize = GetTheNetwork(Population);
        //for(int i = 0; i < iterations; i++)
        //{
        //    if(i % 100 != 0)
        //    print('\t> Iteration {}/{}'.format(i, iterations), end = "\r")
        //# Choose a random city
        //city = cities.sample(1)[['x', 'y']].values
        //winner_idx = select_closest(network, city)
        //# Generate a filter that applies changes to the winner's gaussian
        //gaussian = get_neighborhood(winner_idx, n // 10, network.shape[0])
        //# Update the network's weights (closer to the city)
        //network += gaussian[:, np.newaxis] * learning_rate * (city - network)
        //# Decay the variables
        //learning_rate = learning_rate * 0.99997
        //n = n * 0.9997

        //# Check for plotting interval
        //    if not i % 1000:
        //    plot_network(cities, network, name = 'diagrams/{:05d}.png'.format(i))

        //# Check if any parameter has completely decayed.
        //    if n < 1:
        //    print('Radius has completely decayed, finishing execution',
        //          'at {} iterations'.format(i))
        //    break
        //if learning_rate < 0.001:
        //    print('Learning rate has completely decayed, finishing execution',
        //          'at {} iterations'.format(i))
        //    break
        //else:
        //    print('Completed {} iterations.'.format(iterations))
        //}
        return PointLocations;
    }

    Vector2[] Normalize(Vector2[] PointLocations)
    {
        for (int i = 0; i < PointLocations.Length; i++)
        {
            PointLocations[i] = PointLocations[i].normalized;
        }
        return PointLocations;
    }

    Vector2[] GetTheNetwork(int temp)
    {
        System.Random rand = new();
        Vector2[] RandomVector = new Vector2[temp];
        for (int i = 0; i < temp; i++)
        {
            RandomVector[i].x = (float)rand.NextDouble();
            RandomVector[i].y = (float)rand.NextDouble();
        }
        return RandomVector;
    }
}
