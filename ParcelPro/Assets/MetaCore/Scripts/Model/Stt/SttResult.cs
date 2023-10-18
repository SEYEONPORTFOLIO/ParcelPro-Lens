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
      public class SttResult
      {
        public string detectText;
        public string resultText;
        public int resultIndex;

        public string getDetectText() {
          return detectText;
        }

        public void setDetectText(string detectText) {
            this.detectText = detectText;
        }

        public string getResultText() {
          return resultText;
        }

        public void setResultText(string resultText) {
            this.resultText = resultText;
        }

        public int getResultIndex() {
            return resultIndex;
        }

        public void setResultIndex(int resultIndex) {
            this.resultIndex = resultIndex;
        }

        public string info()
        {
          string buff = string.Empty;

          buff = $"[SttResult detectText={getDetectText()}, resultText={getResultText()}, resultIndex={resultIndex}";
          return buff;
        }        
      }
    }// stt
  } //model
} //pnc
