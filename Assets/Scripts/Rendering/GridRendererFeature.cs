using Grid;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Rendering
{
    public class GridRendererFeature : ScriptableRendererFeature
    {
        private class GridRenderPass : ScriptableRenderPass
        {
            public static int GridWidthShaderProperty;
            public static int GridHeightShaderProperty;
            public static int GridCellUVSizeShaderProperty;
            public static int GridCellStatesShaderProperty;
            public static int GridCellColorsShaderProperty;

            public GridRenderPass(Vector4[] gridCellStateColors)
            {
                GridWidthShaderProperty = Shader.PropertyToID("_GridWidth");
                GridHeightShaderProperty = Shader.PropertyToID("_GridHeight");
                GridCellUVSizeShaderProperty = Shader.PropertyToID("_GridCellUVSize");
                GridCellStatesShaderProperty = Shader.PropertyToID("_GridCellStates");
                GridCellColorsShaderProperty = Shader.PropertyToID("_GridCellColors");

                Shader.SetGlobalVectorArray(GridCellColorsShaderProperty, gridCellStateColors);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                LevelGrid levelGrid = GridSystem.ActiveLevelGrid;
                if (levelGrid)
                {
                    Shader.SetGlobalFloat(GridWidthShaderProperty, levelGrid.GridWidth);
                    Shader.SetGlobalFloat(GridHeightShaderProperty, levelGrid.GridHeight);
                    Shader.SetGlobalVector(GridCellUVSizeShaderProperty, new Vector4(
                        levelGrid.GridCellSize / levelGrid.GridWidth,
                        levelGrid.GridCellSize / levelGrid.GridHeight,
                        levelGrid.GridWidth / levelGrid.GridCellSize,
                        levelGrid.GridHeight / levelGrid.GridCellSize));
                }
                var drawingSettings = CreateDrawingSettings(new ShaderTagId("Grid"), ref renderingData, SortingCriteria.CommonTransparent);
                var filteringSettings = FilteringSettings.defaultValue;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
        }

        [SerializeField]
        Vector4[] gridCellStateColors = new Vector4[4]
        {
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.5f, 1.0f),
            new Vector4(0.0f, 0.5f, 0.3f, 1.0f),
            new Vector4(0.3f, 0.0f, 0.3f, 1.0f)
        };

        GridRenderPass m_ScriptablePass;

        /// <inheritdoc/>
        public override void Create()
        {
            m_ScriptablePass = new GridRenderPass(gridCellStateColors);
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(!renderingData.cameraData.isPreviewCamera)
                renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}


