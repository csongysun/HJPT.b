using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csys.Common;

namespace Csys.Common
{
    public class TaskResult
    {
        public object Obj { get; protected set; }
        public bool Succeeded { get; protected set; }
        public static TaskResult Success { get; } = new TaskResult { Succeeded = true };
        public List<Error> Errors { get; protected set; }
        public static TaskResult Failed(List<Error> errors)
        {
            var result = new TaskResult { Succeeded = false};
            if (result.Errors != null)
            {
                result.Errors.AddRange(errors);
            }
            else result.Errors = errors;
            return result;
        }
        public static TaskResult Failed(params Error[] errors)
        {
            var result = new TaskResult { Succeeded = false };
            if (result.Errors != null)
            {
                result.Errors.AddRange(errors);
            }
            else result.Errors = errors.ToList();
            return result;
        }
        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }

    }

    public class TaskResult<T> :TaskResult
    {

        public static new TaskResult<T> Success(T obj) => new TaskResult<T> { Succeeded = true, Obj = obj };

        public override string ToString()
        {
            return Succeeded ?
                   "Succeeded" :
                   string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }

    }


    public static class EntityResult
    {
        public static TaskResult EntityNotFound => TaskResult.Failed(ErrorDescriber.ItemNotFound);
        public static TaskResult SqlFailed(string msg) => TaskResult.Failed(ErrorDescriber.SqlFailed(msg));
    }

}
