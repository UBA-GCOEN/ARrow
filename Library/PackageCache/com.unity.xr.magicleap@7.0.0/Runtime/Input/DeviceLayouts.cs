#if UNITY_INPUT_SYSTEM
using System;
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Enum representing the eye calibration status for a MagicLeapLightwear headset.
    /// For use with IntegerControl MagicLeapLightwear.eyeCalibrationStatus
    /// </summary>
    public enum CalibrationStatus
    {
        None = 0,
        Bad,
        Good
    }

    /// <summary>
    /// Enum representing the eye calibration status for a MagicLeapLightwear headset.
    /// For use with MagicLeapController.dof IntegerControl
    /// </summary>
    public enum ControllerDoF
    {
        None = 0,
        DoF3,
        DoF6
    }

    /// <summary>
    /// Enum representing the current controller calibration accuracy
    /// For use with MagicLeapController.calibrationAccuracy IntegerControl
    /// </summary>
    public enum ControllerCalibrationAccuracy
    {
        Bad = 0,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Enum representing the type of Magic Leap Controller being used.
    /// For use with MagicLeapController.type IntegerControl
    /// </summary>
    public enum ControllerType
    {
        None = 0,
        Device
    }

    [Preserve]
    [InputControlLayout(displayName = "MagicLeap Headset")]
    public class MagicLeapLightwear : XRHMD
    {
        public bool ControllerEnabled
        {
            get
            {
                var command = QueryControllerEnabled.Create();
                if (ExecuteCommand(ref command) >= 0)
                    return command.isEnabled;
                return false;
            }
            set
            {
                var command = SetControllerEnabled.Create(value);
                ExecuteCommand(ref command);
            }
        }

        public bool EyesEnabled
        {
            get
            {
                var command = QueryEyesEnabled.Create();
                if (ExecuteCommand(ref command) >= 0)
                    return command.isEnabled;
                return false;
            }
            set
            {
                var command = SetEyesEnabled.Create(value);
                ExecuteCommand(ref command);
            }
        }

        [Preserve]
        [InputControl(displayName = "Confidence Level")]
        public AxisControl confidence { get; private set; }
        [Preserve]
        [InputControl(displayName = "Fixation Point Confidence")]
        public AxisControl fixationPointConfidence { get; private set; }
        [Preserve]
        [InputControl(displayName = "Left Eye Center Confidence")]
        public AxisControl eyeLeftCenterConfidence { get; private set; }
        [Preserve]
        [InputControl(displayName = "Right Eye Center Confidence")]
        public AxisControl eyeRightCenterConfidence { get; private set; }
        [Preserve]
        [InputControl(displayName = "Eyes")]
        public EyesControl eyes { get; private set; }
        [Preserve]
        [InputControl(displayName = "Eye Calibration Status")]
        public IntegerControl eyeCalibrationStatus { get; private set; }


        protected override void FinishSetup()
        {
            base.FinishSetup();

            confidence = GetChildControl<AxisControl>("confidence");
            fixationPointConfidence = GetChildControl<AxisControl>("fixationPointConfidence");
            eyeLeftCenterConfidence = GetChildControl<AxisControl>("eyeLeftCenterConfidence");
            eyeRightCenterConfidence = GetChildControl<AxisControl>("eyeRightCenterConfidence");
            eyes = GetChildControl<EyesControl>("eyes");
            eyeCalibrationStatus = GetChildControl<IntegerControl>("eyeCalibrationStatus");

            EyesEnabled = true;
        }
    }

    [Preserve]
    [InputControlLayout(commonUsages = new[] { "LeftHand", "RightHand" }, displayName = "MagicLeap Hand")]
    public class MagicLeapHandDevice : XRController
    {
        [Preserve]
        [InputControl(displayName = "Hand Confidence")]
        public AxisControl handConfidence { get; private set; }
        [Preserve]
        [InputControl(displayName = "Normalized Center")]
        public Vector3Control normalizeCenter { get; private set; }
        [Preserve]
        [InputControl(displayName = "Wrist Center Point")]
        public Vector3Control wristCenter { get; private set; }
        [Preserve]
        [InputControl(displayName = "Wrist Ulnar Point")]
        public Vector3Control wristUlnar { get; private set; }
        [Preserve]
        [InputControl(displayName = "Wrist Radial Point")]
        public Vector3Control wristRadial { get; private set; }

        //Need Bone control and Hand Control

        protected override void FinishSetup()
        {
            base.FinishSetup();
            handConfidence = GetChildControl<AxisControl>("handConfidence");
            normalizeCenter = GetChildControl<Vector3Control>("normalizeCenter");
            wristCenter = GetChildControl<Vector3Control>("wristCenter");
            wristUlnar = GetChildControl<Vector3Control>("wristUlnar");
            wristRadial = GetChildControl<Vector3Control>("wristRadial");
        }
    }

    [Preserve]
    [InputControlLayout(displayName = "MagicLeap Controller")]
    public class MagicLeapController : XRController
    {
        [Preserve]
        [InputControl(displayName = "Touchpad 1 Pressed")]
        public ButtonControl touchpad1Pressed { get; private set; }
        [Preserve]
        [InputControl(displayName = "Touchpad 1 Position")]
        public Vector2Control touchpad1Position { get; private set; }
        [Preserve]
        [InputControl(displayName = "Touchpad 1 Force")]
        public AxisControl touchpad1Force { get; private set; }

        [Preserve]
        [InputControl(displayName = "Touchpad 2 Pressed")]
        [Obsolete("This feature is no longer available as Magic Leap does not support Secondary Touch.")]
        public ButtonControl touchpad2Pressed { get; private set; }
        [Preserve]
        [InputControl(displayName = "Touchpad 2 Position")]
        [Obsolete("This feature is no longer available as Magic Leap does not support Secondary Touch.")]
        public Vector2Control touchpad2Position { get; private set; }
        [Preserve]
        [InputControl(displayName = "Touchpad 2 Force")]
        [Obsolete("This feature is no longer available as Magic Leap does not support Secondary Touch.")]
        public AxisControl touchpad2Force { get; private set; }
        [Preserve]
        [InputControl(displayName = "Trigger Button")]
        public ButtonControl triggerButton { get; private set; }
        [Preserve]
        [InputControl(displayName = "Trigger Axis")]
        public AxisControl trigger { get; private set; }
        [Preserve]
        [InputControl(displayName = "Bumper Button")]
        public ButtonControl bumperButton { get; private set; }
        [Preserve]
        [InputControl(displayName = "Bumper Axis")]
        public AxisControl bumper { get; private set; }
        [Preserve]
        [InputControl(displayName = "Menu")]
        public ButtonControl menu { get; private set; }
        [Preserve]
        [InputControl(displayName = "Degrees of Freedom")]
        public IntegerControl dof { get; private set; }
        [Preserve]
        [InputControl(displayName = "Calibration Accuracy")]
        public IntegerControl calibrationAccuracy { get; private set; }
        [Preserve]
        [InputControl(displayName = "Input Control Type")]
        public IntegerControl type { get; private set; }


        //Need Discrete State for DOF and Type and CalibrationAccuracy

        public bool StartVibe(VibePattern pattern, VibeIntensity vibeIntensity)
        {
            var command = SendControllerVibe.Create(pattern, vibeIntensity);
            return ExecuteCommand(ref command) >= 0;
        }

        public bool StartLEDPattern(LEDPattern ledPattern, LEDColor ledColor, uint durationMs)
        {
            var command = SendLEDPattern.Create(ledPattern, ledColor, durationMs);
            return ExecuteCommand(ref command) >= 0;
        }

        public bool StartLEDEffect(LEDEffect ledEffect, LEDSpeed ledSpeed, LEDPattern ledPattern, LEDColor ledColor, uint durationMs)
        {
            var command = SendLEDEffect.Create(ledEffect, ledSpeed, ledPattern, ledColor, durationMs);
            return ExecuteCommand(ref command) >= 0;
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            touchpad1Pressed = GetChildControl<ButtonControl>("touchpad1Pressed");
            touchpad1Position = GetChildControl<Vector2Control>("touchpad1Position");
            touchpad1Force = GetChildControl<AxisControl>("touchpad1Force");

            triggerButton = GetChildControl<ButtonControl>("triggerButton");
            trigger = GetChildControl<AxisControl>("trigger");
            bumperButton = GetChildControl<ButtonControl>("bumperbutton");
            bumper = GetChildControl<AxisControl>("bumper");
            menu = GetChildControl<ButtonControl>("menu");

            dof = GetChildControl<IntegerControl>("dof");
            calibrationAccuracy = GetChildControl<IntegerControl>("calibrationAccuracy");
            type = GetChildControl<IntegerControl>("type");
        }
    }
}
#endif
