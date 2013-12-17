using Microsoft.Xna.Framework;
using plat_kill.GameModels.Players.Helpers.AI;
using plat_kill.GameModels.Players.Helpers.AI.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players
{
    public class AIPlayer : Player
    {
        private PKGame game;
        private SearchNode crumb2;
        private Vector2 movingTowards;
        private StateManager stateManager;
        public bool gotWeapon;
        private Player target;

        #region Getter-Setter
        public StateManager StateManager
        {
            get { return stateManager; }
            set { stateManager = value; }
        }

        public Player Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector2 MovingTowards
        {
            get { return movingTowards; }
            set { movingTowards = value; }
        }


        public PKGame Game
        {
            get { return game; }
        }

        public SearchNode Crumb2
        {
            get { return crumb2; }
            set { crumb2 = value; }
        }
#endregion

        public AIPlayer(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length, bool isLocal, PKGame game)
            : base(id, health, stamina, defense, meleePower, rangePower, speed, jumpSpeed, position, rotationSpeed, mass, width, height, length, isLocal)
        {
            this.game = game;
            this.StateManager = new StateManager(this);
            this.movingTowards = new Vector2(this.Position.X,this.Position.Z);
            this.gotWeapon = true;
        }

        private void FollowGraph(GameTime gametime)
        {
            Vector2 direction = movingTowards - new Vector2(this.Position.X, this.Position.Z);
            direction.Normalize();
            this.CharacterController.HorizontalMotionConstraint.MovementDirection = direction;
            float radianAngle = (float)Math.Acos(Vector2.Dot(direction,new Vector2(World.Backward.X,World.Backward.Z)));
            if (Math.Abs(radianAngle) > 0.1)
            {
                radianAngle = MathHelper.WrapAngle(radianAngle);
                this.rotation -= new Vector3(0, radianAngle * (float)gametime.ElapsedGameTime.TotalSeconds, 0);
            }
        }

        

        public void Update(GameTime gametime)
        {
            this.UpdateState();
            base.Update(gametime);

            stateManager.Update();
            FollowGraph(gametime);
        }
    }
}
