using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ArrayExtensions
{
	private static void Seed(int seed)
	{
		if (seed == 0)
			return;
		Random.InitState(seed);
	}

	public static T GetRandomWithinRange<T>(this IList<T> list, int startIndex, int endIndex, int seed = 0)
	{
		Seed(seed);
		return list[Random.Range(startIndex, endIndex)];
	}

	public static T GetRandomStartingAt<T>(this IList<T> list, int startIndex, int seed = 0)
	{
		Seed(seed);
		return list[Random.Range(startIndex, list.Count)];
	}

	public static T GetRandomEndingAt<T>(this IList<T> list, int endIndex, int seed = 0)
	{
		Seed(seed);
		return list[Random.Range(0, endIndex)];
	}

	public static T GetRandom<T>(this IList<T> list, int seed = 0)
	{
		Seed(seed);
		return list[Random.Range(0, list.Count)];
	}
}