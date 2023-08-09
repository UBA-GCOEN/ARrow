# Magic Leap

This package provides support for developing Magic Leap applications using Unity.
Currently, it provides necessary functionality to enable rendering and spatial mapping
capabilities.

## How to develop this package

### Prerequisites

- git clone the [`unity/xr.sdk.magicleap`](https://github.cds.internal.unity3d.com/xr/sdk/magicleap) repository.
- use the latest 2019.2 installers.
- `cd magicleap; [mono] bee.exe` (on Windows, you can execute bee.exe directly; everywhere else requires you launch it via mono)
- If you have any C# compilation errors, something is wrong.

### How to build this package

We are using Bee to build. You must be in the repository root:

```
> [mono] ./bee
```

If you are a Unity employee, Bee will attempt to automatically download a compatible Android toolset SDK for you.

If not, you will need to set up your Android NDK system paths.
The build tool can discover the NDK at ANDROID_NDK_HOME or ANDROID_NDK_ROOT.
The NDK version should be 21d, but this has been tested with 21e. [Download](https://developer.android.com/ndk/downloads/older_releases)

Ping `@ashley.matheson` or `@ed` if you have questions.