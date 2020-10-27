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
	public struct FractalBillowDeriv<T> : INoise3D, INoiseDeriv3D where T : struct, INoiseDeriv3D {
    public readonly T mNoise;
    public readonly int octaves;
    public readonly float gain;
    public readonly float weightedStrength;
    public readonly float lacunarity;
    public readonly float fractalBounding;

    public FractalBillowDeriv(int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) : this(new T(), octaves, lacunarity, gain, weightedStrength) {
    
    }

    public FractalBillowDeriv(T noise, int octaves, float lacunarity = 1.99f, float gain = 0.5f, float weightedStrength = 0) {
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
      dsum = new real3();

      for (int i = 0; i < octaves; i++) {
        real noise = abs(mNoise.GetValue(seed, point, out var deriv)) * 2 - 1;
        dsum += deriv;
        sum += noise * amp / (1 + dot(dsum, dsum));
        amp *= lerp(1.0f, (noise + 1) * 0.5f, weightedStrength);

        point *= lacunarity;
        amp *= gain;
      }

      return sum;
    }
  }
}
