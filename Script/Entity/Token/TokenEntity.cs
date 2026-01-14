using System.Collections;
using System.Collections.Generic;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Game.ECS
{
    public class TokenEntity : Entity
    {
        public Transform transformImplement;
        public Rigidbody2D rigidBodyImplement;
        public SpriteRenderer spriterendererImplement;
        public TranslateImplement translateImplement;
        public ApplyBuffImplement applyBuffImplement;
        public TokenStateImplement tokenStateImplement;
        public TokenSpeechImplement tokenSpeechImplement;
        
        [Inject] protected ICandidateRemoveEntitySystem candidateRemoveEntitySystem;

        public override void OnCreateEntity()
        {
            base.OnCreateEntity();
            spriterendererImplement = GetComponentInChildren<SpriteRenderer>();
            transformImplement = transform;
            rigidBodyImplement = GetComponent<Rigidbody2D>();
            
            applyBuffImplement = GetComponent<ApplyBuffImplement>();
            translateImplement = GetComponent<TranslateImplement>();
            tokenStateImplement = GetComponent<TokenStateImplement>();
            
            Components.Add(applyBuffImplement);
            Components.Add(translateImplement);
            Components.Add(tokenStateImplement);

            if(TryGetComponent(out tokenSpeechImplement))
            {
                Components.Add(tokenSpeechImplement);
            }
            
            candidateRemoveEntitySystem.RegistComponent(null, this);
        }

        public override void OnRemoveEntity()
        {
            base.OnRemoveEntity();
            candidateRemoveEntitySystem.UnRegistComponent(this);
        }

        public override void ApplyComponentData()
        {
            base.ApplyComponentData();
            spriterendererImplement.transform.localPosition = Vector3.zero;
            tokenStateImplement.tokenStateProperty.statusValue = TokenStateType.Idle;
        }

        protected override void Update()
        {
            base.Update();
            UpdateSpritePosition(spriterendererImplement, tokenStateImplement);
        }

        private static void UpdateSpritePosition(SpriteRenderer spriteRendererImpl, TokenStateImplement tokenStateImpl)
        {
            if (tokenStateImpl.tokenStateProperty.statusValue == TokenStateType.Catched)
            {
                if (spriteRendererImpl.transform.localPosition.y < GameSettings.TokenCatchAnimDistance)
                    spriteRendererImpl.transform.localPosition = Vector3.MoveTowards(spriteRendererImpl.transform.localPosition, Vector3.up * GameSettings.TokenCatchAnimDistance, 1 / GameSettings.TokenCatchAnimSpeed * Time.deltaTime);
                else
                    tokenStateImpl.tokenStateProperty.statusValue = TokenStateType.Applied;
            }
        }
    }
}
