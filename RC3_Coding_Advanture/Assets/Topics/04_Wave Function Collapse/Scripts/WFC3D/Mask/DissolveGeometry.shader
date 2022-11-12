Shader "Custom/DissolveGeometry"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_EdgeColor("Edge Color",Color) = (1,1,1,1)
		_EdgeEmission("Edge Emission",float) = 0.0
		_MainTex("Albedo", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		 _Dissolve("Dissolve",Range(0,1)) = 0.0
			_Scale("Scale",float) = 1.0
			_Noise("Noise",Range(0,1)) = 1.0
			
	}
		SubShader
		{
			Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}

			LOD 200

			 Pass {
				 ColorMask 0
			 }

			 ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0




			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)



			float random(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
			}

			float2 random2(float2 st) {
				st = float2(dot(st, float2(127.1, 311.7)),dot(st, float2(269.5, 183.3)));
				return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
			}

			float3 random3(float3 st) 
			{
				return frac(sin(dot(st ,float3(12.9898,78.233,45.5432))) * 43758.5453);
			}


			float mix(float a,float b,float t)
			{
				return  a * (1 - t) + b * t;
			}

			float hash(float n)
			{
				return frac(sin(n) * 43758.5453);
			}


			float noise(in float2 st)
			{
				float2 i = floor(st);
				float2 f = frac(st);

				// Four corners in 2D of a tile
				float a = random(i);
				float b = random(i + float2(1.0, 0.0));
				float c = random(i + float2(0.0, 1.0));
				float d = random(i + float2(1.0, 1.0));

				float2 u = f * f * (3.0 - 2.0 * f);

				return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
			}

			float noise3(in float3 x)
			{
				// The noise function returns a value in the range -1.0f -> 1.0f

				float3 p = floor(x);
				float3 f = frac(x);

				f = f * f * (3.0 - 2.0 * f);
				float n = p.x + p.y * 57.0 + 113.0 * p.z;

				return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0),f.x),
							   lerp(hash(n + 57.0), hash(n + 58.0),f.x),f.y),
						   lerp(lerp(hash(n + 113.0), hash(n + 114.0),f.x),
							   lerp(hash(n + 170.0), hash(n + 171.0),f.x),f.y),f.z);
			}

			#define OCTAVES 6
			float fbm3(in float3 st) {
				// Initial values
				float value = 0.0;
				float amplitude = .5;
				float frequency = 0.;
				//
				// Loop of octaves
				for (int i = 0; i < OCTAVES; i++) {
					value += amplitude * noise3(st);
					st *= 2.;
					amplitude *= 0.5;
				}
				return value;
			}



			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			half _Glossiness;
			half _Metallic;
			float _Dissolve;
			float _Scale;
			float _Noise;
			fixed4 _Color;
			fixed4 _EdgeColor;
			float _EdgeEmission;


			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;

			};

			struct Input
			{
				float3 worldPos;
				float4 vert;
				float3 normal;
				float2 uv_MainTex;
			};

			void vert(inout vertexInput v, out Input o) {
				
				
				o.vert = v.vertex+25;
				o.worldPos = mul(unity_ObjectToWorld, o.vert).xyz;
				o.normal = v.normal;

			}


			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;

				//float xy = mix(1, noise(IN.worldPos.xy * _Scale), _Noise);
				//float yz = mix(1, noise(IN.worldPos.yz * _Scale), _Noise);
				//float xz = mix(1, noise(IN.worldPos.xz * _Scale), _Noise);

				float n = fbm3(IN.worldPos * _Scale+ 1.0*float3(_Time.x,-_Time.x,_Time.x));

				float h = 1 - clamp((IN.worldPos.y) / 3, 0, 1);

				h = mix(h, noise3(IN.worldPos*_Scale*0.3), 0.2);

				h = mix(h, n, _Noise);

				//float3 camPos = _WorldSpaceCameraPos;

				//float3 dir = normalize(IN.worldPos - camPos);

				//float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, IN.uv_MainTex);

				//n= mix(n, fbm3((IN.worldPos+dir) * _Scale + 1.0 * float3(_Time.x, -_Time.x, _Time.x)), depth);

				

					if (abs(h - _Dissolve) <= 0.01)
					{
						o.Albedo = _EdgeColor;
						o.Alpha = 1;
						o.Emission = _EdgeEmission * _EdgeColor;

						return;
					}

					/*if (_Dissolve < 0.01) 
					{
						o.Albedo = _EdgeColor;
						o.Alpha = 1;
						o.Emission = _EdgeEmission * _EdgeColor;

						return;
					}*/

					float disov= smoothstep(_Dissolve, _Dissolve + 0.03, h) * c.a;

					o.Alpha = mix(disov, c.a, step(_Dissolve,0.0));


			}
			ENDCG
		}
			FallBack "Standard"
}
