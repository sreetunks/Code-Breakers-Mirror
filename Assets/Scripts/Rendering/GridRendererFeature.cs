using System;
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
            private static int _gridCellUVSizeShaderProperty;
            private static int _gridCellStatesShaderProperty;

            public GridRenderPass(Vector4[] gridCellStateColors)
            {
                _gridCellUVSizeShaderProperty = Shader.PropertyToID("_GridCellUVSize");
                _gridCellStatesShaderProperty = Shader.PropertyToID("_GridCellStates");
                var gridCellColorsShaderProperty = Shader.PropertyToID("_GridCellColors");

                Shader.SetGlobalVectorArray(gridCellColorsShaderProperty, gridCellStateColors);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var levelGrid = GridSystem.ActiveLevelGrid;
                if (levelGrid)
                {
                    Shader.SetGlobalVector(_gridCellUVSizeShaderProperty, new Vector4(
                        (levelGrid.GridCellSize * 0.5f) / levelGrid.GridWidth,
                        (levelGrid.GridCellSize * 0.5f) / levelGrid.GridHeight,
                        levelGrid.GridWidth / levelGrid.GridCellSize,
                        levelGrid.GridHeight / levelGrid.GridCellSize));
                }
                var drawingSettings = CreateDrawingSettings(new ShaderTagId("Grid"), ref renderingData, SortingCriteria.CommonTransparent);
                var filteringSettings = FilteringSettings.defaultValue;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
        }

        [SerializeField] private Vector4[] gridCellStateColors = new Vector4[4]
        {
            new(0.0f, 0.0f, 0.0f, 1.0f),
            new(0.0f, 0.0f, 0.5f, 1.0f),
            new(0.0f, 0.5f, 0.3f, 1.0f),
            new(0.3f, 0.0f, 0.3f, 1.0f)
        };

        private GridRenderPass _mScriptablePass;

        /// <inheritdoc/>
        public override void Create()
        {
            _mScriptablePass = new GridRenderPass(gridCellStateColors)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(!renderingData.cameraData.isPreviewCamera)
                renderer.EnqueuePass(_mScriptablePass);
        }
    }
}


