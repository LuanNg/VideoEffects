﻿using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;

namespace NickDarvey.VideoEffects
{
    public sealed class ZoomVideoEffect : IBasicVideoEffect
    {
        private readonly ZoomMediaExtension MediaExtension = new ZoomMediaExtension();

        /// <summary>
        /// Gets a value indicating whether the video effect will modify the contents of the input frame.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Gets the encoding properties supported by the custom video effect.
        /// </summary>
        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties { get; } = new List<VideoEncodingProperties>
            {  new VideoEncodingProperties { Subtype = "ARGB32" } };

        /// <summary>
        /// Gets a value that indicates whether the custom video effect supports the use of GPU memory or CPU memory.
        /// </summary>
        public MediaMemoryTypes SupportedMemoryTypes { get; } = MediaMemoryTypes.Gpu;

        private ICanvasImage BackgroundImage { get; set; } = default(ICanvasImage);

        public void ProcessFrame(ProcessVideoFrameContext context)
        {
            using (var inputBitmap = CanvasBitmap.CreateFromDirect3D11Surface(MediaExtension.Device, context.InputFrame.Direct3DSurface))
            using (var renderTarget = CanvasRenderTarget.CreateFromDirect3D11Surface(MediaExtension.Device, context.OutputFrame.Direct3DSurface))
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Transform = MediaExtension.CreateTransform(inputBitmap);
                ds.DrawImage(inputBitmap);
            }
        }

        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device) =>
           MediaExtension.SetEncodingProperties(encodingProperties, device);

        public void Close(MediaEffectClosedReason reason)
        {
            BackgroundImage?.Dispose();
            MediaExtension.Close(reason);
        }

        public void DiscardQueuedFrames() =>
            MediaExtension.DiscardQueuedFrames();

        public bool TimeIndependent =>
            MediaExtension.TimeIndependent;

        public void SetProperties(IPropertySet configuration)
        {
            MediaExtension.SetProperties(configuration);
        }
    }
}
