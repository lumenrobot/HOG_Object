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
        public string type = "RecognizedObject"; // Untuk menandai tipe data yang dikirim
        [JsonProperty("name")]
        public string name { get; set; } // Ini adalah nama objek yang akan dikirim ke server
        //public string name;
        public Vector2 topPosition;
        public Vector2 bottomPosition;
    }
}