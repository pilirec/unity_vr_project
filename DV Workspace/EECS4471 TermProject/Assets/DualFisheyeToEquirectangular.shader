Shader "Custom/DualFisheyeToEquirectangular"
{
    Properties
    {
        _MainTex ("Dual Fisheye Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        ZWrite Off
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            #define PI 3.14159265359

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // This fragment function converts the equirectangular (spherical) UVs into
            // a sample point in the dual-fisheye input. The left fisheye image occupies the left half of the texture,
            // and the right fisheye image occupies the right half.
            fixed4 frag(v2f i) : SV_Target
            {
                // Convert equirectangular UV into spherical angles.
                // Theta (longitude) in range [-PI, PI]
                float theta = i.uv.x * 2.0 * PI - PI;
                // Phi (latitude) in range [PI/2, -PI/2] (top to bottom)
                float phi = (0.5 - i.uv.y) * PI;

                // Convert spherical coordinates to a 3D direction vector.
                float3 dir;
                dir.x = cos(phi) * sin(theta);
                dir.y = sin(phi);
                dir.z = cos(phi) * cos(theta);
                dir = normalize(dir);

                // Assume the left fisheye lens is aimed along (0,0,1)
                // and the right fisheye lens is aimed along (0,0,-1).
                float dotLeft = dot(dir, float3(0,0,1));
                float dotRight = dot(dir, float3(0,0,-1));

                float2 fisheyeUV;
                if (dotLeft >= dotRight)
                {
                    // Use left fisheye.
                    float alpha = acos(saturate(dotLeft));
                    // If outside the 90° FOV, discard (return black).
                    if(alpha > (PI/2.0))
                        discard;

                    // Project onto the lens plane:
                    float3 proj = normalize(dir - float3(0,0,1) * dotLeft);
                    float phi_fish = atan2(proj.y, proj.x);
                    float r = alpha / (PI/2.0); // normalized radius in [0,1]

                    // Convert polar to UV for the left lens.
                    // The left fisheye image occupies the left half of the texture (U from 0 to 0.5).
                    float u_fish = 0.5 * r * cos(phi_fish) + 0.5;
                    float v_fish = 0.5 * r * sin(phi_fish) + 0.5;
                    fisheyeUV = float2(u_fish * 0.5, v_fish);
                }
                else
                {
                    // Use right fisheye.
                    float alpha = acos(saturate(dotRight));
                    if(alpha > (PI/2.0))
                        discard;

                    float3 proj = normalize(dir - float3(0,0,-1) * dotRight);
                    float phi_fish = atan2(proj.y, proj.x);
                    float r = alpha / (PI/2.0);

                    float u_fish = 0.5 * r * cos(phi_fish) + 0.5;
                    float v_fish = 0.5 * r * sin(phi_fish) + 0.5;
                    // Offset U by 0.5 since the right fisheye image occupies the right half.
                    fisheyeUV = float2(0.5 + u_fish * 0.5, v_fish);
                }
                
                fixed4 col = tex2D(_MainTex, fisheyeUV);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
