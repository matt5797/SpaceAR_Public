using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace SpaceAR.Core.Network
{
    /// <summary>
    /// Replace with real PhotonTransformView
    /// </summary>
    public class MockPhotonTransformView : MonoBehaviour
    {
        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = true;
        public bool m_UseLocal = false;
    }
}
