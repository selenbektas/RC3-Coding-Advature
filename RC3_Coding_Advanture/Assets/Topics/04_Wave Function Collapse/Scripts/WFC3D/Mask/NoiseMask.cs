using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMask : VolumetricMask
{
	public float threshold = 0.5f;
	public float scale = 1.0f;
    public override bool IsMaskedTrue(Vector3 point)
    {
		float t = fbm3(point*scale);

		return t > threshold;
    }

	float frac(float value)
	{
		return value - Mathf.Floor(value);
	}

	Vector2 frac(Vector2 vec)
	{
		return new Vector2(frac(vec.x), frac(vec.y));
	}

	Vector3 frac(Vector3 point)
	{
		return new Vector3(frac(point.x), frac(point.y), frac(point.z));
	}
	

	float hash(float n)
	{
		return frac(Mathf.Sin(n) * 43758.5453f);
	}

	
	float lerp(float a,float b, float t)
	{
		return a + t * (b - a);
	}

	Vector3 mul(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	float noise3(Vector3 x)
	{
		// The noise function returns a value in the range -1.0f -> 1.0f

		Vector3 p = new Vector3(Mathf.Floor(x.x), Mathf.Floor(x.y), Mathf.Floor(x.z));
		Vector3 f = frac(x);

		f = mul(f , mul(f, (3.0f*Vector3.one - 2.0f * f)));

		float n = p.x + p.y * 57.0f + 113.0f * p.z;

		return lerp(lerp(lerp(hash(n + 0.0f), hash(n + 1.0f), f.x),
					   lerp(hash(n + 57.0f), hash(n + 58.0f), f.x), f.y),
				   lerp(lerp(hash(n + 113.0f), hash(n + 114.0f), f.x),
					   lerp(hash(n + 170.0f), hash(n + 171.0f), f.x), f.y), f.z);
	}


	float fbm3(Vector3 st)
	{
		// Initial values
		float value = 0.0f;
		float amplitude = 0.5f;
		//
		// Loop of octaves
		for (int i = 0; i < 6; i++)
		{
			value += amplitude * noise3(st);
			st *= 2.0f;
			amplitude *= 0.5f;
		}
		return value;
	}
}
