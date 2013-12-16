﻿using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionTests;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using plat_kill.Events;
using plat_kill.GameModels.Players;
using plat_kill.GameModels.Weapons;
using plat_kill.Helpers.Serializable.Weapons;
using plat_kill.Networking;
using plat_kill.Networking.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace plat_kill.Managers
{
    public class WeaponManager
    {
        public event EventHandler<WeaponHasBeenCreated> WeaponHasBeenCreated;
//        public event EventHandler<WeaponHasBeenLooted> WeaponHasBeenLooted;

        private Dictionary<long, Tuple<Weapon, Box>> activeWeapons;

        public Dictionary<long, Tuple<Weapon, Box>> ActiveWeapons
        {
            get { return activeWeapons; }
            set { activeWeapons = value; }
        }
        private SerializableWeapon[] differentWeapons;
        private List<Vector3> spawnPoints;
        private TimeSpan ReloadWeapons;
        private DateTime LastWeaponReload;
        private PKGame game;

        public List<Vector3> SpawnPoints
        {
            set { spawnPoints = value; }
        }

        public WeaponManager(PKGame game)
        {
            this.game = game;
            this.differentWeapons = DeserializeCharacterCollection("Content\\Weapons\\Weapons.xml").Weapons;
            this.ReloadWeapons = new TimeSpan(0,0,5);
            this.spawnPoints = new List<Vector3>();
            this.activeWeapons = new Dictionary<long, Tuple<Weapon, Box>>();
        }

        public void Init()
        {
            this.LastWeaponReload = DateTime.Now;
        }

        public void Update()
        {

            if (game.NetworkManager == null || game.NetworkManager.GetType().Equals(typeof(ServerNetworkManager)))
            {
                if (LastWeaponReload.Add(ReloadWeapons) < DateTime.Now)
                {
                    Random randomizer = new Random();
                    int quantity = randomizer.Next((spawnPoints.Count - ActiveWeapons.Count) + 1);
                    if (ReloadWeapons.Seconds == 5) quantity = spawnPoints.Count;
                    for (int i = 0; i < spawnPoints.Count && quantity > 0; i++)
                    {
                        if (!ActiveWeapons.ContainsKey(i))
                        {
                            var temp = new Tuple<Weapon, Box>(createWeapon(differentWeapons[randomizer.Next(differentWeapons.Length)]),
                                                                new Box(spawnPoints[i], 5, 5, 5));
                            
                            ActiveWeapons.Add(i, temp);
                            ActiveWeapons[i].Item2.Tag = i;
                            ActiveWeapons[i].Item2.CollisionInformation.Events.ContactCreated += ContactCreated;
                            ActiveWeapons[i].Item2.CollisionInformation.CollisionRules.Personal = BEPUphysics.CollisionRuleManagement.CollisionRule.NoSolver;
                            game.Space.Add(ActiveWeapons[i].Item2);
                        }
                    }

                    ReloadWeapons = new TimeSpan(0, 0, randomizer.Next(20, 61));
                    LastWeaponReload = DateTime.Now;
                }
            }
        }

        private void ContactCreated(EntityCollidable sender, Collidable other, CollidablePairHandler pair, ContactData contact)
        {
            Box weapon = sender.Entity as Box;
            if (weapon != null)
            {
                var otherEntityInformation = other as EntityCollidable;
                if (otherEntityInformation != null)
                {
                    Player p = otherEntityInformation.Entity.Tag as Player;
                    if (p != null)
                    {
                        p.addWeapon(pickupWeapon(Convert.ToInt64(weapon.Tag)));
                    }
                }
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            foreach (long key in ActiveWeapons.Keys)
            {
                ActiveWeapons[key].Item1.DrawOnFloor(spawnPoints[(int)key],view, projection);
            }
        }

        public Weapon pickupWeapon(long weapon)
        {
            var temp = ActiveWeapons[weapon];
            ActiveWeapons.Remove(weapon);
            game.Space.Remove(temp.Item2);
            return temp.Item1;
        }

        public Weapon GetWeapon(long weaponIndex) 
        {
            var temp = createWeapon(differentWeapons[weaponIndex]);
             return temp;
        }

        public void AddWeapon(Vector3 weaponPosition, long weaponID)
        {
            var temp = new Tuple<Weapon, Box>(createWeapon(differentWeapons[weaponID]),  new Box(weaponPosition, 5, 5, 5));

            
            ActiveWeapons.Add(ActiveWeapons.Count, temp);
        }

        private Weapon createWeapon(SerializableWeapon weapon)
        {
            return new Weapon(Array.IndexOf(differentWeapons, weapon),game.Content, weapon.modelPath,"Models\\Objects\\"+weapon.modelPath, weapon.weaponType, weapon.projectileType, weapon.weaponDamage, weapon.fireRate, weapon.loadedAmmo, weapon.totalAmmo);
        }

        private WeaponCollection DeserializeCharacterCollection(string xmlPath)
        {
            WeaponCollection tempCollection = null;

            XmlSerializer serializer = new XmlSerializer(typeof(WeaponCollection));

            StreamReader reader = new StreamReader(xmlPath);
            tempCollection = (WeaponCollection)serializer.Deserialize(reader);
            reader.Close();
            return tempCollection;
        }

        protected void OnWeaponCreated(Weapon weapon)
        {
            EventHandler<WeaponHasBeenCreated> weaponHasBeenCreated = this.WeaponHasBeenCreated;
            if (weaponHasBeenCreated != null)
            {
                weaponHasBeenCreated(this, new WeaponHasBeenCreated(weapon));
            }
        }

       /* protected void OnWeaponLooted(Weapon weapon)
        {
            EventHandler<WeaponHasBeenLooted> weaponHasBeenLooted = this.WeaponHasBeenLooted;
            if (weaponHasBeenLooted != null)
            {
                weaponHasBeenLooted(this, new WeaponHasBeenLooted(weapon));
            }
        }*/
    }
}
