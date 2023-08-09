using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Assertions;

namespace UnityEngine.XR.ARKit
{
    sealed class ARKitImageDatabase : MutableRuntimeReferenceImageLibrary
    {
        internal IntPtr self { get; }

        internal const string resourceGroupName = "ARReferenceImages";

        ~ARKitImageDatabase()
        {
            Assert.AreNotEqual(self, IntPtr.Zero);

            // Release references
            var n = count;
            for (var i = 0; i < n; ++i)
            {
                GetReferenceImage(self, i).Dispose();
            }

            NSObject.Release(self);
        }

        IntPtr CreateImageDatabase(XRReferenceImageLibrary library)
        {
            if (!Api.AtLeast11_3())
                throw new NotSupportedException($"Image libraries require iOS 11.3 or newer.");

            if (library == null)
            {
                return Init();
            }

            using var managedReferenceImages = library.ToNativeArray(Allocator.Temp);
            using var bundle = library.GetNSBundle();
            if (bundle == null)
                throw new InvalidOperationException($"Could not create reference image library '{library.name}'. Unable to create resource bundle.");

            using var groupName = library.GetARResourceGroupName();
            using var referenceImages = ARReferenceImage.GetReferenceImagesInGroupNamed(groupName, bundle);
            if (referenceImages.Count != managedReferenceImages.Length)
                throw new InvalidOperationException($"The number of images in the {nameof(XRReferenceImageLibrary)} named '{library.name}' ({library.count}) does nat match the number of images in the native reference image data ({referenceImages.Count}). The {nameof(XRReferenceImageLibrary)} may need to be re-exported for iOS.");

            return InitWithImages(referenceImages.AsIntPtr(), managedReferenceImages.AsNativeView());
        }

        public ARKitImageDatabase(XRReferenceImageLibrary serializedLibrary) =>
            self = CreateImageDatabase(serializedLibrary);

        /// <summary>
        /// (Read Only) Whether image validation is supported. `True` on iOS 13 and later.
        /// </summary>
        public override bool supportsValidation => Api.AtLeast13_0();

        protected override AddReferenceImageJobState ScheduleAddImageWithValidationJobImpl(
            NativeSlice<byte> imageBytes,
            Vector2Int sizeInPixels,
            TextureFormat format,
            XRReferenceImage referenceImage,
            JobHandle inputDeps)
        {
            if (!referenceImage.specifySize)
                throw new InvalidOperationException("ARKit requires physical dimensions for all reference images.");

            // Add a reference to keep the native object alive even if we get finalized while a job is running
            var convertedImage = new NativeArray<byte>();
            var validator = IntPtr.Zero;
            try
            {
                NSObject.Retain(self);

                // Schedules a job to convert the image bytes if necessary
                convertedImage = GetImageBytesToConvert(imageBytes, sizeInPixels, ref format, ref inputDeps);

                // Create a native object to contain the validation status
                validator = CreateValidator(self);

                inputDeps = new AddImageJob
                {
                    image = convertedImage.IsCreated ? new NativeSlice<byte>(convertedImage) : imageBytes,
                    database = self,
                    validator = validator,
                    width = sizeInPixels.x,
                    height = sizeInPixels.y,
                    physicalWidth = referenceImage.size.x,
                    format = format,
                    managedReferenceImage = new ManagedReferenceImage(referenceImage)
                }.Schedule(inputDeps);
            }
            catch
            {
                DestroyValidator(self, validator);
                throw;
            }
            finally
            {
                // Release our native counterpart that we previously retained
                inputDeps = Release(inputDeps);

                // If we had to perform a conversion, then release that memory
                if (convertedImage.IsCreated)
                {
                    inputDeps = convertedImage.Dispose(inputDeps);
                }
            }

            return CreateAddJobState(validator, inputDeps);
        }

        protected override AddReferenceImageJobStatus GetAddReferenceImageJobStatus(AddReferenceImageJobState handle)
            => GetValidatorStatus((IntPtr)handle);

        NativeArray<byte> GetImageBytesToConvert(
            NativeSlice<byte> imageBytes, Vector2Int sizeInPixels, ref TextureFormat format, ref JobHandle inputDeps)
        {
            // RGBA32 is not supported by CVPixelBuffer, but ARGB32 is, so
            // we offer a conversion for this common case.
            if (format == TextureFormat.RGBA32)
            {
                int numPixels = sizeInPixels.x * sizeInPixels.y;
                var argb32ImageBytes = new NativeArray<byte>(
                    numPixels * 4,
                    Allocator.Persistent,
                    NativeArrayOptions.UninitializedMemory);

                inputDeps = new ConvertRGBA32ToARGB32Job
                {
                    rgbaImage = imageBytes.SliceConvert<uint>(),
                    argbImage = argb32ImageBytes.Slice().SliceConvert<uint>()
                }.Schedule(numPixels, 64, inputDeps);

                // Format is now ARGB32
                format = TextureFormat.ARGB32;

                return argb32ImageBytes;
            }

            // No conversion necessary; echo back inputs
            return default;
        }

