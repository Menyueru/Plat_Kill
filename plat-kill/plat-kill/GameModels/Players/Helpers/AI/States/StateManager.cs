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

        public IState CurrentState
        {
            set { currentState = value; }
        }


        public IState PrevioustState
        {
            set { previousState = value; }
        }

        public StateManager(AIPlayer bot)
        {
            this.OwnerBot = bot;
            this.currentState = null;
            this.previousState = null;
            this.Init();
        }

        public void Init()
        {
            ChangeState(new GetWeaponState());
            //TODO
        }

        public void Update()
        {
            //TODO Global State

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
