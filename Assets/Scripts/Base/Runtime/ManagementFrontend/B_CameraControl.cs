using System.Collections.Generic;
using Base.UI;
using JetBrains.Annotations;
using UnityEngine;
namespace Base {
    public static class B_CameraControl {
        
        private static CameraFunctions _cameraFunctions;

        public static void Setup(CameraFunctions cameraFunctions) {
            _cameraFunctions = cameraFunctions;
            _cameraFunctions.ManagerStrapping();
        }

        public static void CameraSetAll(this ActiveVirtualCameras cameras, Transform target) => _cameraFunctions.VirtualCameraSetAll(cameras, target);

        public static void CameraSetFollow(this ActiveVirtualCameras cameras, Transform target) => _cameraFunctions.VrtualCameraSetFollow(cameras, target);

        public static void CameraSetAim(this ActiveVirtualCameras cameras, Transform target) => _cameraFunctions.VirtualCameraSetAim(cameras, target);

        public static void SwitchCamera(this ActiveVirtualCameras cameras, [CanBeNull] Transform target = null) => _cameraFunctions.SwitchToCamera(cameras, target);

        public static void ShakeCamera(this ActiveVirtualCameras cameras, float amp, float time) => _cameraFunctions.VirtualCameraShake(cameras, amp, time);

        public static void ChangeCameraFow(this ActiveVirtualCameras cameras, float fow, float time) => _cameraFunctions.ChangeCameraFOW(cameras, fow, time);

    }
}