using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game.ECS
{
    public static class GameSettings
    {
        public const float DefaultSpeedValue = 5f;

        public const float DefaultDeviceProbability = 100;
        public const float OwnedDeviceProbability = 150;
        public const float AdvanceDeviceProbability = 300;
        public const float DefaultKnockBackDistance = 0.1f;
        public const float DefaultKnockBackVelocity = 10;
        public const float DefaultKnockBackDuration = 0.2f;
        public const float DefaultHitTintTime = 0.2f;
        public const float DefaultCakeChance = 0.01f;
        public const float DefaultMagnetChance = 0.01f;
        public const float DefaultDeviceBoxChance = 0.01f;
        public const float DefaultGoldChance = 0.01f;
        public const float DefaultBombChance = 0.01f;
        public const float DefaultGoldFeverChance = 0.01f;
        public const float DefaultGiganticChance = 0.01f;
        public const float DefaultMicrifyChance = 0.01f;
        public const float TokenCatchAnimDistance = 0.5f;
        public const float TokenCatchAnimSpeed = 0.5f;
        public const float MagnetDistance = 999f;
        public const float BombDistance = 10f;
        public const float GiganticEarthShakeDistance = 10f;
        public const float FiveDeviceProbability = 0.1f;
        public const float ThreeDeviceProbability = 0.3f;

        public const float GoldFeverTime = 10f;
        public const int StageMaxGold = 300;

        public const float GiganticDuration = 5f;
        public const float GiganticCameraFollowDuration = 0.3f;
        public const float GiganticScaleAdd = 1.5f;
        public const float GiganticEarthShakeInterval = 0.5f;
        public const float GiganticEarthShakeDamage = 30f;
        public const float GiganticContactDamage = 100f;

        public const float MicrifyDuration = 5f;
        public const float MicrifyScaleAdd = -0.7f;

        public const uint SubstituteGolds = 100;
        public const uint SubstituteHeal = 100;

        public const string CakeTokenPath = "Prefabs/Token/Item_Drop_Cake";
        public const string MagnetTokenPath = "Prefabs/Token/Item_Drop_Magnet";
        public const string DeviceBoxTokenPath = "Prefabs/Token/Item_Drop_DeviceBox";
        public const string BombTokenPath = "Prefabs/Token/Item_Drop_Bomb";
        public const string GoldFeverTokenPath = "Prefabs/Token/Item_Drop_GoldFever";
        public const string GiganticTokenPath = "Prefabs/Token/Item_Drop_GiganticTime";
        public const string MicrifyTokenPath = "Prefabs/Token/Item_Drop_MicrifyTime";

        public const string InteractableTeleportPath = "Prefabs/Interactable/Interactable_Teleport";

        public const string InteractableSanctumExperiencePath = "Prefabs/Interactable/Interactable_Sanctum_Experience";
        public const string InteractableSanctumSpeedPath = "Prefabs/Interactable/Interactable_Sanctum_Speed";
        public const string InteractableSanctumHealPath = "Prefabs/Interactable/Interactable_Sanctum_Heal";

        public const string SanctumExperienceVfxPath = "Prefabs/Vfx/Sanctum_Experience";
        public const string SanctumSpeedVfxPath = "Prefabs/Vfx/Sanctum_Speed";
        public const string SanctumHealVfxPath = "Prefabs/Vfx/Sanctum_Heal";

        public const string GiganticBuffId = "Gigantic";
        public const string MicrifyBuffId = "Micrify";

        public const int playerLayerMask = 1 << 7;
        public const int enemyLayerMask = 1 << 8;
        public const int tokenLayerMask = 1 << 12;

        public const int MapChunkSize = 30;

        public static readonly UnityEngine.Quaternion NotReversed = UnityEngine.Quaternion.AngleAxis(0f, UnityEngine.Vector3.up);
        public static readonly UnityEngine.Quaternion Reversed = UnityEngine.Quaternion.AngleAxis(180f, UnityEngine.Vector3.up);

        public const string GoldFeverRewardTokenPrefabPath = "Prefabs/Token/Item_Drop_Coin_1";

        public static int[,] GetRandomMapLayout()
        {
            List<int[,]> maps = new List<int[,]>() {

                new int[,] {
                    {5, 0, 2, 0, 0},
                    {0, 3, 0, 0, 4},
                    {0, 0, 0, 3, 0},
                    {0, 1, 3, 0, 0},
                    {0, 0, 0, 4, 2}
                },

                new int[,] {
                    {0,0,0,0,2},
                    {0,1,0,4,0},
                    {0,0,0,0,0},
                    {3,2,0,3,0},
                    {4,3,0,0,5}
                    },

                new int[,] { //3
                    {0, 0, 0, 0, 3},
                    {0, 0, 0, 0, 4},
                    {2, 3, 1, 0, 2},
                    {0, 0, 0, 0, 4},
                    {0, 0, 0, 0, 5}
                    },
                new int[,] { //4
                    {0, 4, 3, 2, 0},
                    {0, 0, 0, 0, 0},
                    {0, 1, 0, 3, 5},
                    {0, 0, 0, 0, 0},
                    {0, 4, 3, 2, 0}
                    },
                new int[,] { //5
                    {3, 0, 0, 0, 4},
                    {0, 0, 1, 3, 0},
                    {0, 0, 0, 0, 0},
                    {4, 3, 0, 0, 0},
                    {0, 0, 0, 5, 0}
                    },
                new int[,] { //6
                    {5, 0, 0, 4, 0},
                    {0, 0, 3, 0, 0},
                    {0, 0, 1, 0, 0},
                    {0, 0, 0, 3, 0},
                    {0, 4, 0, 0, 0}
                    },
                new int[,] { //7
                    {2, 4, 0, 0, 0},
                    {0, 0, 1, 0, 0},
                    {0, 3, 0, 0, 0},
                    {0, 4, 0, 0, 2},
                    {3, 0, 0, 0, 5}
                    },
                new int[,] { //8
                    {2, 0, 5, 0, 2},
                    {0, 3, 0, 0, 4},
                    {4, 0, 0, 3, 0},
                    {0, 0, 1, 0, 0},
                    {0, 0, 0, 0, 0}
                    },
                new int[,] { //9
                    {5, 0, 0, 4, 3},
                    {0, 0, 0, 0, 0},
                    {0, 0, 1, 0, 0},
                    {0, 0, 0, 0, 0},
                    {3, 4, 0, 0, 0}
                    },
                new int[,] { //10
                    {0, 0, 5, 0, 4},
                    {0, 0, 0, 0, 3},
                    {0, 0, 0, 0, 4},
                    {0, 3, 1, 3, 0},
                    {0, 0, 0, 0, 0}
                    },
                new int[,] { //11
                    {0, 0, 4, 0, 0},
                    {0, 0, 3, 0, 5},
                    {3, 1, 0, 0, 0},
                    {0, 0, 0, 4, 0},
                    {0, 0, 0, 0, 0}
                    },
                new int[,] { //12
                    {0, 4, 0, 0, 5},
                    {0, 0, 0, 0, 0},
                    {0, 3, 0, 0, 0},
                    {0, 4, 0, 1, 0},
                    {0, 0, 0, 3, 0}
                    },
                new int[,] { //13
                    {5, 0, 0, 0, 4},
                    {0, 3, 0, 0, 0},
                    {0, 0, 1, 0, 0},
                    {0, 0, 0, 0, 4},
                    {0, 0, 0, 3, 0}
                    },
                new int[,] { //14
                    {0, 0, 5, 0, 0},
                    {0, 0, 0, 0, 4},
                    {3, 0, 0, 0, 0},
                    {4, 0, 0, 1, 3},
                    {0, 3, 0, 0, 0}
                    },
                new int[,] { //15
                    {0, 0, 0, 4, 0},
                    {0, 4, 0, 3, 0},
                    {0, 0, 0, 0, 0},
                    {0, 0, 3, 1, 0},
                    {5, 0, 0, 0, 0}
                    },
                new int[,] { //16
                    {0, 0, 0, 0, 0},
                    {0, 0, 0, 4, 0},
                    {0, 0, 1, 3, 0},
                    {0, 0, 0, 0, 0},
                    {3, 4, 0, 0, 5}
                    },
                new int[,] { //17
                    {3, 0, 0, 0, 0},
                    {4, 0, 0, 1, 0},
                    {0, 0, 0, 0, 0},
                    {0, 0, 3, 3, 0},
                    {5, 0, 0, 0, 4}
                    },
                new int[,] { //18
                    {3, 0, 0, 0, 0},
                    {4, 0, 0, 1, 0},
                    {0, 0, 3, 0, 0},
                    {0, 0, 0, 0, 0},
                    {0, 4, 0, 0, 5}
                    },
                new int[,] { //19
                    {4, 3, 0, 3, 4},
                    {0, 0, 0, 0, 0},
                    {0, 0, 0, 1, 0},
                    {0, 0, 3, 0, 0},
                    {0, 0, 5, 0, 0}
                    },
                new int[,] { //20
                    {0, 0, 0, 0, 0},
                    {4, 3, 0, 1, 0},
                    {0, 0, 0, 3, 0},
                    {0, 0, 0, 4, 0},
                    {5, 0, 0, 0, 0}
                    },
            };

            return maps.Random();
        }

        public static readonly string[] DefaultMapChunkNames =
        {
            "Stage0/Default/DefaultMapChunk_1",
            "Stage0/Default/DefaultMapChunk_2",
            "Stage0/Default/DefaultMapChunk_3",
            "Stage0/Default/DefaultMapChunk_4",
            "Stage0/Default/DefaultMapChunk_5",
            "Stage0/Default/DefaultMapChunk_6",
            "Stage0/Default/DefaultMapChunk_7",
            "Stage0/Default/DefaultMapChunk_8",
            "Stage0/Default/DefaultMapChunk_9",
            "Stage0/Default/DefaultMapChunk_10",
            "Stage0/Default/DefaultMapChunk_11",
            "Stage0/Default/DefaultMapChunk_12",
            "Stage0/Default/DefaultMapChunk_13",
            "Stage0/Default/DefaultMapChunk_14",
            "Stage0/Default/DefaultMapChunk_15",
            "Stage0/Default/DefaultMapChunk_16",
            "Stage0/Default/DefaultMapChunk_17",
            "Stage0/Default/DefaultMapChunk_18",
            "Stage0/Default/DefaultMapChunk_19",
            "Stage0/Default/DefaultMapChunk_20",
            "Stage0/Default/DefaultMapChunk_21",
            "Stage0/Default/DefaultMapChunk_22",
            "Stage0/Default/DefaultMapChunk_23",
            "Stage0/Default/DefaultMapChunk_24",
            "Stage0/Default/DefaultMapChunk_25",
            "Stage0/Default/DefaultMapChunk_26",
            "Stage0/Default/DefaultMapChunk_27",
            "Stage0/Default/DefaultMapChunk_28",
            "Stage0/Default/DefaultMapChunk_29",
            "Stage0/Default/DefaultMapChunk_30",
            "Stage0/Default/DefaultMapChunk_31",

        };

        public static readonly string[] StartMapChunkNames =
        {
            "Stage0/Start/StartMapChunk_1",
            "Stage0/Start/StartMapChunk_2",
            "Stage0/Start/StartMapChunk_3",
            "Stage0/Start/StartMapChunk_4",
        };

        public static readonly string[] MiddleBossMapChunkNames =
        {
            "Stage0/MiddleBoss/MiddleBossMapChunk_1",
            "Stage0/MiddleBoss/MiddleBossMapChunk_2"
        };

        public static readonly string[] EventMapChunkNames =
        {
            "Stage0/Event/EventMapChunk_1",
            "Stage0/Event/EventMapChunk_2",
            "Stage0/Event/EventMapChunk_3",
            "Stage0/Event/EventMapChunk_4",
            "Stage0/Event/EventMapChunk_5",
            "Stage0/Event/EventMapChunk_6",
            "Stage0/Event/EventMapChunk_7",
            "Stage0/Event/EventMapChunk_8",
            "Stage0/Event/EventMapChunk_9",
            "Stage0/Event/EventMapChunk_10",
            "Stage0/Event/EventMapChunk_11",
            "Stage0/Event/EventMapChunk_12",
            "Stage0/Event/EventMapChunk_13",
            "Stage0/Event/EventMapChunk_14",
            "Stage0/Event/EventMapChunk_15",
        };

        public static readonly string[] TeleportMapChunkNames =
        {
            "Stage0/Teleport/TeleportMapChunk_1",
            "Stage0/Teleport/TeleportMapChunk_2",
        };

        public static readonly string[] FinalBossMapChunkNames =
        {
            "Stage0/FinalBoss/FinalBossMapChunk_1",
        };

        public static uint GetRequiredExpForNextLevel(int currentLevel)
        {
            uint _currentLevel = (uint)currentLevel;

            return currentLevel switch
            {
                <= 20 => _currentLevel * 5,
                <= 30 => _currentLevel * 7 - 40,
                <= 40 => _currentLevel * 9 - 100,
                <= 50 => _currentLevel * 11 - 180,
                _ => _currentLevel * 13 - 280,
            };
        }

        public static string GetExpTokenPrefabPath(int exp) => exp switch
        {
            < 4 => "Prefabs/Token/Item_Drop_EXP_1",
            < 10 => "Prefabs/Token/Item_Drop_EXP_2",
            _ => "Prefabs/Token/Item_Drop_EXP_3"
        };

        public static string GetGoldTokenPrefabPath(float goldRate) => goldRate switch
        {
            < 1f / 3f => "Prefabs/Token/Item_Drop_Coin_1",
            < 2f / 3f => "Prefabs/Token/Item_Drop_Coin_2",
            _ => "Prefabs/Token/Item_Drop_Coin_3"
        };
        public static float GetGoldFeverTimeIncrease(float goldRate) => goldRate switch
        {
            < 1f / 3f => 0,
            < 2f / 3f => 0.05f,
            _ => 0.1f
        };

        public static T DeepClone<T>(this T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
