// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

using Pnc.Model.Hands;

namespace Pnc.UI.Button
{
    public class HandTrackingInputEventData : InputEventData<Vector3>
    {
        /// <summary>
        /// Constructor creates a default EventData object.
        /// Requires initialization.
        /// </summary>
        public HandTrackingInputEventData(EventSystem eventSystem) { }

        public HandsResult Controller { get; set; }

        /// <summary>
        /// This function is called to fill the HandTrackingIntputEventData object with information
        /// </summary>
        /// <param name="sourceHandedness">Handedness of the HandTrackingInputSource that created the EventData</param>
        /// <param name="touchPoint">Global position of the HandTrackingInputSource that created the EventData</param>
        public void Initialize(HandsResult sourceHandedness, Vector3 touchPoint)
        {
            Initialize(sourceHandedness, touchPoint);
        }
    }
}
