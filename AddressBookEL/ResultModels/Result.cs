using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ResultModels
{
    public class Result : IResult
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public Result()
        {
            
        }
        public Result(bool issuccess, string? msg)
        {
            this.IsSuccess = issuccess;
            this.Message = msg;
        }
    }
}
