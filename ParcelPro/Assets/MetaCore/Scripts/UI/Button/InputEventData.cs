// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Pnc.Model;
using Pnc.Model.Hands;
//using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.EventSystems;

namespace Pnc.UI.Button
{
    /// <summary>
    /// Describes an Input Event that has a source id.
    /// </summary>
    public class InputEventData
    {
        /// <summary>
        /// Handedness of the <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource"/>.
        /// </summary>
        public HandsResult Handedness { get; private set; } = null;

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(HandsResult handedness)
        {
            Handedness = handedness;
        }
    }

    /// <summary>
    /// Describes and input event with a specific type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputEventData<T> : InputEventData
    {
        /// <summary>
        /// The input data of the event.
        /// </summary>
        public T InputData { get; private set; }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        public void Initialize(HandsResult handedness, T data)
        {
            Initialize(handedness);
            InputData = data;
        }
    }
}
