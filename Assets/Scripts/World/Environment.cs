﻿using System.Collections.Generic;
using Assets.Scripts.Units;

namespace Assets.Scripts.World
{
    public class Environment
    {
        private readonly Dictionary<UnitTypes, float> modifiers;
        public EnvironmentGameObject EnvironmentGameObject { get; private set; }
        public bool IsWalkable { get; set; }

        public Environment(EnvironmentGameObject game, bool isWalkable, Dictionary<UnitTypes, float> modifiers)
        {
            EnvironmentGameObject = game;
            IsWalkable = isWalkable;
            this.modifiers = modifiers;
        }

        /// <summary>
        /// Returns the environment modifier for the given UnitTypes.
        /// This method should not be called directly. The Unit class has the method GetGroundModifier() which does some checks and if need be calls this method.
        /// </summary>
        /// <param Name="type"></param>
        /// <returns></returns>
        public float GetEnvironmentModifier(UnitTypes type)
        {
            return modifiers[type];
        }
    }
}