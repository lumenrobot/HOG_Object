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
        [JsonProperty("hasPosition")]
        public bool hasPosition = true;
        [JsonProperty("hasDistance")]
        public bool hasDistance = false;
        [JsonProperty("hasYaw")]
        public bool hasYaw = false;
        public List<RecognizedObject> trashes = new List<RecognizedObject>();
        public List<RecognizedObject> trashCans = new List<RecognizedObject>();
    }
}