using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000358 RID: 856
	public class MeteorStormController : MonoBehaviour
	{
		// Token: 0x0600119E RID: 4510 RVA: 0x000573B3 File Offset: 0x000555B3
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.meteorList = new List<MeteorStormController.Meteor>();
				this.waveList = new List<MeteorStormController.MeteorWave>();
			}
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x000573D4 File Offset: 0x000555D4
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
						EffectManager.instance.SpawnEffect(this.warningEffectPrefab, new EffectData
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

		// Token: 0x060011A0 RID: 4512 RVA: 0x000575C6 File Offset: 0x000557C6
		private void OnDestroy()
		{
			this.onDestroyEvents.Invoke();
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x000575D3 File Offset: 0x000557D3
		private void DoMeteorEffect(MeteorStormController.Meteor meteor)
		{
			meteor.didTravelEffect = true;
			if (this.travelEffectPrefab)
			{
				EffectManager.instance.SpawnEffect(this.travelEffectPrefab, new EffectData
				{
					origin = meteor.impactPosition
				}, true);
			}
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0005760C File Offset: 0x0005580C
		private void DetonateMeteor(MeteorStormController.Meteor meteor)
		{
			EffectData effectData = new EffectData
			{
				origin = meteor.impactPosition
			};
			EffectManager.instance.SpawnEffect(this.impactEffectPrefab, effectData, true);
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

		// Token: 0x040015A7 RID: 5543
		public int waveCount;

		// Token: 0x040015A8 RID: 5544
		public float waveMinInterval;

		// Token: 0x040015A9 RID: 5545
		public float waveMaxInterval;

		// Token: 0x040015AA RID: 5546
		public GameObject warningEffectPrefab;

		// Token: 0x040015AB RID: 5547
		public GameObject travelEffectPrefab;

		// Token: 0x040015AC RID: 5548
		public float travelEffectDuration;

		// Token: 0x040015AD RID: 5549
		public GameObject impactEffectPrefab;

		// Token: 0x040015AE RID: 5550
		public float impactDelay;

		// Token: 0x040015AF RID: 5551
		public float blastDamageCoefficient;

		// Token: 0x040015B0 RID: 5552
		public float blastRadius;

		// Token: 0x040015B1 RID: 5553
		public float blastForce;

		// Token: 0x040015B2 RID: 5554
		[NonSerialized]
		public GameObject owner;

		// Token: 0x040015B3 RID: 5555
		[NonSerialized]
		public float ownerDamage;

		// Token: 0x040015B4 RID: 5556
		[NonSerialized]
		public bool isCrit;

		// Token: 0x040015B5 RID: 5557
		public UnityEvent onDestroyEvents;

		// Token: 0x040015B6 RID: 5558
		private List<MeteorStormController.Meteor> meteorList;

		// Token: 0x040015B7 RID: 5559
		private List<MeteorStormController.MeteorWave> waveList;

		// Token: 0x040015B8 RID: 5560
		private int wavesPerformed;

		// Token: 0x040015B9 RID: 5561
		private float waveTimer;

		// Token: 0x02000359 RID: 857
		private class Meteor
		{
			// Token: 0x040015BA RID: 5562
			public Vector3 impactPosition;

			// Token: 0x040015BB RID: 5563
			public float startTime;

			// Token: 0x040015BC RID: 5564
			public bool didTravelEffect;

			// Token: 0x040015BD RID: 5565
			public bool valid = true;
		}

		// Token: 0x0200035A RID: 858
		private class MeteorWave
		{
			// Token: 0x060011A5 RID: 4517 RVA: 0x000576F0 File Offset: 0x000558F0
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

			// Token: 0x060011A6 RID: 4518 RVA: 0x00057784 File Offset: 0x00055984
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

			// Token: 0x040015BE RID: 5566
			private readonly CharacterBody[] targets;

			// Token: 0x040015BF RID: 5567
			private int currentStep;

			// Token: 0x040015C0 RID: 5568
			private float hitChance = 0.4f;

			// Token: 0x040015C1 RID: 5569
			private readonly Vector3 center;

			// Token: 0x040015C2 RID: 5570
			public float timer;

			// Token: 0x040015C3 RID: 5571
			private NodeGraphSpider nodeGraphSpider;
		}
	}
}
