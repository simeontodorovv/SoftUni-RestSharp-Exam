﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestSharpAPITests.Models
{
    public  class ErrorDTO
    {
        [JsonPropertyName("errMsg")]
        public string Message { get; set; } 
    }
}
