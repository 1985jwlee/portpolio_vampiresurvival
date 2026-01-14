using Reflex.Scripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ECS
{
    public class CharacterFollowWorldUi : MonoBehaviour
    {
        [Inject] IEntityContainer entityContainer;
        private void Update()
        {
            transform.position = entityContainer.playerCharacterEntity.transform.position;
        }
    }
}
