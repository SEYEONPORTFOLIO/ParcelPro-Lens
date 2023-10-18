// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Events;
using Pnc.Model.Hands;
using UnityEngine;

namespace Pnc.UI.Button
{
    [AddComponentMenu("Scripts/MetaCore/SDK/TouchHandler")]
    public class TouchHandler : MonoBehaviour, IMetaCoreTouchHandler
    {
        [System.Serializable]
        public class TouchEvent : UnityEvent<HandsResult> { }
        
        #region Event handlers
        public TouchEvent OnTouchStarted = new TouchEvent();
        public TouchEvent OnTouchCompleted = new TouchEvent();
        public TouchEvent OnTouchUpdated = new TouchEvent();
        #endregion


        void IMetaCoreTouchHandler.OnTouchCompleted(HandsResult eventData)
        {
            OnTouchCompleted.Invoke(eventData);
        }

        void IMetaCoreTouchHandler.OnTouchStarted(HandsResult eventData)
        {
            OnTouchStarted.Invoke(eventData);
        }

        void IMetaCoreTouchHandler.OnTouchUpdated(HandsResult eventData)
        {
            OnTouchUpdated.Invoke(eventData);
        }
    }
}
