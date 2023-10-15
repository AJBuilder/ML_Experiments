using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst;
using UnityEngine;

public class Layer
{
    private float[,] weights;
    private System.Func<float, float> activation = TanH;
    public enum activationFunctions
    {
        Linear,
        TanH,
        ReLU
    }

    public Layer(int inputDim, int outputDim, Layer.activationFunctions activation)
    {
        weights = new float[outputDim, inputDim + 1]; // +1 is the bias
    }

    public Layer(int inputDim, int outputDim, System.Func<float, float> activationFunc)
    {
        weights = new float[outputDim, inputDim + 1]; // +1 is the bias
    }


    public float[] compute(float[] input)
    {
        float[] output = new float[weights.GetLength(0)];

        for (int i = 0; i < weights.GetLength(0); i++)
        {
            float o = 0;
            for (int j = 0; j < weights.GetLength(1) - 1; j++)
            {
                o += weights[i, j] * input[j];
            }
            output[i] = activation(weights[i, weights.GetLength(1) - 1] + o);
        }

        return output;
    }

    public void setActivationFunc(activationFunctions func)
    {
        switch (func)
        {
            case activationFunctions.Linear:
                activation = Linear;
                break;
            case activationFunctions.TanH:
                activation = TanH;
                break;
            case activationFunctions.ReLU:
                activation = ReLU;
                break;
        }
    }

    public void initRandomWeights(){
        for(int i = 0; i < weights.GetLength(0); i++){
            for ( int j = 0; j < weights.GetLength(1); j++){
                weights[i,j] = Random.Range(-1f, 1f);
            }
        }
    }

    internal void initRandomBiases()
    {
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            weights[i, weights.GetLength(1) - 1] = Random.Range(-1f, 1f);
        }
    }

    public void deviateWeights(float maxDeviation)
    {
        float absDev = System.Math.Abs(maxDeviation);
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                weights[i, j] += Random.Range(-absDev, absDev);
            }
        }
    }

    public bool setWeights(float[,] _weights){
        if (_weights.GetLength(0) != weights.GetLength(0) && _weights.GetLength(1) != weights.GetLength(1)){
            return false;
        }
        else {
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                for (int j = 0; j < weights.GetLength(1); j++)
                {
                    weights[i, j] = _weights[i,j];
                }
            }
            return true;
        }
    }

    public float[,] getWeights()
    {
        return weights;
    }

    public int getOutputSize()
    {
        return weights.GetLength(0);
    }
    public int getInputSize()
    {
        return weights.GetLength(1);
    }

    public Layer copy()
    {
        Layer l = new Layer(weights.GetLength(1) - 1, weights.GetLength(0), this.activation);
        l.setWeights(weights);
        return l;
    }

    static float Linear(float x)
    {
        return x;
    }

    static float TanH(float x)
    {
        return (float)System.Math.Tanh(x);
    }

    static float ReLU(float x){

        if(x < 0){
            return 0;
        }
        else {
            return x;
        }


    }

    
}
