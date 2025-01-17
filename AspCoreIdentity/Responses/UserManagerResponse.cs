﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreIdentity.Responses
{
    public class UserManagerResponse<T> where T : class
    {
        public string Message { get; set; }
        public bool isSuccess { get; set; }
        public T Data { get; set; }
        public string  Errors { get; set; }
    }
}
