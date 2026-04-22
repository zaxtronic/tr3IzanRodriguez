using Combat;
using Combat.Data;
using Combat.Interfaces;
using Entity_Components;
using Entity_Components.Character;
using Referencing.Scriptable_Pool;
using System.Collections;
using UnityEngine;

namespace Item.Actions
{
    [CreateAssetMenu(fileName = "Item Action Attack", menuName = "Items/Item Actions/Attack")]
    public class ItemAction_Attack : ItemAction
    {
        [SerializeField]
        private ScriptablePoolContainer damageVolumePool;

        [SerializeField]
        private string[] targetTags;

        [SerializeField]
        private int damage;

        [SerializeField]
        private float speed;

        [SerializeField]
        private float attackDistance;

        [Header("Zelda Style Tuning")]
        [SerializeField]
        private bool useAdvancedTimings = false;

        [SerializeField]
        private float windupTime = 0.08f;

        [SerializeField]
        private float activeTime = 0.12f;

        [SerializeField]
        private float recoveryTime = 0.18f;

        [SerializeField]
        private Vector2 hitboxSize = new Vector2(0.6f, 0.6f);

        [SerializeField]
        private float lungeDistance = 0.20f;

        [SerializeField]
        private float lungeDuration = 0.06f;

        [SerializeField]
        private float attackerHitStopTime = 0.06f;

        [SerializeField]
        private float targetHitStopTime = 0.08f;

        [SerializeField]
        private bool freezeMovementDuringAttack = true;

        [Header("Directional Feel")]
        [SerializeField]
        private bool useLiveAimDuringAttack = false;

        [SerializeField]
        private float aimUpdateRate = 0f;

        [SerializeField, Tooltip("Is one attack able to hit multiple targets")]
        private bool hitMultipleTargets;

        private enum AttackType { Smash, Slash };

        [SerializeField]
        private AttackType attackType = AttackType.Smash;

        public override IEnumerator ItemUseAction(Inventory.Inventory userInventory, int itemIndex)
        {
            Aimer getAimer = userInventory.GetComponent<Aimer>();
            Mover getMover = userInventory.GetComponent<Mover>();
            //GridSelector getGridSelector = userInventory.GetComponent<GridSelector>();

            if (getMover.IsMovementFrozen)
                yield break;

            Vector2 aimDir = GetAimDirectionOrFallback(getAimer, Vector2.down);
            Vector2 attackLocation = (Vector2)userInventory.transform.position + (aimDir * attackDistance);
            //Vector2 attackLocation = getGridSelector.GetGridWorldSelectionPosition();

            getAimer.LookAt(attackLocation);

            BodyAnimation[] getEntityAnimator = userInventory.GetComponentsInChildren<BodyAnimation>();

            float animationTime = 0;

            for (int i = 0; i < getEntityAnimator.Length; i++)
            {
                switch (attackType)
                {
                    case AttackType.Smash:
                        animationTime = getEntityAnimator[i].ApplySmashAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
                        break;
                    case AttackType.Slash:
                        animationTime = getEntityAnimator[i].ApplySlashAnimation(speed, userInventory.GetItem(itemIndex).Data.Icon);
                        break;
                    default:
                        break;
                }
            }

            if (freezeMovementDuringAttack)
            {
                getMover.FreezeMovement(true);
            }

            float windup = useAdvancedTimings ? windupTime : animationTime * 0.5f;
            float active = useAdvancedTimings ? activeTime : 0.5f;
            float recovery = useAdvancedTimings ? recoveryTime : animationTime * 0.5f;

            if (windup > 0)
            {
                yield return new WaitForSeconds(windup);
            }

            if (useAdvancedTimings && lungeDistance > 0 && lungeDuration > 0)
            {
                yield return userInventory.StartCoroutine(Lunge(getMover, aimDir, lungeDistance, lungeDuration));
            }

            GameObject damageVolume = damageVolumePool.Retrieve(attackLocation, new Quaternion());

            DamageVolume damageComponent = damageVolume.GetComponent<DamageVolume>();

            if (attackerHitStopTime > 0 || targetHitStopTime > 0)
            {
                damageComponent.SetCallBack(new HitStopCallback(userInventory, getMover, attackerHitStopTime, targetHitStopTime));
            }

            Vector2 useHitboxSize = useAdvancedTimings ? hitboxSize : new Vector2(0.10f, 0.10f);

            damageComponent.Configure(new DamageVolumeConfiguration()
            {
                Damage = damage,
                Owner = userInventory.gameObject,
                ActiveTime = active,
                TargetTags = targetTags,
                AllowDuplicateDamage = true,
                CanDamageMultiple = hitMultipleTargets,
                Size = useHitboxSize
            });

            if (useLiveAimDuringAttack && active > 0)
            {
                userInventory.StartCoroutine(FollowAimDuringActive(damageVolume.transform, userInventory.transform, getAimer, attackDistance, active, aimUpdateRate));
            }

            if (recovery > 0)
            {
                yield return new WaitForSeconds(recovery);
            }

            if (freezeMovementDuringAttack)
            {
                getMover.FreezeMovement(false);
            }

            yield return null;
        }

