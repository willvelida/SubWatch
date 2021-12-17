using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubWatch.Common.Response
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public void SetErrorMessage(string message)
        {
            Success = false;
            Message = message;
        }

        public void SetSuccessMessage(string message)
        {
            Success = true;
            Message = message;
        }
    }
}
