Shader "Unlit/UnitStencilMask"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZWrite On
            Stencil
            {
                Ref 2
                Comp Always
                Pass Replace
                Fail Keep
                ZFail IncrSat
            }
        }
    }
}
