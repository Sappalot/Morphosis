using UnityEngine;

static class MutationUtil {

	public static bool ShouldMutate(float chanceToMutate) {
		float rnd = Random.Range(0, chanceToMutate * GlobalSettings.instance.mutation.masterMutationStrength + 1000f);
		return (rnd < chanceToMutate * GlobalSettings.instance.mutation.masterMutationStrength);
	}

	public static bool ShouldMutate(float chanceToMutate, float mutationStrength) {
		float rnd = Random.Range(0, chanceToMutate * mutationStrength + 1000f);
		return (rnd < chanceToMutate * mutationStrength);
	}
}
