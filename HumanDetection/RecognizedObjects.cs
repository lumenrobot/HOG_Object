using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanDetection
{
    class RecognizedObjects
    {
        [JsonProperty("@type")]
        public String type = "RecognizedObjects"; // Untuk menandai tipe data yang dikirim
        bool hasPosition = true;
        bool hasDistance = false;
        bool hasYaw = false;
        public List<RecognizedObject> trashes = new List<RecognizedObject>();
        public List<RecognizedObject> trashCans = new List<RecognizedObject>();
    }
}
