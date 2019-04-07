using System;
using UnityEngine;

namespace Generics.Dynamics
{
	// Token: 0x020006DD RID: 1757
	public class InverseKinematics : MonoBehaviour
	{
		// Token: 0x06002753 RID: 10067 RVA: 0x000B65B6 File Offset: 0x000B47B6
		private void OnEnable()
		{
			if (this.rigReader == null)
			{
				this.DetectRig();
			}
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x000B65C8 File Offset: 0x000B47C8
		private void LateUpdate()
		{
			Core.Solvers solvers = this.solver;
			if (solvers != Core.Solvers.CyclicDescend)
			{
				if (solvers == Core.Solvers.FastReach)
				{
					for (int i = 0; i < this.otherChains.Length; i++)
					{
						FastReachSolver.Process(this.otherChains[i]);
					}
					FastReachSolver.Process(this.rLeg);
					FastReachSolver.Process(this.lLeg);
					FastReachSolver.Process(this.rArm);
					FastReachSolver.Process(this.lArm);
				}
			}
			else
			{
				CyclicDescendSolver.Process(this.rLeg);
				CyclicDescendSolver.Process(this.lLeg);
				CyclicDescendSolver.Process(this.rArm);
				CyclicDescendSolver.Process(this.lArm);
				for (int j = 0; j < this.otherChains.Length; j++)
				{
					CyclicDescendSolver.Process(this.otherChains[j]);
				}
			}
			for (int k = 0; k < this.otherKChains.Length; k++)
			{
				ChainKinematicSolver.Process(this.otherKChains[k]);
			}
		}

		// Token: 0x06002755 RID: 10069 RVA: 0x000B66AC File Offset: 0x000B48AC
		public void DetectRig()
		{
			if (!this.animator)
			{
				this.animator = base.GetComponent<Animator>();
			}
			this.rigReader = new RigReader(this.animator);
		}

		// Token: 0x06002756 RID: 10070 RVA: 0x000B66D8 File Offset: 0x000B48D8
		public void BuildRig()
		{
			this.rArm = this.rigReader.RightArmChain();
			this.lArm = this.rigReader.LeftArmChain();
			this.rLeg = this.rigReader.RightLegChain();
			this.lLeg = this.rigReader.LeftLegChain();
		}

		// Token: 0x0400297D RID: 10621
		public Core.Solvers solver;

		// Token: 0x0400297E RID: 10622
		public Core.Chain rArm;

		// Token: 0x0400297F RID: 10623
		public Core.Chain lArm;

		// Token: 0x04002980 RID: 10624
		public Core.Chain rLeg;

		// Token: 0x04002981 RID: 10625
		public Core.Chain lLeg;

		// Token: 0x04002982 RID: 10626
		public Core.Chain[] otherChains;

		// Token: 0x04002983 RID: 10627
		public Core.KinematicChain[] otherKChains;

		// Token: 0x04002984 RID: 10628
		public RigReader rigReader;

		// Token: 0x04002985 RID: 10629
		public Animator animator;
	}
}
