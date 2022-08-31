// This shader fills the mesh shape with a color predefined in the code.
Shader "TheHungrySwans/Grid"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { }

        // The SubShader block containing the Shader code. 
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

            #define MAX_GRID_TILE_COUNT 1024
            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv : TEXCOORD;
            };

            float _GridWidth;
            float _GridHeight;
            float4 _GridCellUVSize;
            half _GridCellStates[MAX_GRID_TILE_COUNT];
            float4 _GridCellColors[4];

            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv + (_GridCellUVSize.xy * 0.5f);
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                float2 gridCellPosition = IN.uv * _GridCellUVSize.zw;
                //return half4(gridCellPosition.x / _GridWidth, gridCellPosition.y / _GridHeight, 0, 1);
                float gridCellStateIndex = (gridCellPosition.y * _GridWidth);
                int gridCellState = _GridCellStates[(int)gridCellStateIndex];
                half3 gridCellColor = _GridCellColors[gridCellStateIndex % 4].xyz;
                float2 perCellUV = float2(
                    smoothstep(0.0f, _GridCellUVSize.x, IN.uv.x % _GridCellUVSize.x),
                    smoothstep(0.0f, _GridCellUVSize.y, IN.uv.y % _GridCellUVSize.y));
                float2 radialUV = abs(perCellUV - float2(0.5, 0.5));
                float alpha = length(radialUV);
                alpha = saturate(alpha + smoothstep(0.498, 0.5, radialUV.x));
                alpha = saturate(alpha + smoothstep(0.498, 0.5, radialUV.y));
                return half4(gridCellColor, alpha);
            }
            ENDHLSL
        }
    }
}