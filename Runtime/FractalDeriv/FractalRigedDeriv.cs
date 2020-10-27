﻿using static Unity.Mathematics.math;
using static Unbegames.Noise.Utils;

#if NOISE_DOUBLE_PRECISION
using real = System.Double;
using real3 = Unity.Mathematics.double3;
#else
using real = System.Single;
using real3 = Unity.Mathematics.float3;
#endif

namespace Unbegames.Noise {
	public struct FractalRigedDeriv<T> : INoise3D, INoiseDeriv3D where T : struct, INoiseDeriv3D {
    private readonly T mNoise;
    private readonly int octaves;
    private readonly float gain;
    private readonly float weightedStrength;
    private readonly float lacunarity;
    private readonly float fractalBounding;

    public FractalRigedDeriv(int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) : this(new T(), octaves, lacunarity, gain, weightedStrength) {

    }

    public FractalRigedDeriv(T noise, int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) {
      mNoise = noise;
      this.octaves = octaves;
      this.lacunarity = lacunarity;
      this.gain = gain;
      this.weightedStrength = weightedStrength;
      fractalBounding = CalculateFractalBounding(octaves, gain);
    }

    public real GetValue(int mSeed, real3 point) {
      return GetValue(mSeed, point);
    }

    public real GetValue(int mSeed, real3 point, out real3 dsum) {
      int seed = mSeed;
      real sum = 0;
      real amp = fractalBounding;
      dsum = real3.zero;

      for (int i = 0; i < octaves; i++) {
        real noise = abs(mNoise.GetValue(seed++, point, out var deriv));
        dsum += deriv;
        sum += (noise * -2 + 1) * amp * (1 + dot(dsum, dsum));
        amp *= lerp(1.0f, 1 - noise, weightedStrength);

        point *= lacunarity;
        amp *= gain;
      }

      return sum;
    }
  }
}
