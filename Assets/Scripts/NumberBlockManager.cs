using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberBlockManager : MonoBehaviour
{
    [SerializeField] NumBlock numBlockPrefab;


    float[] probabilities = { 0.5f, 0.25f, 0.15f, 0.1f };

    float[] cummulativeProbabilities = new float[4];

    List<int> spawnNumberBlockValues = new List<int>(4) { 2, 4, 8, 16};

    List<NumBlock> numBlocksCreated = new List<NumBlock>();

    private void Awake()
    {
        SetCummulativeProbabilityValues();

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public NumBlock CreateNumberBlockAccordingToProbability()
    {
        SetCummulativeProbabilityValues();
        float randomValue = Random.Range(0f, 1f);
        Debug.Log(randomValue);
        for (int i = 0; i < cummulativeProbabilities.Length; i++)
        {
            if(cummulativeProbabilities[i]>= randomValue)
            {
                Debug.Log("cummulativeVal: " + cummulativeProbabilities[i] + " index:" + i);
                return CreateNumberBlock(spawnNumberBlockValues[i]);
            }

        }
        return null;
    }


    private void SetCummulativeProbabilityValues()
    {
        float cummulativeValue = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cummulativeValue += probabilities[i];
            cummulativeProbabilities[i] = cummulativeValue;
        }
    }

    NumBlock CreateNumberBlock(int numValue)
    {
        NumBlock numBlock = Instantiate(numBlockPrefab);
        //numBlock.Init(numValue);
        numBlocksCreated.Add(numBlock);
        return numBlock;
    }

}
