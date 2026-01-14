using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.ECS
{
    public class WaitBuffTickImplement : MonoBehaviour, IComponent
    {
        public readonly Dictionary<uint, float> affectBuffSourceEntity = new Dictionary<uint, float>();

        public bool HasWaitBuffTick(uint myEGID)
        {
            var hasBuffTick = affectBuffSourceEntity.ContainsKey(myEGID);
            if (hasBuffTick == false)
            {
                affectBuffSourceEntity.Add(myEGID, 0.3f);
            }

            return hasBuffTick;
        } 

        private bool ScanBuffTick(uint egid)
        {
            if (affectBuffSourceEntity.TryGetValue(egid, out var remainTime))
            {
                remainTime -= Time.deltaTime;
            }
            affectBuffSourceEntity[egid] = remainTime;
            return remainTime >=  0f;
        }

        private void ScanBuffTick()
        {
            var keys = affectBuffSourceEntity.Keys.ToList();
            foreach (uint key in keys)
            {
                if (ScanBuffTick(key) == false)
                {
                    affectBuffSourceEntity.Remove(key);
                }
            }
        }

        private void Update()
        {
            ScanBuffTick();
        }

        public void InitializeComponent()
        {
            
        }
    }
}
