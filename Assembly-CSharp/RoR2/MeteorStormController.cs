using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000281 RID: 641
	public class MeteorStormController : MonoBehaviour
	{
		// Token: 0x06000E33 RID: 3635 RVA: 0x0003F410 File Offset: 0x0003D610
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.meteorList = new List<MeteorStormController.Meteor>();
				this.waveList = new List<MeteorStormController.MeteorWave>();
			}
		}

		// Token: 0x06000E34 RID: 3636 RVA: 0x0003F430 File Offset: 0x0003D630
		private void FixedUpdate()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.waveTimer -= Time.fixedDeltaTime;
			if (this.waveTimer <= 0f && this.wavesPerformed < this.waveCount)
			{
				this.wavesPerformed++;
				this.waveTimer = UnityEngine.Random.Range(this.waveMinInterval, this.waveMaxInterval);
				MeteorStormController.MeteorWave item = new MeteorStormController.MeteorWave(CharacterBody.readOnlyInstancesList.ToArray<CharacterBody>(), base.transform.position);
				this.waveList.Add(item);
			}
			for (int i = this.waveList.Count - 1; i >= 0; i--)
			{
				MeteorStormController.MeteorWave meteorWave = this.waveList[i];
				meteorWave.timer -= Time.fixedDeltaTime;
				if (meteorWave.timer <= 0f)
				{
					meteorWave.timer = UnityEngine.Random.Range(0.05f, 1f);
					MeteorStormController.Meteor nextMeteor = meteorWave.GetNextMeteor();
					if (nextMeteor == null)
					{
						this.waveList.RemoveAt(i);
					}
					else if (nextMeteor.valid)
					{
						this.meteorList.Add(nextMeteor);
						EffectManager.SpawnEffect(this.warningEffectPrefab, new EffectData
						{
							origin = nextMeteor.impactPosition,
							scale = this.blastRadius
						}, true);
					}
				}
			}
			float num = Run.instance.time - this.impactDelay;
			float num2 = num - this.travelEffectDuration;
			for (int j = this.meteorList.Count - 1; j >= 0; j--)
			{
				MeteorStormController.Meteor meteor = this.meteorList[j];
				if (meteor.startTime < num2 && !meteor.didTravelEffect)
				{
					this.DoMeteorEffect(meteor);
				}
				if (meteor.startTime < num)
				{
					this.meteorList.RemoveAt(j);
					this.DetonateMeteor(meteor);
				}
			}
			if (this.wavesPerformed == this.waveCount && this.meteorList.Count == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x0003F61D File Offset: 0x0003D81D
		private void OnDestroy()
		{
			this.onDestroyEvents.Invoke();
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x0003F62A File Offset: 0x0003D82A
		private void DoMeteorEffect(MeteorStormController.Meteor meteor)
		{
			meteor.didTravelEffect = true;
			if (this.travelEffectPrefab)
			{
				EffectManager.SpawnEffect(this.travelEffectPrefab, new EffectData
				{
					origin = meteor.impactPosition
				}, true);
			}
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x0003F660 File Offset: 0x0003D860
		private void DetonateMeteor(MeteorStormController.Meteor meteor)
		{
			EffectData effectData = new EffectData
			{
				origin = meteor.impactPosition
			};
			EffectManager.SpawnEffect(this.impactEffectPrefab, effectData, true);
			new BlastAttack
			{
				inflictor = base.gameObject,
				baseDamage = this.blastDamageCoefficient * this.ownerDamage,
				baseForce = this.blastForce,
				canHurtAttacker = true,
				crit = this.isCrit,
				falloffModel = BlastAttack.FalloffModel.Linear,
				attacker = this.owner,
				bonusForce = Vector3.zero,
				damageColorIndex = DamageColorIndex.Item,
				position = meteor.impactPosition,
				procChainMask = default(ProcChainMask),
				procCoefficient = 1f,
				teamIndex = TeamIndex.None,
				radius = this.blastRadius
			}.Fire();
		}

		// Token: 0x04000E1F RID: 3615
		public int waveCount;

		// Token: 0x04000E20 RID: 3616
		public float waveMinInterval;

		// Token: 0x04000E21 RID: 3617
		public float waveMaxInterval;

		// Token: 0x04000E22 RID: 3618
		public GameObject warningEffectPrefab;

		// Token: 0x04000E23 RID: 3619
		public GameObject travelEffectPrefab;

		// Token: 0x04000E24 RID: 3620
		public float travelEffectDuration;

		// Token: 0x04000E25 RID: 3621
		public GameObject impactEffectPrefab;

		// Token: 0x04000E26 RID: 3622
		public float impactDelay;

		// Token: 0x04000E27 RID: 3623
		public float blastDamageCoefficient;

		// Token: 0x04000E28 RID: 3624
		public float blastRadius;

		// Token: 0x04000E29 RID: 3625
		public float blastForce;

		// Token: 0x04000E2A RID: 3626
		[NonSerialized]
		public GameObject owner;

		// Token: 0x04000E2B RID: 3627
		[NonSerialized]
		public float ownerDamage;

		// Token: 0x04000E2C RID: 3628
		[NonSerialized]
		public bool isCrit;

		// Token: 0x04000E2D RID: 3629
		public UnityEvent onDestroyEvents;

		// Token: 0x04000E2E RID: 3630
		private List<MeteorStormController.Meteor> meteorList;

		// Token: 0x04000E2F RID: 3631
		private List<MeteorStormController.MeteorWave> waveList;

		// Token: 0x04000E30 RID: 3632
		private int wavesPerformed;

		// Token: 0x04000E31 RID: 3633
		private float waveTimer;

		// Token: 0x02000282 RID: 642
		private class Meteor
		{
			// Token: 0x04000E32 RID: 3634
			public Vector3 impactPosition;

			// Token: 0x04000E33 RID: 3635
			public float startTime;

			// Token: 0x04000E34 RID: 3636
			public bool didTravelEffect;

			// Token: 0x04000E35 RID: 3637
			public bool valid = true;
		}

		// Token: 0x02000283 RID: 643
		private class MeteorWave
		{
			// Token: 0x06000E3A RID: 3642 RVA: 0x0003F740 File Offset: 0x0003D940
			public MeteorWave(CharacterBody[] targets, Vector3 center)
			{
				this.targets = new CharacterBody[targets.Length];
				targets.CopyTo(this.targets, 0);
				Util.ShuffleArray<CharacterBody>(targets);
				this.center = center;
				this.nodeGraphSpider = new NodeGraphSpider(SceneInfo.instance.groundNodes, HullMask.Human);
				this.nodeGraphSpider.AddNodeForNextStep(SceneInfo.instance.groundNodes.FindClosestNode(center, HullClassification.Human));
				int num = 0;
				int num2 = 20;
				while (num < num2 && this.nodeGraphSpider.PerformStep())
				{
					num++;
				}
			}

			// Token: 0x06000E3B RID: 3643 RVA: 0x0003F7D4 File Offset: 0x0003D9D4
			public MeteorStormController.Meteor GetNextMeteor()
			{
				if (this.currentStep >= this.targets.Length)
				{
					return null;
				}
				CharacterBody characterBody = this.targets[this.currentStep];
				MeteorStormController.Meteor meteor = new MeteorStormController.Meteor();
				if (characterBody && UnityEngine.Random.value < this.hitChance)
				{
					meteor.impactPosition = characterBody.corePosition;
					Vector3 origin = meteor.impactPosition + Vector3.up * 6f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					onUnitSphere.y = -1f;
					RaycastHit raycastHit;
					if (Physics.Raycast(origin, onUnitSphere, out raycastHit, 12f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
					{
						meteor.impactPosition = raycastHit.point;
					}
					else if (Physics.Raycast(meteor.impactPosition, Vector3.down, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
					{
						meteor.impactPosition = raycastHit.point;
					}
				}
				else if (this.nodeGraphSpider.collectedSteps.Count != 0)
				{
					int index = UnityEngine.Random.Range(0, this.nodeGraphSpider.collectedSteps.Count);
					SceneInfo.instance.groundNodes.GetNodePosition(this.nodeGraphSpider.collectedSteps[index].node, out meteor.impactPosition);
				}
				else
				{
					meteor.valid = false;
				}
				meteor.startTime = Run.instance.time;
				this.currentStep++;
				return meteor;
			}

			// Token: 0x04000E36 RID: 3638
			private readonly CharacterBody[] targets;

			// Token: 0x04000E37 RID: 3639
			private int currentStep;

			// Token: 0x04000E38 RID: 3640
			private float hitChance = 0.4f;

			// Token: 0x04000E39 RID: 3641
			private readonly Vector3 center;

			// Token: 0x04000E3A RID: 3642
			public float timer;

			// Token: 0x04000E3B RID: 3643
			private NodeGraphSpider nodeGraphSpider;
		}
	}
}
