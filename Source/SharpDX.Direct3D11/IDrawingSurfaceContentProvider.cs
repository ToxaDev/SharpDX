﻿// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#if WP8
using System;
using System.Runtime.InteropServices;

namespace SharpDX.Direct3D11
{
    [ShadowAttribute(typeof(DrawingSurfaceContentProviderShadow))]
    public partial interface IDrawingSurfaceContentProviderNative
    {
        // IDrawingSurfaceRuntimeHostNative *
        void Connect(DrawingSurfaceRuntimeHost host);

        void Disconnect();

        // _In_  const LARGE_INTEGER *presentTargetTime,
        // DrawingSurfaceSizeF *desiredRenderTargetSize
        /* public void PrepareResources(long resentTargetTimeRef, out SharpDX.Bool isContentDirty) */
        /* public void PrepareResources(long resentTargetTimeRef, ref SharpDX.DrawingSizeF desiredRenderTargetSize) */
        void PrepareResources(DateTime presentTargetTime, ref Bool isContentDirty);

        //_In_  ID3D11Device1 *hostDevice,
        //_In_  ID3D11DeviceContext1 *hostDeviceContext,
        //_In_  ID3D11RenderTargetView *hostRenderTargetView) = 0;
        /* public void GetTexture(SharpDX.DrawingSizeF surfaceSize, out SharpDX.Direct3D11.DrawingSurfaceSynchronizedTexture synchronizedTexture, out SharpDX.RectangleF textureSubRectangle) */
        /* public void Draw(SharpDX.Direct3D11.Device1 hostDevice, SharpDX.Direct3D11.DeviceContext1 hostDeviceContext, SharpDX.Direct3D11.RenderTargetView hostRenderTargetView) */
        void GetTexture(DrawingSizeF surfaceSize, ref DrawingSurfaceSynchronizedTexture synchronizedTexture, ref RectangleF textureSubRectangle);
    }

    public abstract class DrawingSurfaceContentProviderNativeBase : CallbackBase, IDrawingSurfaceContentProviderNative, IInspectable, ICustomQueryInterface
    {
        private IntPtr thisComObjectPtr;

        protected DrawingSurfaceContentProviderNativeBase()
        {
            thisComObjectPtr = CppObjectShadow.ToIntPtr<IDrawingSurfaceContentProviderNative>(this);
        }

        public abstract void Connect(DrawingSurfaceRuntimeHost host);

        public abstract void Disconnect();

        public abstract void PrepareResources(DateTime presentTargetTime, ref Bool isContentDirty);

        public abstract void GetTexture(DrawingSizeF surfaceSize, ref DrawingSurfaceSynchronizedTexture synchronizedTexture, ref RectangleF textureSubRectangle);

        CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out IntPtr ppv)
        {
            ppv = thisComObjectPtr;
            return CustomQueryInterfaceResult.Handled;
        }
    }

    /// <summary>
    /// Internal DrawingSurfaceContentProvider Callback
    /// </summary>
    internal class DrawingSurfaceContentProviderShadow : ComObjectShadow
    {
        private static readonly DrawingSurfaceContentProviderVtbl Vtbl = new DrawingSurfaceContentProviderVtbl();

        private DrawingSurfaceRuntimeHost host;
        //private Device device;
        //private DeviceContext context;
        //private RenderTargetView renderTargetView;
        private DrawingSurfaceSynchronizedTexture synchronizedTexture;

        /// <summary>
        /// Return a pointer to the unamanged version of this callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>A pointer to a shadow c++ callback</returns>
        public static IntPtr ToIntPtr(IDrawingSurfaceContentProviderNative callback)
        {
            return ToIntPtr<IDrawingSurfaceContentProviderNative>(callback);
        }

        public class DrawingSurfaceContentProviderVtbl : ComObjectVtbl
        {
            public DrawingSurfaceContentProviderVtbl()
                : base(4)
            {
                AddMethod(new ConnectDelegate(Connect));
                AddMethod(new DisconnectDelegate(Disconnect));
                AddMethod(new PrepareResourcesDelegate(PrepareResources));
                AddMethod(new GetTextureDelegate(GetTexture));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int ConnectDelegate(IntPtr thisPtr, IntPtr host);
            private static int Connect(IntPtr thisPtr, IntPtr hostPtr)
            {
                try
                {
                    var shadow = ToShadow<DrawingSurfaceContentProviderShadow>(thisPtr);
                    var callback = (IDrawingSurfaceContentProviderNative)shadow.Callback;

                    // Precache values.
                    shadow.host = new DrawingSurfaceRuntimeHost(hostPtr);
                    //shadow.device = new Device(hostDevicePtr);
                    //shadow.context = shadow.device.ImmediateContext;

                    callback.Connect(shadow.host);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }

            /// <unmanaged>HRESULT ID2D1DrawingSurfaceContentProvider::SetComputeInfo([In] ID2D1ComputeInfo* computeInfo)</unmanaged>	
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate void DisconnectDelegate(IntPtr thisPtr);
            private static void Disconnect(IntPtr thisPtr)
            {
                var shadow = ToShadow<DrawingSurfaceContentProviderShadow>(thisPtr);
                var callback = (IDrawingSurfaceContentProviderNative)shadow.Callback;
                callback.Disconnect();
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int PrepareResourcesDelegate(IntPtr thisPtr, IntPtr presentTargetTime, ref Bool isContentDirty);
            private unsafe static int PrepareResources(IntPtr thisPtr, IntPtr presentTargetTime, ref Bool isContentDirty)
            {
                try
                {
                    var shadow = ToShadow<DrawingSurfaceContentProviderShadow>(thisPtr);
                    var callback = (IDrawingSurfaceContentProviderNative)shadow.Callback;
                    callback.PrepareResources(new DateTime(*(long*)presentTargetTime), ref isContentDirty);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }


            
            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetTextureDelegate(IntPtr thisPtr, IntPtr surfaceSize, IntPtr synchronizedTexture, IntPtr textureSubRectangle);
            private unsafe static int GetTexture(IntPtr thisPtr, IntPtr surfaceSize, IntPtr synchronizedTexture, IntPtr textureSubRectangle)
            {
                try
                {
                    var shadow = ToShadow<DrawingSurfaceContentProviderShadow>(thisPtr);
                    var callback = (IDrawingSurfaceContentProviderNative)shadow.Callback;

                    if (surfaceSize == IntPtr.Zero || textureSubRectangle == IntPtr.Zero)
                        throw new ArgumentException();

                    callback.GetTexture(*(DrawingSizeF*)surfaceSize, ref shadow.synchronizedTexture , ref *(RectangleF*) textureSubRectangle);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
                return Result.Ok.Code;
            }
        }

        protected override CppObjectVtbl GetVtbl
        {
            get { return Vtbl; }
        }
    }
}
#endif