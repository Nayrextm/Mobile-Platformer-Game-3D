Shader "Unlit/TintGlassShader"
{
    Properties
    {
        // Створюємо галочку в Інспекторі
        [Toggle(_USE_TEXTURE)] _UseTex ("Use Texture?", Float) = 0
        
        _MainTex ("Glass Texture (Main Texture Default)", 2D) = "white" {}
        [HDR] _Color ("Glass Base Tint", Color) = (0.1, 0.1, 0.1, 0.5)
        [HDR] _RimColor ("Gloss / Highlight Color", Color) = (1.0, 1.0, 1.0, 0.3)
        _RimPower ("Glass Curvature (Fresnel)", Range(0.5, 8.0)) = 3.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // МАГІЯ ТУТ: Кажемо Unity компілювати дві різні версії коду
            #pragma shader_feature _USE_TEXTURE 
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _Color;
            float4 _RimColor;
            float _RimPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); 
                o.normal = UnityObjectToWorldNormal(v.normal);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // За замовчуванням беремо лише колір
                fixed4 col = _Color;

                // Цей блок коду буде існувати ТІЛЬКИ якщо стоїть галочка в матеріалі!
                #if _USE_TEXTURE
                    fixed4 texColor = tex2D(_MainTex, i.uv);
                    col *= texColor;
                #endif

                // Прораховуємо глянцеві краї
                float3 n = normalize(i.normal);
                float3 v = normalize(i.viewDir);
                float NdotV = abs(dot(n, v)); 
                float fresnel = pow(1.0 - NdotV, _RimPower);

                col.rgb += _RimColor.rgb * fresnel * _RimColor.a;
                col.a = saturate(col.a + fresnel * _RimColor.a);

                return col;
            }
            ENDCG
        }
    }
}