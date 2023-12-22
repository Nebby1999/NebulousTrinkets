using MSU;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NebulousTrinkets.Items
{
    public class NebulousSkull : IItemContentPiece
    {
        public NullableRef<GameObject> ItemDisplayPrefab => null;

        public ItemDef Asset => _asset;
        private ItemDef _asset;

        public bool IsAvailable()
        {
            return true;
        }

        public IEnumerator LoadContentAsync()
        {
            var request = new NebulousTrinketsAssets.SingleBundleAssetRequest<ItemDef>("NebulousSkull", NebulousTrinketsBundle.Items);
            yield return request.Load();
            _asset = request.Asset;
        }

        public void Initialize()
        {
            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
            On.RoR2.Orbs.GenericDamageOrb.Begin += GenericDamageOrb_Begin;
        }

        private void GenericDamageOrb_Begin(On.RoR2.Orbs.GenericDamageOrb.orig_Begin orig, RoR2.Orbs.GenericDamageOrb self)
        {
            if (self.attacker && self.attacker.TryGetComponent<CharacterBody>(out var body))
            {
                var itemCount = body.GetItemCount(Asset);
                if (itemCount == 0)
                {
                    orig(self);
                    return;
                }

                var procCoef = self.procCoefficient;
                self.procCoefficient = procCoef + (procCoef / 10 * itemCount);

                var speed = self.speed;
                self.speed = speed + (procCoef / 4 * itemCount);
            }

            orig(self);
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults)
        {
            float sizeModifier = 1;
            if (self.attacker && self.attacker.TryGetComponent<CharacterBody>(out var body))
            {
                var itemCount = body.GetItemCount(Asset);
                if (itemCount == 0)
                {
                    return orig(self, hitResults);
                }

                var procCoef = self.procCoefficient;
                self.procCoefficient = procCoef + ((procCoef / 10) * itemCount);

                sizeModifier += 0.1f * itemCount;

                foreach (HitBox hitBox in self.hitBoxGroup.hitBoxes)
                {
                    if (!hitBox || !hitBox.enabled || !hitBox.gameObject || !hitBox.gameObject.activeInHierarchy || !hitBox.transform)
                    {
                        continue;
                    }

                    var t = hitBox.transform;
                    t.localScale *= sizeModifier;
                }
            }
            bool result = orig(self, hitResults);

            foreach(HitBox hitBox in self.hitBoxGroup.hitBoxes)
            {
                if (!hitBox || !hitBox.enabled || !hitBox.gameObject || !hitBox.gameObject.activeInHierarchy || !hitBox.transform)
                {
                    continue;
                }

                var t = hitBox.transform;
                t.localScale /= sizeModifier;
            }
            return result;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            if(self.owner && self.owner.TryGetComponent<CharacterBody>(out var body))
            {
                var itemCount = body.GetItemCount(Asset);
                if (itemCount == 0)
                {
                    orig(self);
                    return;
                }

                var procCoef = self.procCoefficient;
                self.procCoefficient = procCoef + ((procCoef / 10) * itemCount);

                var radius = self.radius;
                self.radius = radius + (radius / 4) * itemCount;

                var distance = self.maxDistance;
                self.maxDistance = distance + (distance / 2) * itemCount;

                var maxSpread = self.maxSpread;
                self.maxSpread = Mathf.Max(0, maxSpread - (maxSpread / 5) * itemCount);

                var minSpread = self.minSpread;
                self.minSpread = Mathf.Max(0, minSpread - (minSpread / 5) * itemCount);

                var yawSpread = self.spreadYawScale;
                self.spreadYawScale = Mathf.Max(0, yawSpread - (yawSpread / 5) * itemCount);

                var pitchSpread = self.spreadPitchScale;
                self.spreadPitchScale = Mathf.Max(0, pitchSpread - (pitchSpread / 5) * itemCount);

                var bulletCount = self.bulletCount;
                self.bulletCount = (uint)Mathf.RoundToInt(bulletCount + (bulletCount / 10) * itemCount);
            }
            orig(self);
            return;
        }

        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self)
        {
            if(self.attacker && self.attacker.TryGetComponent<CharacterBody>(out var body))
            {
                var itemCount = body.GetItemCount(Asset);
                if (itemCount == 0)
                    return orig(self);

                var procCoef = self.procCoefficient;
                self.procCoefficient = procCoef + ((procCoef / 10) * itemCount);

                var radius = self.radius;
                self.radius = radius + (radius / 4) * itemCount;
            }
            return orig(self);
        }
    }
}
