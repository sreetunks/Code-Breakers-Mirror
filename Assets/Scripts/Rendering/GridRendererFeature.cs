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
            private static int _gridHighlightInfoShaderProperty;
            private static int _gridCellColorsShaderProperty;

            private readonly Vector4[] _cellStateColors;

            public GridRenderPass(Vector4[] gridCellStateColors)
            {
                _gridCellUVSizeShaderProperty = Shader.PropertyToID("_GridCellUVSize");
                _gridHighlightInfoShaderProperty = Shader.PropertyToID("_GridHighlightInfo");
                _gridCellColorsShaderProperty = Shader.PropertyToID("_GridCellColors");

                _cellStateColors = gridCellStateColors;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Shader.SetGlobalVectorArray(_gridCellColorsShaderProperty, _cellStateColors);
                var levelGrid = GridSystem.ActiveLevelGrid;
                if (levelGrid)
                {
                    Shader.SetGlobalVector(_gridCellUVSizeShaderProperty, new Vector4(
                        1.0f / levelGrid.GridWidth,
                        1.0f / levelGrid.GridHeight,
                        0,
                        0));
                    Shader.SetGlobalVector(_gridHighlightInfoShaderProperty, new Vector4(
                        (Application.isPlaying && GridSystem.HighlightPosition != GridPosition.Invalid) ? 1 : 0,
                        GridSystem.HighlightRange,
                        GridSystem.HighlightPosition.X,
                        GridSystem.HighlightPosition.Z));
                }
                var drawingSettings = CreateDrawingSettings(new ShaderTagId("Grid"), ref renderingData, SortingCriteria.CommonTransparent);
                var filteringSettings = FilteringSettings.defaultValue;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
        }

        [SerializeField] private Vector4[] gridCellStateColors;

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


