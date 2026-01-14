using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public interface ITableDataIndexNo : IStatusValue<int> { }

    [Serializable]
    public struct TableDataIndexNo : ITableDataIndexNo
    {
        [SerializeField] private int value;
        public int statusValue { get => value; set => this.value = value; }
    }
}
