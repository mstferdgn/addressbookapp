using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookEL.ResultModels
{
    public class DataResult<T> : IDataResult<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public DataResult()
        {
            
        }

        public DataResult(bool issucces, string? msg, T data)
        {
            this.Message = msg;
            this.IsSuccess = issucces;
            this.Data = data;
        }
    }
}
