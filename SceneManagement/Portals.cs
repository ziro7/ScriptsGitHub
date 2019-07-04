namespace RPG.SceneManagement
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Portals : MonoBehaviour {
        
        public static Dictionary<DestinationIdentifer, Portal> portals = new Dictionary <DestinationIdentifer, Portal>();

        public static void AddPortal(DestinationIdentifer location, Portal portal)
        {
            if (portals.ContainsKey(location)) { return; }
            portals.Add(location, portal);
        }
    }
}
