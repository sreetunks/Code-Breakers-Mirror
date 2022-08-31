using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GridRendererFeature : ScriptableRendererFeature
{
    class GridRenderPass : ScriptableRenderPass
    {
        public static int _gridWidthShaderProperty;
        public static int _gridHeightShaderProperty;
        public static int _gridCellUVSizeShaderProperty;
        public static int _gridCellStatesShaderProperty;
        public static int _gridCellColorsShaderProperty;

        List<float> gridCellStateList = new List<float>();

        Vector4[] gridCellStateColors = new Vector4[4]
        {
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.5f, 1.0f),
            new Vector4(0.0f, 0.5f, 0.3f, 1.0f),
            new Vector4(0.3f, 0.0f, 0.0f, 1.0f)
        };

        public GridRenderPass()
        {
            _gridWidthShaderProperty = Shader.PropertyToID("_GridWidth");
            _gridHeightShaderProperty = Shader.PropertyToID("_GridHeight");
            _gridCellUVSizeShaderProperty = Shader.PropertyToID("_GridCellUVSize");
            _gridCellStatesShaderProperty = Shader.PropertyToID("_GridCellStates");
            _gridCellColorsShaderProperty = Shader.PropertyToID("_GridCellColors");

            Shader.SetGlobalVectorArray(_gridCellColorsShaderProperty, gridCellStateColors);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            gridCellStateList.Clear();
            LevelGrid levelGrid = GridSystem.ActiveLevelGrid;
            if (levelGrid)
            {
                foreach (GridCellState state in levelGrid.GridCellStates)
                    gridCellStateList.Add((float)state);
                Shader.SetGlobalFloat(_gridWidthShaderProperty, levelGrid.GridWidth);
                Shader.SetGlobalFloat(_gridHeightShaderProperty, levelGrid.GridHeight);
                Shader.SetGlobalVector(_gridCellUVSizeShaderProperty, new Vector4(
                    levelGrid.GridCellSize / levelGrid.GridWidth,
                    levelGrid.GridCellSize / levelGrid.GridHeight,
                    levelGrid.GridWidth / levelGrid.GridCellSize,
                    levelGrid.GridHeight / levelGrid.GridCellSize));
                Shader.SetGlobalFloatArray(_gridCellStatesShaderProperty, gridCellStateList);
            }
            else
            {
                Shader.SetGlobalInt(_gridWidthShaderProperty, 0);
                Shader.SetGlobalInt(_gridHeightShaderProperty, 0);
                Shader.SetGlobalVector(_gridCellUVSizeShaderProperty, Vector4.one);
                Shader.SetGlobalFloatArray(_gridCellStatesShaderProperty, gridCellStateList);
            }

            var drawingSettings = CreateDrawingSettings(new ShaderTagId("Grid"), ref renderingData, SortingCriteria.CommonTransparent);
            var filteringSettings = FilteringSettings.defaultValue;
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    GridRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new GridRenderPass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


