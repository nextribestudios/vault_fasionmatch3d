Shader "Custom/LinearFill"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        [Enum(FillDirection)] _FillDirection ("Fill Direction", Float) = 0 // 0: Horizontal, 1: Vertical
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FillAmount;
            float _FillDirection;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Check fill direction and apply filling
                if (_FillDirection == 0) // Horizontal
                {
                    if (i.uv.x > _FillAmount)
                        discard; // Discard pixels outside the fill area
                }
                else if (_FillDirection == 1) // Vertical
                {
                    if (i.uv.y > _FillAmount)
                        discard; // Discard pixels outside the fill area
                }

                return col;
            }
            ENDCG
        }
    }
}
