---
uid: arcore-image-tracking
---
# Image tracking

To use image tracking on ARCore, you must create a reference image library. See AR Foundation [Image tracking](xref:arfoundation-image-tracking) for instructions.

When you build the Player for Android, the ARCore build code creates a  `imgdb` file for each reference image library. ARCore creates these files in your project's `StreamingAssets` folder, in a subdirectory called `HiddenARCore`, so Unity can access them at runtime.

You can use .jpg or .png files as AR reference images in ARCore. If a reference image in the `XRReferenceImageLibrary` isn't a .jpg or .png, the ARCore build processor attempts to convert the Texture to a .png so that ARCore can use it.

When you export a  `Texture2D` to .png, it can fail if the Texture's [Texture Import Settings](https://docs.unity3d.com/Manual/class-TextureImporter.html) have **Read/Write Enabled** disabled and **Compression** is set to **None**.

To use the Texture at runtime (not as a source Asset for the reference image), create a separate .jpg or .png copy for the source Asset. This reduces the performance impact of the Texture Import Settings at runtime.

## Reference image dimensions

To improve image detection in ARCore you can specify the image dimensions. When you specify the dimensions for a reference image, ARCore receives the image's width, and then determines the height from the image's aspect ratio.
