Shader "Unlit/MirrorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // Setting Flipped Axis to 0 means flipping X axis, 1 means flipping Y axis 
        _FlippedAxis ("Flipped Axis", int) = 0
        

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog not! work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _FlippedAxis;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 flippedXuv = float2(1-i.screenPos.x/ i.screenPos.w,i.screenPos.y/ i.screenPos.w) ;
                float2 flippedYuv = float2(i.screenPos.x/ i.screenPos.w,1-i.screenPos.y/ i.screenPos.w) ;
                fixed4 col = tex2D(_MainTex, flippedXuv *(1- _FlippedAxis) + flippedYuv *_FlippedAxis);
                return col;
            }
            ENDCG
        }
    }
}
