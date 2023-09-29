Shader "Unlit/FillOutSideFrame"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _TopColor("Top Color", Color) = (1,0,0,1)
        _BottomColor("Bottom Color", Color) = (0.5,0,0,1)
        [HideInInspector] _ViewRect("View Rect", Vector) = (0.1,0.1,0.9,0.9)
    }
    SubShader
    {
        Tags {
            "RenderType"="TransparentCutout"
            "Queue"="Background"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite On
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            half4 _TopColor;
            half4 _BottomColor;
            half4 _ViewRect;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.z = 0.01;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = lerp(_BottomColor, _TopColor, (o.vertex.y+1)*0.5);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed canView = step(_ViewRect.x, i.vertex.x)
                    * step(_ViewRect.y, i.vertex.y)
                    * (1 - step(_ViewRect.z, i.vertex.x))
                    * (1 - step(_ViewRect.w, i.vertex.y));
                clip(0.5- canView);
                return  col * i.color;
            }
            ENDCG
        }
    }
}
