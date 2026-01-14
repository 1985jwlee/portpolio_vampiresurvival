using System.Collections.Generic;
using Reflex;
using Reflex.Scripts.Attributes;
using UniRx;
using UnityEngine;

namespace Game.ECS
{
    public class HPBarView : MonoBehaviour
    {
        public RectTransform rectTransform;
        [Inject] private Camera mainCam;
        [Inject] private Container container;
        [Inject] private IEntityContainer entityContainer;

        private Dictionary<uint, (HPBarGaugeView view, float offset)> linkEntityHPBar = new Dictionary<uint, (HPBarGaugeView, float)>();
        private List<uint> removeHPBarCandidate = new List<uint>();

        private void Awake()
        {
            MessageBroker.Default.Receive<KeyValuePair<string, (uint id, float offset)>>()
                .TakeUntilDestroy(gameObject)
                .Subscribe(_x =>
                {
                    switch (_x.Key)
                    {
                        case "bossCharacterCreate":
                        case "playerCharacterCreate":
                            CreateHPBarByEntityId(_x.Value.id, _x.Value.offset);
                            break;
                    }
                });
        }

        private void Update()
        {
            SyncHPBarPosition();
            foreach (var kv in linkEntityHPBar)
            {
                RemoveHPBarDeathBoss(kv.Key);
            }
            DestroyHPBarCandidated();
        }
        
        private void CreateHPBarByEntityId(uint targetEntityId, float offset)
        {
            if (entityContainer.GetEntity(targetEntityId, out CharacterEntity entity))
            {
                if (linkEntityHPBar.ContainsKey(entity.EGID) == false)
                {
                    var srcHpbarRoot = Resources.Load<HPBarGaugeView>(GetSrcHpBarRootPath(targetEntityId));
                    var gaugeView = container.Instantiate(srcHpbarRoot, rectTransform);
                    var rectPos = ExtensionFunction.WorldPostionToCanvasPosition(mainCam, rectTransform, entity.translateImplement.positionProperty.statusValue);
                    rectPos.y -= offset;
                    gaugeView.targetEntityId = entity.EGID;
                    gaugeView.maxHitPoint = entity.statusImplement.hitPointProperty.statusValue;
                    gaugeView.myTransform.anchoredPosition = rectPos;
                    linkEntityHPBar.Add(entity.EGID, (gaugeView, offset));
                }
            }
        }

        private void RemoveHPBarDeathBoss(uint targetEntityId)
        {
            if (entityContainer.GetEntity<BossEnemyCharacterEntity>(targetEntityId, out var bossEntity))
            {
                if (bossEntity.statusImplement.hitPointProperty.statusValue > 0 == false)
                {
                    if (linkEntityHPBar.ContainsKey(bossEntity.EGID))
                    {
                        removeHPBarCandidate.Add(bossEntity.EGID);
                    }
                }
            }
        }

        private void DestroyHPBarCandidated()
        {
            foreach (var remove in removeHPBarCandidate)
            {
                if (linkEntityHPBar.TryGetValue(remove, out var keyValue))
                {
                    Destroy(keyValue.view.gameObject);
                }

                linkEntityHPBar.Remove(remove);
            }
            removeHPBarCandidate.Clear();
        }
        

        private string GetSrcHpBarRootPath(uint egid)
        {
            if (entityContainer.playerCharacterEntity.EGID == egid)
            {
                return "Prefabs/UI/Old/HPBarGaugeView";
            }

            if (entityContainer.GetEntity<BossEnemyCharacterEntity>(egid, out _))
            {
                return "Prefabs/UI/BossHPBarGaugeView";
            }
            return "";
        }

        private void SyncHPBarPosition()
        {
            foreach (var kv in linkEntityHPBar)
            {
                if (entityContainer.GetEntity(kv.Key, out CharacterEntity entity))
                {
                    var rectPos = ExtensionFunction.WorldPostionToCanvasPosition(mainCam, rectTransform, entity.translateImplement.positionProperty.statusValue);
                    rectPos.y -= kv.Value.offset;
                    kv.Value.view.myTransform.anchoredPosition = rectPos;
                }
            }
        }
    }
}
