using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FSMState((int)PlayerController.PlayerState.Move)]
public class PlayerMoveState : UnitState<PlayerController>
{
	public override void Begin(PlayerController pc)
	{
		//base.Begin(pc);
		pc.animator.SetBool("Move", true);
	}

	public override void Update(PlayerController pc)
	{
		if(pc.moveDir == Vector3.zero)
		{
			pc.ChangeState(PlayerController.PlayerState.Idle);
		}
	}

	public override void FixedUpdate(PlayerController pc)
	{
		//if (!pc.IsCurrentState(PlayerController.PlayerState.Dash) && !pc.IsCurrentState(PlayerController.PlayerState.Attack) && !pc.IsCurrentState(PlayerController.PlayerState.AttackDelay))
		//{
			Vector3 rotVec = Quaternion.AngleAxis(45, Vector3.up) * pc.moveDir;
			pc.transform.rotation = Quaternion.Lerp(pc.transform.rotation, Quaternion.LookRotation(rotVec), 15.0f * Time.deltaTime);
			
			pc.transform.position += rotVec.normalized * 2.5f * pc.playerData.Speed * Time.deltaTime;
		//}
	}

	public override void End(PlayerController pc)
	{
		//base.End(pc);
		pc.animator.SetBool("Move", false);
		pc.rigid.velocity = Vector3.zero;
	}

	public override void OnTriggerEnter(PlayerController unit, Collider other)
	{
		throw new System.NotImplementedException();
	}
}
