using Microsoft.Xna.Framework;
using plat_kill.GameModels.Players.Helpers.AI;
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

        public AIPlayer(long id, long health, long stamina, long defense, long meleePower, long rangePower, long speed, long jumpSpeed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length, bool isLocal, PKGame game)
            : base(id, health, stamina, defense, meleePower, rangePower, speed, jumpSpeed, position, rotationSpeed, mass,width,height,length, isLocal)
        {
            this.game = game;
        }

        private void FollowGraph()
        {
            Vector2 direction = movingTowards - new Vector2(this.Position.X,this.Position.Z);
            direction.Normalize();
            this.CharacterController.HorizontalMotionConstraint.MovementDirection = direction;
            float radianAngle = (float)Math.Acos(Vector3.Dot(this.World.Backward, new Vector3(direction.X,0, direction.Y)));
            if (radianAngle >= MathHelper.PiOver4/2)
            {
                this.rotation -= new Vector3(0, 100f * radianAngle, 0);
            }
        }

        public void Update(GameTime gametime)
        {
            base.Update(gametime);
            if (crumb2 == null)
            {
                Vector3 temp = game.PlayerManager.nextSpawnPoint();
                crumb2 = PathFinder.FindPath(game.Place, new Point3D((int)this.Position.X / 20, 0, (int)this.Position.Z / 20),
                                                            new Point3D((int)temp.X / 20, 0, (int)temp.Z / 20));
            }
            else
            {
                if (crumb2.position == new Point3D((int)this.Position.X / 20, 0, (int)this.Position.Z / 20))
                {
                    crumb2 = crumb2.next;
                    if(crumb2!=null)
                    movingTowards = new Vector2((crumb2.position.X * 20) + 10, (crumb2.position.Z * 20) + 10);
                }
                FollowGraph();
            }
        }
    }
}
