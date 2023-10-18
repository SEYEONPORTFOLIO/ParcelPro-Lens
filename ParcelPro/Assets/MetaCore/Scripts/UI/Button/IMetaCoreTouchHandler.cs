using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pnc.Model.Hands;
using UnityEngine.EventSystems;

namespace Pnc.UI.Button
{
    public interface IMetaCoreTouchHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a Touch motion has occurred, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchStarted(HandsResult eventData);

        /// <summary>
        /// When a Touch motion ends, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchCompleted(HandsResult eventData);

        /// <summary>
        /// When a Touch motion is updated, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchUpdated(HandsResult eventData);
    }
}
