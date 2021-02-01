using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.Text; // StringBuilder
#endif

using com.GMM.RandomUtils;

[System.Serializable]
public class ObjectWeight
{
    public string name;
    [Range(0, 100)]
    public int weight;
}

/// <summary>
/// Random Weight
/// </summary>
[System.Serializable]
public class RandomWeight : MonoBehaviour
{
	public bool updateSumEachRandom = false;

	public ObjectWeight[] weights;
	protected int _sum = 0;
	public int WeightSum { get { return _sum; } }

	private bool _initialized = false;
	
	/// <summary>
	/// Updates the sum of all weights
	/// </summary>
	public bool UpdateWeightSum()
	{
		if (weights == null || weights.Length == 0)
		{
			Debug.LogWarning("[RandomWeight] weights == NULL");
			_initialized = false;
			return false;
		}

		_sum = 0;
		for (int i=0; i<weights.Length; ++i)
		{
			_sum += weights[i].weight;
		}
		_initialized = true;
		return true;
	}

	/// <summary>
	/// Gets the weight with index index
	/// </summary>
	public int GetWeight(int index)
	{
		if (index >= 0 && index < weights.Length)
		{
			return weights[index].weight;
		}
		return -1;
	}

	/// <summary>
	/// Set the weight with index index
	/// </summary>
	public bool SetWeight(int index, int weight)
	{
		if (index >= 0 && index < weights.Length)
		{
			weights[index].weight = weight;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets a copy of the array of weights
	/// </summary>
	public int[] GetWeights()
	{
		if (weights != null)
		{
			return (int[])weights.Clone();
		}
		return null;
	}

	/// <summary>
	/// Sets the array of weights and update weights sum
	/// </summary>
	public bool SetWeights(int[] weights)
	{
		if (weights != null)
		{
			this.weights = (ObjectWeight[])weights.Clone();
			UpdateWeightSum();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets a Random
	/// </summary>
	public int GetRandom()
	{
		if (updateSumEachRandom == true || _initialized == false)
		{
			UpdateWeightSum();
		}
		// if weights == NULL RandomUtils.RandomWeight returns 0
		return RandomUtils.RandomWeight(weights, _sum);
	}

    //#if UNITY_EDITOR
    //	private const int ITERATIONS = 1000;

    //	public void TestRandomWeight()
    //	{
    //		int[] results = new int[weights.Length];
    //		for (int i = 0; i<ITERATIONS; ++i)
    //		{
    //			++results[GetRandom()];
    //		}

    //		StringBuilder str = new StringBuilder();

    //		str.AppendLine(string.Format("---- RandomWeights ({0} Iterations) ----", ITERATIONS));
    //		for (int i=0; i<results.Length; ++i)
    //		{
    //			str.AppendLine(string.Format("\t{0} --> {1}%", i, ((float)results[i] / (float)ITERATIONS * 100f)));
    //		}
    //		Debug.Log (str);
    //	}
    //#endif

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log((POWERUPS)GetRandom());
        }
    }
}
