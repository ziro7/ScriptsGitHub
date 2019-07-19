using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Control
{
    public class BossBehavior : MonoBehaviour
    {
        [SerializeField] DestinationIdentifer[] portalsToEnableInScene = null;
        [SerializeField] DestinationIdentifer[] portalsToEnableInOtherScenes = null;

        public DestinationIdentifer[] PortalsToEnableInScene { get => portalsToEnableInScene; private set => portalsToEnableInScene = value; }
        public DestinationIdentifer[] PortalsToEnableInOtherScenes { get => portalsToEnableInOtherScenes; set => portalsToEnableInOtherScenes = value; }

    }    
}

