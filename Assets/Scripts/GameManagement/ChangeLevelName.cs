﻿using UnityEngine;

namespace GameManagement
{
    public class ChangeLevelName : MonoBehaviour
    {
        private LevelLoader levelLoader;

        private void Awake() => levelLoader = GetComponentInParent<LevelLoader>();

        public void ChangeName() => levelLoader.DisplayLevelName();
    }
}