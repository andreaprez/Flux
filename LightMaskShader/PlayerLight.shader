	Shader "BrokenRib/PlayerLightShader"
{
	Properties
	{
		_MainTex ("Black Texture", 2D) = "white" {}
		_MaskTex ("Mask texture", 2D) = "white" {}
		_AlphaValue("Alpha Value", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Cutout" }

		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};	
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _AlphaValue;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = lerp(tex2D(_MainTex, i.uv), tex2D(_MaskTex, i.uv), _AlphaValue);
				return col;
			}
			ENDCG
		}
	}
}