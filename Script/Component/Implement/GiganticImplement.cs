using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class GiganticImplement : MonoBehaviour, IComponent
    {
        public GiganticTimeData giganticTimeProperty;
        public CameraFollowRateData cameraFollowProperty;
        public EarthShakeCounterData earthShakeCounterProperty;

        public Vector3 CalcCameraOffset(float offsetOfSpritePivot) => CalcCenterOffset(offsetOfSpritePivot) * cameraFollowProperty.statusValue;
        public Vector3 CalcCenterOffset(float offsetOfSpritePivot) => Vector3.up * -offsetOfSpritePivot * (1 + GameSettings.GiganticScaleAdd);

        public void InitializeComponent()
        {
        }
    }

    [Serializable]
    public struct GiganticTimeData : IStatusValue<float>
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct CameraFollowRateData : IStatusValue<float>
    {
        [SerializeField] private float value;
        public float statusValue { get => value; set => this.value = value; }
    }

    [Serializable]
    public struct EarthShakeCounterData : IStatusValue<int>
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }
}
