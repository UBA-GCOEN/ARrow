---
uid: xr-core-utila-whats-new
---
# What's new in version 2.2

This package is replacing XR Tools Utilities package. Summary of changes in XR Core Utilities package version 2.2.

The main updates in this release include:

**Added**

- Added `SwapAtIndices<T>()` function to `ListExtensions` that performs an index-based element swap on any `List<T>`.
- Added bindable variable classes which allow a typed variable to be observed for value changes.
- Added value datum classes, which store data in a `ScriptableObject` or directly within a serializable class. These can be utilized to share common configuration across multiple objects and are used by the affordance system.
- Added common primitive types of `UnityEvent<T>` to allow serialized typed Unity Editor events.
- Added `HashSetList`, which is basically a wrapper for both a `HashSet` and `List` that allows the benefits of O(1) `Contains` checks, while allowing deterministic iteration without allocation.
- Added Multiply, Divide, and SafeDivide Vector3 extensions.
- Added `SetValueWithoutNotify` method to `BindableVariableBase<T>` to let users set the value without broadcasting to subscribers.
- Added `BuildValidationRule.OnClick` lambda function that is invoked when the rule is clicked in the validator. Also added the `BuildValidator.SelectObject` method to perform the object select logic for rules.
- Added `BuildValidator.FixIssues` method to process and fix a batch of validation rules.

**Updated**

- Renamed `UnBindAction` to `UnbindAction` in [EventBinding](xref:Unity.XR.CoreUtils.Bindings.EventBinding).
- The `Fix All` button, in the `Project Validation`, now processes and fixes all issues in a single frame. Set `BuildValidationRule.FixItAutomatic` to `false` if the issue cannot be processed with others in the same frame (Ex. if the fix requires a Unity Editor restart).
