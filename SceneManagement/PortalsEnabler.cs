using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class PortalsEnabler : MonoBehaviour
    {
        public static Dictionary<DestinationIdentifer, Boolean> PortalsEnabled = new Dictionary<DestinationIdentifer, Boolean>();
    }
}