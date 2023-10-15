using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NeuralNetwork
{
    private List<Layer> layers;

    public NeuralNetwork()
    {
        layers = new List<Layer>();
    }

    public float[] computeNetwork(float[] input)
    {
        if(input.Length != layers[0].getInputSize() - 1)
        {
            Debug.Log("Input vector is of incorrect size");
            return null;
        }
        else
        {
            float[] vector = new float[input.Length];
            input.CopyTo(vector, 0);

            foreach(Layer l in layers)
            {
                vector = l.compute(vector);
            }
            return vector;
        }
    }

    public void addLayer(Layer layer)
    {
        this.layers.Add(layer);
    }

    public List<Layer> getLayers()
    {
        return layers;
    }

    public void setLayers(List<Layer> layers)
    {
        this.layers = layers;
    }

    public void setLayers(Layer[] layers)
    {
        this.layers = layers.ToList<Layer>();
    }

    public void initRandomWeights()
    {
        foreach(Layer l in layers)
        {
            l.initRandomWeights();
        }
    }

    public void initRandomBiases()
    {
        foreach (Layer l in layers)
        {
            l.initRandomBiases();
        }
    }

    public NeuralNetwork copy()
    {
        NeuralNetwork n = new NeuralNetwork();
        Layer[] ls = new Layer[layers.Count];
        for(int i = 0; i < layers.Count; i++)
        {
            ls[i] = layers[i].copy();
        }
        n.setLayers(ls);

        return n;        
    }

    public void deviateWeights(float maxDeviation)
    {
        foreach (Layer l in layers)
        {
            l.deviateWeights(maxDeviation);
        }
    }

    public void deviateOneWeight(float maxDeviation)
    {
        Layer layer = layers[(int)Random.Range(0, layers.Count)];
        float[,] weights = layer.getWeights();

        weights[(int)Random.Range(0, weights.GetLength(0)), (int)Random.Range(0, weights.GetLength(1))] += Random.Range(-maxDeviation, maxDeviation);

    }

}