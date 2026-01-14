using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class MainGameSceneContextModel
    {
        public CharacterDataEntity characterDataEntity;
        public CharacterDataEntity supporterCharacter0DataEntity;
        public CharacterDataEntity supporterCharacter1DataEntity;
        public Bounds mapBound;
        public Vector2 startPosition;
        public Vector2 mainBossIconPosition;
        public List<Vector2> bossIconPositions = new();
        public Vector2 bossTeleportPosition;
        public List<Vector2> teleportIconPositions = new();
    }
}
