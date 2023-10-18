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
    [System.Serializable]
    public class PncApi
    {
      public const String INIT_PARAM_CAMERA_ID = "cameraId";
      public const String INIT_PARAM_CAMERA_ROTATION = "cameraRotation";
      public const String INIT_PARAM_STT_MIC_ID = "micId"; //[STT전용] 사용할 mic ID
      public const String INIT_PARAM_STT_LANGUAGE = "language";  //[STT전용] 처리할 언어
      public const String INIT_PARAM_STT_COMMAND = "sttCommand";  //[STT전용] 처리해야할 명령어 목록(명령어 사이는 ','쉼표로 구분)
                                                                              //  "e.g) 실행,중지,홈화면"
      public string name { get; set; }     // API name, 호출할 API 명
      public string serviceType { get; set; }     // API parameter, 서비스종류: gesture/slam/...
      public string uniqueId { get; set; }     // API parameter, 서비스 고유ID
      public string cameraId { get; set; }     // API parameter, 사용할 카메라ID
      public string language { get; set; }     // API parameter, stt에서 사용할 언어
      public string micId { get; set; }     // API parameter, stt에서 사용할 mic ID
      public string sttCommand { get; set; }     // API parameter, stt에서 처리되어야 하는 명령어 목록

      public string cameraRotation { get; set; }     // API parameter, 사용할 카메라 회전상태, A21M은 D221114 이전에는 270도 회전되어 있음, 이후는 0으로 변경됨

      // public static PncApi deserialize(string jsonString)
      // {
      //   PncApi model = JsonConvert.DeserializeObject<PncApi>(jsonString);
      //   return model;
      // }

      // public static string serialize(PncApi jsonObject)
      // {
      //   return JsonConvert.SerializeObject(jsonObject);
      // }


      public string info()
      {
        string buff = string.Empty;

        buff = $"[PncApi name={name}, serviceType={serviceType}, uniqueId={uniqueId}, cameraId={cameraId}, cameraRotation={cameraRotation}, sttCommand={sttCommand} ";
        return buff;
      }

    }
  } //model
} //pnc
