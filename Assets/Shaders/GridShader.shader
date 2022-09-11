Shader "TheHungrySwans/Grid"
{
    Properties
    {
        _HighlightColor("Highlight Color", Color) = (0,1,0,1)
    }
 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Tags { "LightMode" = "Grid" }
            Blend SrcAlpha OneMinusSrcAlpha
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD0;
                nointerpolation float cellState : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            half4 _HighlightColor;
            float4 _GridCellUVSize;
            float4 _GridHighlightInfo;
            float4 _GridCellColors[8];

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = _GridCellColors[IN.cellState];
                OUT.color.a = 1 - frac(IN.cellState);
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                float2 perCellUV = float2(
                    smoothstep(0.0f, _GridCellUVSize.x, IN.uv.x % _GridCellUVSize.x),
                    smoothstep(0.0f, _GridCellUVSize.y, IN.uv.y % _GridCellUVSize.y));
                float2 radialUV = abs(perCellUV - float2(0.5, 0.5));
                float alpha = (radialUV.x * radialUV.x) + (radialUV.y * radialUV.y);// length(radialUV);
                alpha = saturate(alpha + smoothstep(0.498, 0.5, radialUV.x));
                alpha = saturate(alpha + smoothstep(0.498, 0.5, radialUV.y));
                half4 outColor =  half4(IN.color.xyz, lerp(alpha, smoothstep(0.5, 1, IN.color.a), 1 - sign(length(IN.color.xyz))));

                float2 gridCellPosition = float2((int)(IN.uv.x / _GridCellUVSize.x), (int)(IN.uv.y / _GridCellUVSize.y));
                float distance = abs(gridCellPosition.x - _GridHighlightInfo.z) + abs(gridCellPosition.y - _GridHighlightInfo.w);
                float inRange = smoothstep(0, _GridHighlightInfo.y + 1, distance);
                float lerpFactor = sign(1 - inRange) * sign(inRange);
                half4 highlightColor = lerp(outColor, half4(_HighlightColor.xyz, lerp(alpha, smoothstep(0.5, 1, IN.color.a), 1 - sign(length(IN.color.xyz)))), lerpFactor);

                return lerp(outColor, highlightColor, _GridHighlightInfo.x);
            }
            ENDHLSL
        }
    }
}