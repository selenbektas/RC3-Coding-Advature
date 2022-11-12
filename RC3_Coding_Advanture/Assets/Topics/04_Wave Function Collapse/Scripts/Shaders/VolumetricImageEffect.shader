Shader "Hidden/VolumetricImageEffect"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
        _MainTex("Main Tex",2D) = "white"{}
         _V3d("V3d", 3D) = "white" {}
        _Alpha("Alpha",Range(0,1)) = 1.0
        _Threshold("Threshold",Range(0,1)) = 0.0
         _StepSize("Step Size",float) = 1.0
            _MaxDensity("Max Density",float) =1.0 
    }
    SubShader
    {
        // No culling or depth

        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = v.uv;
                // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));
                return output;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler3D _V3d;
            float _StepSize;
            float _Threshold;
            float _MaxDensity;
            float _Alpha;
            float4 _Color;

            float3 boundMin;
            float3 boundMax;

#define MAX_STEP_COUNT 128

            // Returns (dstToBox, dstInsideBox). If ray misses box, dstInsideBox will be zero
            float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 invRaydir) {
                // Adapted from: http://jcgt.org/published/0007/03/04/
                float3 t0 = (boundsMin - rayOrigin) * invRaydir;
                float3 t1 = (boundsMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);

                float dstA = max(max(tmin.x, tmin.y), tmin.z);
                float dstB = min(tmax.x, min(tmax.y, tmax.z));

                // CASE 1: ray intersects box from outside (0 <= dstA <= dstB)
                // dstA is dst to nearest intersection, dstB dst to far intersection

                // CASE 2: ray intersects box from inside (dstA < 0 < dstB)
                // dstA is the dst to intersection behind the ray, dstB is dst to forward intersection

                // CASE 3: ray misses box (dstA > dstB)

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox, dstInsideBox);
            }

            float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
                return minNew + (v - minOld) * (maxNew - minNew) / (maxOld - minOld);
            }

            float3 remap3(float3 pos, float3 min, float3 max,float3 minNew,float3 maxNew) 
            {
                float x = remap(pos.x, min.x, max.x, minNew.x, maxNew.x);
                float y = remap(pos.y, min.y, max.y, minNew.y, maxNew.y);
                float z = remap(pos.z, min.z, max.z, minNew.z, maxNew.z);

                return float3(x, y, z);
            }

            bool isOut(float3 uvw,float threshold) 
            {
                return uvw.x > 1-threshold || uvw.x < 0+threshold || uvw.y>1-threshold || uvw.y < 0+threshold || uvw.z>1-threshold || uvw.z < 0+threshold;
            }

            float4 BlendUnder(float4 color, float4 newColor)
            {
                color.rgb += (1.0 - color.a) * newColor.a * newColor.rgb;
                color.a += (1.0 - color.a) * newColor.a;
                return color;
            }

          
            fixed4 frag (v2f i) : SV_Target
            {
                  float3 rayPos = _WorldSpaceCameraPos;
                float viewLength = length(i.viewVector);
                float3 rayDir = i.viewVector / viewLength;

                float nonlin_depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);

                //return float4(nonlin_depth, nonlin_depth, nonlin_depth, 1.0);

                float depth = LinearEyeDepth(nonlin_depth) * viewLength;

                float2 rayToContainerInfo = rayBoxDst(boundMin, boundMax, rayPos, 1 / rayDir);

                float dstToBox = rayToContainerInfo.x;
                float dstInsideBox = rayToContainerInfo.y;

                float dstLimit = min(depth - dstToBox, dstInsideBox);

                float3 entryPoint = rayPos + rayDir * dstToBox;

                float dstTravelled = 0;

                float volume = 0;

                float4 col = float4(0, 0, 0, 0);

                int iterations = MAX_STEP_COUNT;
                float stepSize = dstLimit / iterations;

                int count = 0;

               
               for(int t=0;t<iterations;t++)
                {
                    rayPos = entryPoint + rayDir * dstTravelled;

                    float3 uvw = remap3(rayPos, boundMin, boundMax, float3(-0.01, -0.01, -0.01), float3(1.01, 1.01, 1.01));

                    if (!isOut(uvw,0.05))
                    {
                        float4 vol= tex3D(_V3d, uvw);
                        vol.a *= _Alpha;
                        float a = vol.a*vol.a;
                        float add = step(_Threshold,a);
                        col = BlendUnder(col, vol)*add+col*(1-add);

                        volume += a*add;

                        count++;

                        if (volume > _MaxDensity)
                        {
                            break;
                        }
                    }

                    

                    if (dstTravelled > depth)
                    {
                        break;
                    }
                    dstTravelled += stepSize;

                   
                }

               fixed4 mainCol = tex2D(_MainTex, i.uv);

                if (volume > 0) 
                {
                    float thickness = dstTravelled/dstLimit;

                    col /= volume;

                    //col.a = thickness;

                    //col = lerp(col, float4(0, 0, 0, col.a), thickness);

                    col.a *= thickness;

                    mainCol = BlendUnder(col, mainCol);

                    //mainCol += col;
                }

                return mainCol;
            }
            ENDCG
        }
    }
}
