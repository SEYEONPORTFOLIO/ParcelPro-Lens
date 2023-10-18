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
    namespace Stt
    {
      [System.Serializable]
      public class EventSttResult : PncEvent
      {
        public SttResult sttResult;

        public SttResult getSttResult()
        {
          return sttResult;
        }

        public void getSttResult(SttResult sttResult)
        {
          this.sttResult = sttResult;
        }

        public string info()
        {
          string buff = string.Empty;

          buff = $"EventSttResult=[owner={getOwner()}, event type={getEventType()}" 
          + $", timestamp={getTimestamp()}, slamResult={sttResult.info()}] ";

          return buff;
        }        
      }
    }// slam
  } //model
} //pnc
