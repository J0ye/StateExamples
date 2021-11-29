using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum SimpleState { Walking, Airborn, DoubleJumped, Ducking }

public class State
{
    public virtual void HandleInput(StatePatternPlayer player){}

    public virtual void Update(StatePatternPlayer player){}

    public virtual void Exit(StatePatternPlayer player, State next)
    {
        player.state = next;
        player.stateName = nameof(next);
    }
}


public class Walking : State
{
    public override void HandleInput(StatePatternPlayer player)
    {
        if (player.GetJumpInput())
        {
            Exit(player, StatePatternPlayer.airborn);
            player.stateName = "Airborn";
            Debug.Log("Changed to airborn");
            player.Jump();
        }
        else if (player.GetInputAxisRaw().z < 0)
        {
            Exit(player,StatePatternPlayer.ducking);
            player.stateName = "Ducking";
            player.Duck();
        }
    }
}

public class Airborn : State
{
    public override void HandleInput(StatePatternPlayer player)
    {
        if (player.GetJumpInput())
        {
            Exit(player,StatePatternPlayer.doubleJumped);
            player.stateName = "DoubleJumped";
            player.Jump();
        }
        else if (player.OnGround() && player.rb.velocity.y < 0)
        {
            Exit(player,StatePatternPlayer.walking);
            player.stateName = "Walking";
        }
    }
    
}

public class DoubleJumped : State
{
    public override void HandleInput(StatePatternPlayer player)
    {
        if (player.OnGround())
        {
            Exit(player,StatePatternPlayer.walking);
            player.stateName = "Walking";
        }
    }
}

public class Ducking : State
{
    public override void HandleInput(StatePatternPlayer player)
    {
        if (!player.OnGround())
        {
            Exit(player,StatePatternPlayer.airborn);
            player.stateName = "Airborn";
            player.ResetDuck();
        } else if(player.GetInputAxisRaw().z > -1)
        {
            Exit(player,StatePatternPlayer.walking);
            player.stateName = "Walking";
            player.ResetDuck();
        }
    }
}
