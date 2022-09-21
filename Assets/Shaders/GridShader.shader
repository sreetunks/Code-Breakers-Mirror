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
                float4 positionOS                       : POSITION;
                float2 uv                               : TEXCOORD0;
                nointerpolation float cellState         : TEXCOORD1;
                nointerpolation float pathFindingState  : TEXCOORD2;
            };

            struct Varyings
            {
                float4 positionHCS      : SV_POSITION;
                float2 uv               : TEXCOORD0;
                float cellState         : TEXCOORD1;
                float pathFindingState  : TEXCOORD2;
                float4 color            : COLOR;
            };

            half4 _HighlightColor;
            float4 _GridCellUVSize;
            float4 _GridHighlightInfo;
            float4 _GridCellColors[9];

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.cellState = IN.cellState;
                OUT.pathFindingState = IN.pathFindingState;
                OUT.color = _GridCellColors[IN.cellState];
                OUT.color.a = 1 - frac(IN.cellState);
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                float2 radialUV = abs(IN.uv - float2(0.5, 0.5));
                float alpha = (radialUV.x * radialUV.x) + (radialUV.y * radialUV.y);// length(radialUV);
                alpha = saturate(alpha + smoothstep(0.48, 0.5, radialUV.x));
                alpha = saturate(alpha + smoothstep(0.48, 0.5, radialUV.y));
                half4 outColor =  half4(IN.color.xyz, lerp(alpha, smoothstep(0.5, 1, IN.color.a), 1 - sign(length(IN.color.xyz))));

                float lerpFactor = saturate(IN.pathFindingState);
                half4 highlightColor = lerp(outColor, half4(_HighlightColor.xyz, lerp(alpha, smoothstep(0.5, 1, IN.color.a), 1 - sign(length(IN.color.xyz)))), lerpFactor);

                highlightColor = lerp(highlightColor, outColor, smoothstep(0, 0.5, saturate(IN.cellState - 8)));

                return lerp(outColor, highlightColor, _GridHighlightInfo.x);
            }
            ENDHLSL
        }
    }
}