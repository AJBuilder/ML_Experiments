using System.Collections.Generic;

public class NeuralNetwork
{
    private List<Layer> layers;

    public NeuralNetwork()
    {
        layers = new List<Layer>();
    }

    public float[] computeNetwork(float[] input)
    {
        if(input.Length != layers[0].getInputSize())
        {
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

    public void initRandomWeights()
    {
        foreach(Layer l in layers)
        {
            l.initRandomWeights();
        }
    }

    public void deviateWeights(float maxDeviation)
    {
        foreach (Layer l in layers)
        {
            l.deviateWeights(maxDeviation);
        }
    }

}