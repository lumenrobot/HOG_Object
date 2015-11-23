using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanDetection
{
    class RecognizedObject
    {
        [JsonProperty("@type")]
        public String type = "RecognizedObject"; // Untuk menandai tipe data yang dikirim
        public string name;
        public Vector2 topPosition;
        public Vector2 bottomPosition;
    }
}