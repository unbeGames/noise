using static Unity.Mathematics.math;
using static Unbegames.Noise.Utils;

#if NOISE_DOUBLE_PRECISION
using real = System.Double;
using real3 = Unity.Mathematics.double3;
#else
using real = System.Single;
using real3 = Unity.Mathematics.float3;
#endif

namespace Unbegames.Noise {
	public struct FractalRiged<T> : INoise3D where T : struct, INoise3D {
    private readonly T mNoise;
    private readonly int octaves;
    private readonly float gain;
    private readonly float weightedStrength;
    private readonly float lacunarity;
    private readonly float fractalBounding;
    public real3 permutation;

    public FractalRiged(int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) : this(new T(), octaves, lacunarity, gain, weightedStrength) {

    }

    public FractalRiged(T noise, int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) {
      mNoise = noise;
      this.octaves = octaves;
      this.lacunarity = lacunarity;
      this.gain = gain;
      this.weightedStrength = weightedStrength;
      permutation = real3.zero;
      fractalBounding = CalculateFractalBounding(octaves, gain);
    }

    public real GetValue(int mSeed, real3 point) {
      int seed = mSeed;
      real sum = 0;
      real amp = fractalBounding;
      real3 permutation = this.permutation;

      for (int i = 0; i < octaves; i++) {
        real noise = abs(mNoise.GetValue(seed++, point));
        sum += (noise * -2 + 1) * amp;
        amp *= lerp(1.0f, 1 - noise, weightedStrength);

        point = point * lacunarity + permutation;
        amp *= gain;
        permutation *= lacunarity;
      }

      return sum;
    }
	}
}
