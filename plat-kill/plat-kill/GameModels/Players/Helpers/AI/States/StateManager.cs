using BEPUphysics;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players.Helpers.AI.States
{
    public class StateManager
    {
        IState currentState;
        IState previousState;
        AIPlayer OwnerBot;
        private float viewAngle;
        private float viewDistance;

        public IState CurrentState
        {
            set { currentState = value; }
        }


        public IState PrevioustState
        {
            set { previousState = value; }
        }

        public StateManager(AIPlayer bot, String difficulty)
        {
            this.OwnerBot = bot;
            this.currentState = null;
            this.previousState = null;
            this.Init();
            if(difficulty.Equals("Easy"))
            {
                this.viewAngle=MathHelper.PiOver2;
                this.viewDistance=75;
            }
            else if (difficulty.Equals("Medium"))
            {
                this.viewAngle=MathHelper.PiOver2;
                this.viewDistance=150;
            }
            else if (difficulty.Equals("Hard"))
            {
                this.viewAngle = MathHelper.Pi;
                this.viewDistance = 200;
            }
        }

        public void Init()
        {
            ChangeState(new GetWeaponState());
        }

        public void Update()
        {

            if (OwnerBot.Health > 0 && !OwnerBot.IsDead && !(currentState is AttackState))
            {
                if (OwnerBot.EquippedWeapons.Count > 0 )
                {
                    Vector3 temprotation = OwnerBot.Rotation;
                    temprotation.Normalize();
                    float angle = temprotation.Y;
                    float mindist = 9999999;
                    long minID = -1;
                    foreach (var player in OwnerBot.Game.PlayerManager.Players)
                    {
                        if (player.Id != OwnerBot.Id)
                        {
                            Vector3 dir = player.Position - OwnerBot.Position;
                            dir.Normalize();
                            
                            float angle2 = dir.Y;
                            float a = (angle - angle2 + MathHelper.Pi) % MathHelper.TwoPi - MathHelper.Pi;
                            if (a <= viewAngle && a >= -viewAngle)
                            {
                                Ray ray = new Ray(OwnerBot.Position + new Vector3(0, OwnerBot.CharacterController.Body.Height / 2, 0),
                                        dir);
                                RayCastResult result;
                                if (OwnerBot.Game.Space.RayCast(ray, candidate => candidate != OwnerBot.CharacterController.Body.CollisionInformation, out result))
                                {
                                    float distance = result.HitData.T * ray.Direction.Length();
                                    if (distance < viewDistance)
                                    {
                                        var obj = result.HitObject as EntityCollidable;
                                        if (obj != null)
                                        {
                                            var person = obj.Entity.Tag as Player;
                                            if (person != null && person.Id == player.Id)
                                            {
                                                mindist = Math.Min(mindist, distance);
                                                if (mindist == distance)
                                                    minID = player.Id;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (minID != -1)
                    {
                        OwnerBot.Target = OwnerBot.Game.PlayerManager.GetPlayer(minID);
                    }

                }
                if (OwnerBot.LastHit != OwnerBot.Id)
                {
                    OwnerBot.Target = OwnerBot.Game.PlayerManager.GetPlayer(OwnerBot.LastHit);
                }

                if(OwnerBot.Target!=null && !(currentState is AttackState)) OwnerBot.StateManager.ChangeState(new AttackState());
                
            }
            if (!(currentState is DeadState) && (OwnerBot.Health <= 0 || OwnerBot.IsDead)) ChangeState(new DeadState());
            if (currentState != null) currentState.Update(this.OwnerBot);
        }

        public void ChangeState(IState newState)
        {
            if (newState == null)
            {
                throw new Exception("Trying to Pass Null State");
            }
            previousState = currentState;
            if(currentState!=null)
                currentState.End(OwnerBot);
            currentState = newState;
            currentState.Start(OwnerBot);
        }

        public void RevertState()
        {
            ChangeState(previousState);
        }

    }
}