        // Just forwards the call to ScheduleAddImageWithValidationJobImpl
        protected override JobHandle ScheduleAddImageJobImpl(
            NativeSlice<byte> imageBytes,
            Vector2Int sizeInPixels,
            TextureFormat format,
            XRReferenceImage referenceImage,
            JobHandle inputDeps) =>
            ScheduleAddImageWithValidationJobImpl(imageBytes, sizeInPixels, format, referenceImage, inputDeps).jobHandle;

        static readonly TextureFormat[] k_SupportedFormats =
        {
            TextureFormat.Alpha8,
            TextureFormat.R8,
            TextureFormat.R16,
            TextureFormat.RFloat,
            TextureFormat.RGB24,
            TextureFormat.RGBA32,
            TextureFormat.ARGB32,
            TextureFormat.BGRA32,
        };

        public override int supportedTextureFormatCount => k_SupportedFormats.Length;

        protected override TextureFormat GetSupportedTextureFormatAtImpl(int index) => k_SupportedFormats[index];

        protected override XRReferenceImage GetReferenceImageAt(int index)
        {
            Assert.AreNotEqual(self, IntPtr.Zero);
            return GetReferenceImage(self, index).ToReferenceImage();
        }

        public override int count => GetReferenceImageCount(self);

        JobHandle Release(JobHandle inputDeps) => NSObject.Dispose(self, inputDeps);

        struct ConvertRGBA32ToARGB32Job : IJobParallelFor
        {
            public NativeSlice<uint> rgbaImage;

            public NativeSlice<uint> argbImage;

            public unsafe void Execute(int index)
            {
                uint rgba = rgbaImage[index];
                argbImage[index] = (rgba << 8) | ((rgba & 0xff000000) >> 24);
            }
        }

        struct AddImageJob : IJob
        {
            public NativeSlice<byte> image;
            public IntPtr database;
            public IntPtr validator;
            public int width;
            public int height;
            public float physicalWidth;
            public TextureFormat format;
            public ManagedReferenceImage managedReferenceImage;

            public unsafe void Execute()
            {
                if (!AddImage(database, validator, image.GetUnsafePtr(), format, width, height, physicalWidth, ref managedReferenceImage))
                {
                    managedReferenceImage.Dispose();
                }
            }

#if UNITY_XR_ARKIT_LOADER_ENABLED && !UNITY_EDITOR
            [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_AddImage")]
            static extern unsafe bool AddImage(
                IntPtr database, IntPtr validator, void* bytes, TextureFormat format,
                int width, int height, float physicalWidth,
                ref ManagedReferenceImage managedReferenceImage);
#else
            static unsafe bool AddImage(
                IntPtr database, IntPtr validator, void* bytes, TextureFormat format,
                int width, int height, float physicalWidth,
                ref ManagedReferenceImage managedReferenceImage)
            {
                throw new NotImplementedException("Apple ARKit XR Plug-in Provider not enabled in project settings.");
            }
#endif
        }

#if UNITY_XR_ARKIT_LOADER_ENABLED && !UNITY_EDITOR
        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_init")]
        static extern IntPtr Init();

        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_initWithImages")]
        static extern IntPtr InitWithImages(IntPtr referenceImages, NativeView managedReferenceImages);

        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_GetReferenceImage")]
        static extern ManagedReferenceImage GetReferenceImage(IntPtr self, int index);

        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_GetReferenceImageCount")]
        static extern int GetReferenceImageCount(IntPtr database);

        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_CreateValidator")]
        static extern IntPtr CreateValidator(IntPtr database);

        [DllImport("__Internal", EntryPoint = "UnityARKit_ReferenceImageValidator_get_status")]
        static extern AddReferenceImageJobStatus GetValidatorStatus(IntPtr validator);

        [DllImport("__Internal", EntryPoint = "UnityARKit_ImageDatabase_DestroyValidator")]
        static extern void DestroyValidator(IntPtr database, IntPtr validator);
#else
        const string k_ExceptionMsg = "Apple ARKit XR Plug-in Provider not enabled in project settings.";

        static IntPtr Init() => default;

        static IntPtr InitWithImages(IntPtr referenceImages, NativeView managedReferenceImages) =>
            throw new NotImplementedException(k_ExceptionMsg);

        static ManagedReferenceImage GetReferenceImage(IntPtr self, int index) =>
            throw new System.NotImplementedException(k_ExceptionMsg);

        static int GetReferenceImageCount(IntPtr database) =>
            throw new System.NotImplementedException(k_ExceptionMsg);

        static IntPtr CreateValidator(IntPtr database) => default;

        static AddReferenceImageJobStatus GetValidatorStatus(IntPtr validator) => default;

        static void DestroyValidator(IntPtr database, IntPtr validator) { }
#endif
    }
}
