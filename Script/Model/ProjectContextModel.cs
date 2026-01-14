using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class ProjectContextModel
    {
        public bool characterSelectionById = true;
        public string selectedCharacterId = "";
        public string selectedSupport0CharacterId = "";
        public string selectedSupport1CharacterId = "";
        public CharacterDataEntity selectedCharacterEntity;
    }
}
