using UnityEngine;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
  namespace Model
  {
    namespace Hands
    {
      [System.Serializable]
      public class EventHandsResult : PncEvent
      {
        public HandsResult handsResult;

        public HandsResult getHandsResult()
        {
          return handsResult;
        }

        public void setHandsResult(HandsResult handsResult)
        {
          this.handsResult = handsResult;
        }

        public string info()
        {
          string buff = string.Empty;

          buff = $"EventHandsResult=[owner={getOwner()}, event type={getEventType()}" 
          + $", timestamp={getTimestamp()}, handresult={handsResult.info()}] ";

          return buff;
        }
      }
    }// hands
  } //model
} //pnc
