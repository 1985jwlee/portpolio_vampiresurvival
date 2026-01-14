using System;
using System.Collections.Generic;
using Client;
using Game.Timeline;
using UnityEngine;

namespace Game.ECS
{
    
    public struct StageLevelTimelineData : IMainKeyData
    {
        public string index;
        public string time;
        public string type;
        public string ec1_monsterid;
        public string ec1_cooltime;
        public string ec1_minCreate;
        public string ec1_maxCreate;
        public string ec1_minEntire;
        public string ec1_maxEntire;
        public string ec2_monsterid;
        public string ec2_cooltime;
        public string ec2_minCreate;
        public string ec2_maxCreate;
        public string ec2_minEntire;
        public string ec2_maxEntire;
        public string ec3_monsterid;
        public string ec3_cooltime;
        public string ec3_minCreate;
        public string ec3_maxCreate;
        public string ec3_minEntire;
        public string ec3_maxEntire;
        public string waveType;
        public string wave_monsterId;
        public string wave_num;
        public string circle_radius;
        public string circle_angleOffset;
        public string linear_distance;
        public string linear_length;
        public string linear_angle;
        public string area_x;
        public string area_y;
        public string area_width;
        public string area_height;

        public string KeyField => index;
        
        public void OnReadData(CSVReader reader)
        {
            var idx = 0;
            index = reader[idx++];
            time= reader[idx++];
            type= reader[idx++];
            ec1_monsterid= reader[idx++];
            ec1_cooltime= reader[idx++];
            ec1_minCreate= reader[idx++];
            ec1_maxCreate= reader[idx++];
            ec1_minEntire= reader[idx++];
            ec1_maxEntire= reader[idx++];
            ec2_monsterid= reader[idx++];
            ec2_cooltime= reader[idx++];
            ec2_minCreate= reader[idx++];
            ec2_maxCreate= reader[idx++];
            ec2_minEntire= reader[idx++];
            ec2_maxEntire= reader[idx++];
            ec3_monsterid= reader[idx++];
            ec3_cooltime= reader[idx++];
            ec3_minCreate= reader[idx++];
            ec3_maxCreate= reader[idx++];
            ec3_minEntire= reader[idx++];
            ec3_maxEntire= reader[idx++];
            waveType= reader[idx++];
            wave_monsterId= reader[idx++];
            wave_num= reader[idx++];
            circle_radius= reader[idx++];
            circle_angleOffset= reader[idx++];
            linear_distance= reader[idx++];
            linear_length= reader[idx++];
            linear_angle= reader[idx++];
            area_x= reader[idx++];
            area_y= reader[idx++];
            area_width= reader[idx++];
            area_height= reader[idx++];
        }
    }
    
#if UNITY_EDITOR
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
#endif

    public class StageLevelTimelineEntity : MainKeyEntityImpl
    {
        public enum TimelineEventType
        {
            EnemyCreation, Wave, BossBattle
        }
        
        public int index;
        public float time;
        public TimelineEventType timelineEventType;
        
        public List<MonsterCreateMarkerData> monsterCreateMarker = new();
        public List<WaveCreateMarkerData> waveCreateMarker = new();

        public StageLevelTimelineEntity(StageLevelTimelineData src) : base(src)
        {
            int.TryParse(src.index, out index);
            float.TryParse(src.time, out time);
            System.Enum.TryParse(src.type, out timelineEventType);
            
            if (string.IsNullOrEmpty(src.ec1_monsterid) == false)
            {
                monsterCreateMarker.Add(new MonsterCreateMarkerData(src.ec1_monsterid, src.ec1_cooltime, src.ec1_minCreate, src.ec1_maxCreate, src.ec1_minEntire, src.ec1_maxEntire));
            }

            if (string.IsNullOrEmpty(src.ec2_monsterid) == false)
            {
                monsterCreateMarker.Add(new MonsterCreateMarkerData(src.ec2_monsterid, src.ec2_cooltime, src.ec2_minCreate, src.ec2_maxCreate, src.ec2_minEntire, src.ec2_maxEntire));
            }
            
            if (string.IsNullOrEmpty(src.ec3_monsterid) == false)
            {
                monsterCreateMarker.Add(new MonsterCreateMarkerData(src.ec3_monsterid, src.ec3_cooltime, src.ec3_minCreate, src.ec3_maxCreate, src.ec3_minEntire, src.ec3_maxEntire));
            }

            if (string.IsNullOrEmpty(src.waveType) == false)
            {
                WaveCreateMarkerData data = new ()
                {
                    monsterId = src.wave_monsterId
                };
                int.TryParse(src.wave_num, out data.num);
                Enum.TryParse(src.waveType, out data.type);
                switch (data.type)
                {
                    case WaveCreateMarkerData.Type.Area:
                        {
                            float.TryParse(src.area_x, out var x);
                            float.TryParse(src.area_y, out var y);
                            float.TryParse(src.area_width, out var w);
                            float.TryParse(src.area_height, out var h);
                            data.rect = new Rect(x,y,w,h);
                        }
                        break;
                    case WaveCreateMarkerData.Type.Circle:
                        {
                            float.TryParse(src.circle_radius, out data.radius);
                            float.TryParse(src.circle_angleOffset, out data.angleOffset);
                        }
                        break;
                    case WaveCreateMarkerData.Type.Linear:
                        {
                            float.TryParse(src.linear_distance, out data.distance);
                            float.TryParse(src.linear_length, out data.length);
                            float.TryParse(src.linear_angle, out data.angle);
                        }
                        break;
                }
                waveCreateMarker.Add(data);
            }
        }
    }

    public class StageLevelTimelineCollection : EntityCollectionImpl<StageLevelTimelineEntity>
    {
        public StageLevelTimelineCollection(List<StageLevelTimelineEntity> list)
        {
            foreach (var e in list)
            {
                entities.Add(e.MainKey, e);
            }
        }
    }
}
