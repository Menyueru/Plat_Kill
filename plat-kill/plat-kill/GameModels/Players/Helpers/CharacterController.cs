﻿using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems;
using BEPUphysics;
using BEPUutilities;
using Microsoft.Xna.Framework;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Materials;
using BEPUphysics.PositionUpdating;
using System.Diagnostics;
using System.Threading;

namespace plat_kill.GameModels.Players.Helpers
{
    /// <summary>
    /// Gives a physical object FPS-like control, including stepping and jumping.
    /// This is more robust/expensive than the SimpleCharacterController.
    /// </summary>
    public class CharacterController : Updateable, IBeforeSolverUpdateable
    {
        /// <summary>
        /// Gets the physical body of the character.  Do not use this reference to modify the character's height and radius.  Instead, use the BodyRadius property and the StanceManager's StandingHeight and CrouchingHeight properties.
        /// </summary>
        public Cylinder Body { get; private set; }

        /// <summary>
        /// Gets the manager responsible for finding places for the character to step up and down to.
        /// </summary>
        public StepManager StepManager { get; private set; }

        /// <summary>
        /// Gets the manager responsible for crouching, standing, and the verification involved in changing states.
        /// </summary>
        public StanceManager StanceManager { get; private set; }

        /// <summary>
        /// Gets the support system which other systems use to perform local ray casts and contact queries.
        /// </summary>
        public QueryManager QueryManager { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to handle horizontal motion.  This includes acceleration due to player input and deceleration when the relative velocity
        /// between the support and the character exceeds specified maximums.
        /// </summary>
        public HorizontalMotionConstraint HorizontalMotionConstraint { get; private set; }

        /// <summary>
        /// Gets the constraint used by the character to stay glued to surfaces it stands on.
        /// </summary>
        public VerticalMotionConstraint VerticalMotionConstraint { get; private set; }

        private float jumpSpeed = 4.5f;
        /// <summary>
        /// Gets or sets the speed at which the character leaves the ground when it jumps.
        /// </summary>
        public float JumpSpeed
        {
            get
            {
                return jumpSpeed;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be nonnegative.");
                jumpSpeed = value;
            }
        }
        float slidingJumpSpeed = 3;
        /// <summary>
        /// Gets or sets the speed at which the character leaves the ground when it jumps without traction.
        /// </summary>
        public float SlidingJumpSpeed
        {
            get
            {
                return slidingJumpSpeed;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be nonnegative.");
                slidingJumpSpeed = value;
            }
        }
        float jumpForceFactor = 1f;
        /// <summary>
        /// Gets or sets the amount of force to apply to supporting dynamic entities as a fraction of the force used to reach the jump speed.
        /// </summary>
        public float JumpForceFactor
        {
            get
            {
                return jumpForceFactor;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be nonnegative.");
                jumpForceFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the body cylinder.  To change the height, use the StanceManager.StandingHeight and StanceManager.CrouchingHeight.
        /// </summary>
        public float BodyRadius
        {
            get { return Body.CollisionInformation.Shape.Radius; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Radius must be positive.");
                Body.CollisionInformation.Shape.Radius = value;
                //Tell the query manager to update its representation.
                QueryManager.UpdateQueryShapes();
            }
        }

        /// <summary>
        /// Gets the support finder used by the character.
        /// The support finder analyzes the character's contacts to see if any of them provide support and/or traction.
        /// </summary>
        public SupportFinder SupportFinder { get; private set; }



        /// <summary>
        /// Constructs a new character controller with the default configuration.
        /// </summary>
        public CharacterController()
            : this(new Vector3(), 1.7f, 1.7f * .7f, .6f, 10)
        {

        }


        /// <summary>
        /// Constructs a new character controller with the most common configuration options.
        /// </summary>
        /// <param name="position">Initial position of the character.</param>
        /// <param name="height">Height of the character body while standing.</param>
        /// <param name="crouchingHeight">Height of the character body while crouching.</param>
        /// <param name="radius">Radius of the character body.</param>
        /// <param name="mass">Mass of the character body.</param>
        public CharacterController(Vector3 position, float height, float crouchingHeight, float radius, float mass)
        {
            Body = new Cylinder(position, height, radius, mass);
            Body.IgnoreShapeChanges = true; //Wouldn't want inertia tensor recomputations to occur when crouching and such.
            Body.CollisionInformation.Shape.CollisionMargin = .1f;
            //Making the character a continuous object prevents it from flying through walls which would be pretty jarring from a player's perspective.
            Body.PositionUpdateMode = PositionUpdateMode.Continuous;
            Body.LocalInertiaTensorInverse = new Matrix3x3();
            //TODO: In v0.16.2, compound bodies would override the material properties that get set in the CreatingPair event handler.
            //In a future version where this is changed, change this to conceptually minimally required CreatingPair.
            Body.CollisionInformation.Events.DetectingInitialCollision += RemoveFriction;
            Body.LinearDamping = 0;
            SupportFinder = new SupportFinder(this);
            HorizontalMotionConstraint = new HorizontalMotionConstraint(this);
            VerticalMotionConstraint = new VerticalMotionConstraint(this);
            StepManager = new StepManager(this);
            StanceManager = new StanceManager(this, crouchingHeight);
            QueryManager = new QueryManager(this);

            //Enable multithreading for the characters.  
            IsUpdatedSequentially = false;
            //Link the character body to the character controller so that it can be identified by the locker.
            //Any object which replaces this must implement the ICharacterTag for locking to work properly.
            Body.CollisionInformation.Tag = new CharacterSynchronizer(Body);
        }


        List<ICharacterTag> involvedCharacters = new List<ICharacterTag>();
        public void LockCharacterPairs()
        {
            //If this character is colliding with another character, there's a significant danger of the characters
            //changing the same collision pair handlers.  Rather than infect every character system with micro-locks,
            //we lock the entirety of a character update.

            foreach (var pair in Body.CollisionInformation.Pairs)
            {
                //Is this a pair with another character?
                var other = pair.BroadPhaseOverlap.EntryA == Body.CollisionInformation ? pair.BroadPhaseOverlap.EntryB : pair.BroadPhaseOverlap.EntryA;
                var otherCharacter = other.Tag as ICharacterTag;
                if (otherCharacter != null)
                {
                    involvedCharacters.Add(otherCharacter);
                }
            }
            if (involvedCharacters.Count > 0)
            {
                //If there were any other characters, we also need to lock ourselves!
                involvedCharacters.Add((ICharacterTag)Body.CollisionInformation.Tag);

                //However, the characters cannot be locked willy-nilly.  There needs to be some defined order in which pairs are locked to avoid deadlocking.
                involvedCharacters.Sort(comparer);

                for (int i = 0; i < involvedCharacters.Count; ++i)
                {
                    Monitor.Enter(involvedCharacters[i]);
                }
            }
        }

        private static Comparer comparer = new Comparer();
        class Comparer : IComparer<ICharacterTag>
        {
            public int Compare(ICharacterTag x, ICharacterTag y)
            {
                if (x.InstanceId < y.InstanceId)
                    return -1;
                if (x.InstanceId > y.InstanceId)
                    return 1;
                return 0;
            }
        }

        public void UnlockCharacterPairs()
        {
            //Unlock the pairs, LIFO.
            for (int i = involvedCharacters.Count - 1; i >= 0; i--)
            {
                Monitor.Exit(involvedCharacters[i]);
            }
            involvedCharacters.Clear();
        }


        void RemoveFriction(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
        {
            var collidablePair = pair as CollidablePairHandler;
            if (collidablePair != null)
            {
                //The default values for InteractionProperties is all zeroes- zero friction, zero bounciness.
                //That's exactly how we want the character to behave when hitting objects.
                collidablePair.UpdateMaterialProperties(new InteractionProperties());
            }
        }

        void ExpandBoundingBox()
        {
            if (Body.ActivityInformation.IsActive)
            {
                //This runs after the bounding box updater is run, but before the broad phase.
                //Expanding the character's bounding box ensures that minor variations in velocity will not cause
                //any missed information.
                //For a character which is not bound to Vector3.Up (such as a character that needs to run around a spherical planet, a '6DOF' character),
                //the bounding box expansion needs to be changed such that it includes the full motion of the character.
                float radius = Body.CollisionInformation.Shape.CollisionMargin * 1.1f; //The character can teleport by its collision margin when stepping up.
#if WINDOWS
                Vector3 offset;
#else
                Vector3 offset = new Vector3();
#endif
                offset.X = radius;
                offset.Y = StepManager.MaximumStepHeight;
                offset.Z = radius;
                BoundingBox box = Body.CollisionInformation.BoundingBox;
                Vector3.Add(ref box.Max, ref offset, out box.Max);
                Vector3.Subtract(ref box.Min, ref offset, out box.Min);
                Body.CollisionInformation.BoundingBox = box;
            }


        }


        void CollectSupportData()
        {
            oldSupport = supportData;
            //Identify supports.
            SupportFinder.UpdateSupports();

            //Collect the support data from the support, if any.
            if (SupportFinder.HasSupport)
            {
                if (SupportFinder.HasTraction)
                    supportData = SupportFinder.TractionData.Value;
                else
                    supportData = SupportFinder.SupportData.Value;
            }
            else
                supportData = new SupportData();

        }

        SupportData supportData;
        SupportData oldSupport;

        public SupportData OldSupport
        {
            get { return oldSupport; }
            set { oldSupport = value; }
        }

        public SupportData SupportData
        {
            get { return supportData; }
            set { supportData = value; }
        }


        void IBeforeSolverUpdateable.Update(float dt)
        {
            //Someone may want to use the Body.CollisionInformation.Tag for their own purposes.
            //That could screw up the locking mechanism above and would be tricky to track down.
            //Consider using the making the custom tag implement ICharacterTag, modifying LockCharacterPairs to analyze the different Tag type, or using the Entity.Tag for the custom data instead.
            Debug.Assert(Body.CollisionInformation.Tag is ICharacterTag, "The character.Body.CollisionInformation.Tag must implement ICharacterTag to link the CharacterController and its body together for character-related locking to work in multithreaded simulations.");

            //We can't let multiple characters manage the same pairs simultaneously.  Lock it up!
            LockCharacterPairs();
            try
            {
                CorrectContacts();

                bool hadSupport = SupportFinder.HasSupport;

                CollectSupportData();


                //Compute the initial velocities relative to the support.
                Vector3 relativeVelocity;
                ComputeRelativeVelocity(ref supportData, out relativeVelocity);
                float verticalVelocity = Vector3.Dot(supportData.Normal, relativeVelocity);


                //Don't attempt to use an object as support if we are flying away from it (and we were never standing on it to begin with).
                if (SupportFinder.HasSupport && !hadSupport && verticalVelocity < 0)
                {
                    SupportFinder.ClearSupportData();
                    supportData = new SupportData();
                }


                //Attempt to jump.
                if (tryToJump && StanceManager.CurrentStance != Stance.Crouching) //Jumping while crouching would be a bit silly.
                {
                    //In the following, note that the jumping velocity changes are computed such that the separating velocity is specifically achieved,
                    //rather than just adding some speed along an arbitrary direction.  This avoids some cases where the character could otherwise increase
                    //the jump speed, which may not be desired.
                    if (SupportFinder.HasTraction)
                    {
                        //The character has traction, so jump straight up.
                        float currentUpVelocity = Vector3.Dot(Body.OrientationMatrix.Up, relativeVelocity);
                        //Target velocity is JumpSpeed.
                        float velocityChange = Math.Max(jumpSpeed - currentUpVelocity, 0);
                        ApplyJumpVelocity(ref supportData, Body.OrientationMatrix.Up * velocityChange, ref relativeVelocity);


                        //Prevent any old contacts from hanging around and coming back with a negative depth.
                        foreach (var pair in Body.CollisionInformation.Pairs)
                            pair.ClearContacts();
                        SupportFinder.ClearSupportData();
                        supportData = new SupportData();
                    }
                    else if (SupportFinder.HasSupport)
                    {
                        //The character does not have traction, so jump along the surface normal instead.
                        float currentNormalVelocity = Vector3.Dot(supportData.Normal, relativeVelocity);
                        //Target velocity is JumpSpeed.
                        float velocityChange = Math.Max(slidingJumpSpeed - currentNormalVelocity, 0);
                        ApplyJumpVelocity(ref supportData, supportData.Normal * -velocityChange, ref relativeVelocity);

                        //Prevent any old contacts from hanging around and coming back with a negative depth.
                        foreach (var pair in Body.CollisionInformation.Pairs)
                            pair.ClearContacts();
                        SupportFinder.ClearSupportData();
                        supportData = new SupportData();
                    }
                }
                tryToJump = false;


                //Try to step!
                Vector3 newPosition;
                if (StepManager.TryToStepDown(out newPosition) ||
                    StepManager.TryToStepUp(out newPosition))
                {
                    TeleportToPosition(newPosition, dt);
                }

                if (StanceManager.UpdateStance(out newPosition))
                {
                    TeleportToPosition(newPosition, dt);
                }
            }
            finally
            {
                UnlockCharacterPairs();
            }

            //if (SupportFinder.HasTraction && SupportFinder.Supports.Count == 0)
            //{
            //There's another way to step down that is a lot cheaper, but less robust.
            //This modifies the velocity of the character to make it fall faster.
            //Impacts with the ground will be harder, so it will apply superfluous force to supports.
            //Additionally, it will not be consistent with instant up-stepping.
            //However, because it does not do any expensive queries, it is very fast!

            ////We are being supported by a ray cast, but we're floating.
            ////Let's try to get to the ground faster.
            ////How fast?  Try picking an arbitrary velocity and setting our relative vertical velocity to that value.
            ////Don't go farther than the maximum distance, though.
            //float maxVelocity = (SupportFinder.SupportRayData.Value.HitData.T - SupportFinder.RayLengthToBottom);
            //if (maxVelocity > 0)
            //{
            //    maxVelocity = (maxVelocity + .01f) / dt;

            //    float targetVerticalVelocity = -3;
            //    verticalVelocity = Vector3.Dot(Body.OrientationMatrix.Up, relativeVelocity);
            //    float change = MathHelper.Clamp(targetVerticalVelocity - verticalVelocity, -maxVelocity, 0);
            //    ChangeVelocityUnilaterally(Body.OrientationMatrix.Up * change, ref relativeVelocity);
            //}
            //}



            //Vertical support data is different because it has the capacity to stop the character from moving unless
            //contacts are pruned appropriately.
            SupportData verticalSupportData;
            Vector3 movement3d = new Vector3(HorizontalMotionConstraint.MovementDirection.X, 0, HorizontalMotionConstraint.MovementDirection.Y);
            SupportFinder.GetTractionInDirection(ref movement3d, out verticalSupportData);


            //Warning:
            //Changing a constraint's support data is not thread safe; it modifies simulation islands!
            //If something other than a CharacterController can modify simulation islands is running
            //simultaneously (in the IBeforeSolverUpdateable.Update stage), it will need to be synchronized.

            //We don't need to synchronize this all the time- only when the support object changes.
            bool needToLock = HorizontalMotionConstraint.SupportData.SupportObject != supportData.SupportObject ||
                              VerticalMotionConstraint.SupportData.SupportObject != verticalSupportData.SupportObject;

            if (needToLock)
                CharacterSynchronizer.ConstraintAccessLocker.Enter();

            HorizontalMotionConstraint.SupportData = supportData;
            VerticalMotionConstraint.SupportData = verticalSupportData;

            if (needToLock)
                CharacterSynchronizer.ConstraintAccessLocker.Exit();




        }

        void TeleportToPosition(Vector3 newPosition, float dt)
        {

            Body.Position = newPosition;
            var orientation = Body.Orientation;
            //The re-do of contacts won't do anything unless we update the collidable's world transform.
            Body.CollisionInformation.UpdateWorldTransform(ref newPosition, ref orientation);
            //Refresh all the narrow phase collisions.
            foreach (var pair in Body.CollisionInformation.Pairs)
            {
                //Clear out the old contacts.  This prevents contacts in persistent manifolds from surviving the step
                //Such old contacts might still have old normals which blocked the character's forward motion.

                pair.ClearContacts();
                pair.UpdateCollision(dt);

            }
            //Also re-collect supports.
            //This will ensure the constraint and other velocity affectors have the most recent information available.
            CollectSupportData();
        }

        void CorrectContacts()
        {
            //Go through the contacts associated with the character.
            //If the contact is at the bottom of the character, regardless of its normal, take a closer look.
            //If the direction from the closest point on the inner cylinder to the contact position has traction
            //and the contact's normal does not, then replace the contact normal with the offset direction.

            //This is necessary because various convex pair manifolds use persistent manifolds.
            //Contacts in these persistent manifolds can live too long for the character to behave perfectly
            //when going over (usually tiny) steps.

            Vector3 downDirection = Body.OrientationMatrix.Down;
            Vector3 position = Body.Position;
            float margin = Body.CollisionInformation.Shape.CollisionMargin;
            float minimumHeight = Body.Height * .5f - margin;
            float coreRadius = Body.Radius - margin;
            float coreRadiusSquared = coreRadius * coreRadius;
            foreach (var pair in Body.CollisionInformation.Pairs)
            {
                foreach (var contactData in pair.Contacts)
                {
                    var contact = contactData.Contact;
                    float dot;
                    //Vector3.Dot(ref contact.Normal, ref downDirection, out dot);
                    //if (Math.Abs(dot) > SupportFinder.cosMaximumSlope)
                    //{
                    //    //This contact will already be considered to have traction.
                    //    //Don't bother doing the somewhat expensive correction process on it.
                    //    //TODO: Test this; see how much benefit there is.
                    //    continue;
                    //}
                    //Check to see if the contact position is at the bottom of the character.
                    Vector3 offset = contact.Position - Body.Position;
                    Vector3.Dot(ref offset, ref downDirection, out dot);
                    if (dot > minimumHeight)
                    {

                        //It is a 'bottom' contact!
                        //So, compute the offset from the inner cylinder to the contact.
                        //To do this, compute the closest point on the inner cylinder.
                        //Since we know it's on the bottom, all we need is to compute the horizontal offset.
                        Vector3.Dot(ref offset, ref downDirection, out dot);
                        Vector3 horizontalOffset;
                        Vector3.Multiply(ref downDirection, dot, out horizontalOffset);
                        Vector3.Subtract(ref offset, ref horizontalOffset, out horizontalOffset);
                        float length = horizontalOffset.LengthSquared();
                        if (length > coreRadiusSquared)
                        {
                            //It's beyond the edge of the cylinder; clamp it.
                            Vector3.Multiply(ref horizontalOffset, coreRadius / (float)Math.Sqrt(length), out horizontalOffset);
                        }
                        //It's on the bottom, so add the bottom height.
                        Vector3 closestPointOnCylinder;
                        Vector3.Multiply(ref downDirection, minimumHeight, out closestPointOnCylinder);
                        Vector3.Add(ref closestPointOnCylinder, ref horizontalOffset, out closestPointOnCylinder);
                        Vector3.Add(ref closestPointOnCylinder, ref position, out closestPointOnCylinder);

                        //Compute the offset from the cylinder to the offset.
                        Vector3 offsetDirection;
                        Vector3.Subtract(ref contact.Position, ref closestPointOnCylinder, out offsetDirection);
                        length = offsetDirection.LengthSquared();
                        if (length > Toolbox.Epsilon)
                        {
                            //Normalize the offset.
                            Vector3.Divide(ref offsetDirection, (float)Math.Sqrt(length), out offsetDirection);
                        }
                        else
                            continue; //If there's no offset, it's really deep and correcting this contact might be a bad idea.

                        Vector3.Dot(ref offsetDirection, ref downDirection, out dot);
                        float dotOriginal;
                        Vector3.Dot(ref contact.Normal, ref downDirection, out dotOriginal);
                        if (Math.Abs(dot) > Math.Abs(dotOriginal)) //if the new offsetDirection normal is less steep than the original slope...
                        {
                            //Then use it!
                            Vector3.Dot(ref offsetDirection, ref contact.Normal, out dot);
                            if (dot < 0)
                            {
                                //Don't flip the normal relative to the contact normal.  That would be bad!
                                Vector3.Negate(ref offsetDirection, out offsetDirection);
                                dot = -dot;
                            }
                            //Update the contact data using the corrected information.
                            //The penetration depth is conservatively updated; it will be less than or equal to the 'true' depth in this direction.
                            contact.PenetrationDepth *= dot;
                            contact.Normal = offsetDirection;
                        }
                    }
                }
            }

        }

        void ComputeRelativeVelocity(ref SupportData supportData, out Vector3 relativeVelocity)
        {

            //Compute the relative velocity between the body and its support, if any.
            //The relative velocity will be updated as impulses are applied.
            relativeVelocity = Body.LinearVelocity;
            if (SupportFinder.HasSupport)
            {
                //Only entities have velocity.
                var entityCollidable = supportData.SupportObject as EntityCollidable;
                if (entityCollidable != null)
                {
                    //It's possible for the support's velocity to change due to another character jumping if the support is dynamic.
                    //Don't let that happen while the character is computing a relative velocity!
                    Vector3 entityVelocity;
                    bool locked;
                    if (locked = entityCollidable.Entity.IsDynamic)
                        entityCollidable.Entity.Locker.Enter();
                    try
                    {
                        entityVelocity = Toolbox.GetVelocityOfPoint(supportData.Position, entityCollidable.Entity.Position, entityCollidable.Entity.LinearVelocity, entityCollidable.Entity.AngularVelocity);
                    }
                    finally
                    {
                        if (locked)
                            entityCollidable.Entity.Locker.Exit();
                    }
                    Vector3.Subtract(ref relativeVelocity, ref entityVelocity, out relativeVelocity);
                }
            }

        }

        /// <summary>
        /// Changes the relative velocity between the character and its support.
        /// </summary>
        /// <param name="supportData">Support data to use to jump.</param>
        /// <param name="velocityChange">Change to apply to the character and support relative velocity.</param>
        /// <param name="relativeVelocity">Relative velocity to update.</param>
        void ApplyJumpVelocity(ref SupportData supportData, Vector3 velocityChange, ref Vector3 relativeVelocity)
        {
            Body.LinearVelocity += velocityChange;
            var entityCollidable = supportData.SupportObject as EntityCollidable;
            if (entityCollidable != null)
            {
                if (entityCollidable.Entity.IsDynamic)
                {
                    Vector3 change = velocityChange * jumpForceFactor;
                    //Multiple characters cannot attempt to modify another entity's velocity at the same time.
                    entityCollidable.Entity.Locker.Enter();
                    try
                    {
                        entityCollidable.Entity.LinearMomentum += change * -Body.Mass;
                    }
                    finally
                    {
                        entityCollidable.Entity.Locker.Exit();
                    }
                    velocityChange += change;
                }
            }

            //Update the relative velocity as well.  It's a ref parameter, so this update will be reflected in the calling scope.
            Vector3.Add(ref relativeVelocity, ref velocityChange, out relativeVelocity);

        }

        /// <summary>
        /// In some cases, an applied velocity should only modify the character.
        /// This allows partially non-physical behaviors, like gluing the character to the ground.
        /// </summary>
        /// <param name="velocityChange">Change to apply to the character.</param>
        /// <param name="relativeVelocity">Relative velocity to update.</param>
        void ChangeVelocityUnilaterally(Vector3 velocityChange, ref Vector3 relativeVelocity)
        {
            Body.LinearVelocity += velocityChange;
            //Update the relative velocity as well.  It's a ref parameter, so this update will be reflected in the calling scope.
            Vector3.Add(ref relativeVelocity, ref velocityChange, out relativeVelocity);

        }






        bool tryToJump = false;
        /// <summary>
        /// Jumps the character off of whatever it's currently standing on.  If it has traction, it will go straight up.
        /// If it doesn't have traction, but is still supported by something, it will jump in the direction of the surface normal.
        /// </summary>
        public void Jump()
        {
            //The actual jump velocities are applied next frame.  This ensures that gravity doesn't pre-emptively slow the jump, and uses more
            //up-to-date support data.
            tryToJump = true;
        }

        public override void OnAdditionToSpace(ISpace newSpace)
        {
            //Add any supplements to the space too.
            newSpace.Add(Body);
            newSpace.Add(HorizontalMotionConstraint);
            newSpace.Add(VerticalMotionConstraint);
            //This character controller requires the standard implementation of Space.
            ((Space)newSpace).BoundingBoxUpdater.Finishing += ExpandBoundingBox;

            Body.AngularVelocity = new Vector3();
            Body.LinearVelocity = new Vector3();
        }
        public override void OnRemovalFromSpace(ISpace oldSpace)
        {
            //Remove any supplements from the space too.
            oldSpace.Remove(Body);
            oldSpace.Remove(HorizontalMotionConstraint);
            oldSpace.Remove(VerticalMotionConstraint);
            //This character controller requires the standard implementation of Space.
            ((Space)oldSpace).BoundingBoxUpdater.Finishing -= ExpandBoundingBox;
            SupportFinder.ClearSupportData();
            Body.AngularVelocity = new Vector3();
            Body.LinearVelocity = new Vector3();
        }


    }
}

