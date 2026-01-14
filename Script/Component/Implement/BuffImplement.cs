using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class BuffImplement : MonoBehaviour, IComponent
    {
        public List<Buff> appliedBuff = new List<Buff>();

        public void CheckBuffCooldown()
        {
            for (int i = 0; i < appliedBuff.Count; ++i)
            {
                var tmp = appliedBuff[i];
                
                var tmpValue = tmp.statusValue;
                if (tmp.markExecuted)
                {
                    continue;
                }
                if (tmpValue.remainTime < 0f)
                {
                    tmp.markExecuted = true;
                    appliedBuff[i] = tmp;
                    continue;
                }
                tmpValue.remainTime -= Time.deltaTime;
                tmp.statusValue = tmpValue;
                appliedBuff[i] = tmp;
            }
        }

        public void BuffImpl(out List<BuffData> buffDatas)
        {
            buffDatas = new List<BuffData>();
            foreach (var d in appliedBuff)
            {
                if (d.markExecuted)
                {
                    buffDatas.Add(d.statusValue);
                }
            }

            appliedBuff.RemoveAll(x => x.markExecuted);
        }

        public void InitializeComponent()
        {
            
        }
    }
}