        public override void ItemAcquisitionAction(Inventory.Inventory userInventory, int itemIndex)
        {
        
        }

        public override bool ItemUseCondition(Inventory.Inventory userInventory, int itemIndex)
        {
            return true;
        }

        private IEnumerator Lunge(Mover mover, Vector2 direction, float distance, float duration)
        {
            Vector2 startPos = mover.transform.position;
            Vector2 targetPos = startPos + (direction.normalized * distance);

            float t = 0f;
            while (t < duration)
            {
                t = Mathf.MoveTowards(t, duration, UnityEngine.Time.deltaTime);
                mover.SetPosition(Vector2.Lerp(startPos, targetPos, t / duration));
                yield return null;
            }
        }

        private Vector2 GetAimDirectionOrFallback(Aimer aimer, Vector2 fallback)
        {
            if (aimer == null)
                return fallback;

            Vector2 dir = aimer.GetAimDirection();
            if (dir == Vector2.zero)
                return fallback;

            return dir.normalized;
        }

        private IEnumerator FollowAimDuringActive(Transform hitboxTransform, Transform owner, Aimer aimer, float distance, float duration, float updateRate)
        {
            float t = 0f;
            float updateTimer = 0f;

            Vector2 lastNonZero = Vector2.down;

            while (t < duration)
            {
                t = Mathf.MoveTowards(t, duration, UnityEngine.Time.deltaTime);

                if (updateRate > 0)
                {
                    updateTimer -= UnityEngine.Time.deltaTime;
                    if (updateTimer > 0)
                    {
                        yield return null;
                        continue;
                    }
                    updateTimer = updateRate;
                }

                Vector2 aimDir = (aimer != null) ? aimer.GetAimDirection() : lastNonZero;
                if (aimDir != Vector2.zero)
                {
                    lastNonZero = aimDir.normalized;
                }

                hitboxTransform.position = (Vector2)owner.position + (lastNonZero * distance);
                yield return null;
            }
        }

        private class HitStopCallback : IDamageCallback
        {
            private readonly MonoBehaviour runner;
            private readonly Mover attackerMover;
            private readonly float attackerHitStop;
            private readonly float targetHitStop;

            public HitStopCallback(MonoBehaviour runner, Mover attackerMover, float attackerHitStop, float targetHitStop)
            {
                this.runner = runner;
                this.attackerMover = attackerMover;
                this.attackerHitStop = attackerHitStop;
                this.targetHitStop = targetHitStop;
            }

            public void OnDamageDone(Health target, DamageInfo info)
            {
                if (attackerMover != null && attackerHitStop > 0)
                {
                    runner.StartCoroutine(DoFreeze(attackerMover, attackerHitStop));
                }

                if (target != null && targetHitStop > 0)
                {
                    Mover targetMover = target.GetComponent<Mover>();
                    if (targetMover != null)
                    {
                        runner.StartCoroutine(DoFreeze(targetMover, targetHitStop));
                    }
                }
            }

            private IEnumerator DoFreeze(Mover mover, float duration)
            {
                bool wasFrozen = mover.IsMovementFrozen;
                if (!wasFrozen)
                {
                    mover.FreezeMovement(true);
                }

                yield return new WaitForSeconds(duration);

                if (!wasFrozen)
                {
                    mover.FreezeMovement(false);
                }
            }
        }
    }
}
