
using Csys.Common;
using HJPT;
using HJPT.Models;

namespace Csys.Identity
{
    public static class SignInResult
    {
        public static TaskResult PwdNotCorrect => TaskResult.Failed(ErrorDescriber.PwdNotCorrect);
        public static TaskResult ValidateFailed => TaskResult.Failed(ErrorDescriber.ValidateFailed);
        public static TaskResult<User> Success(User user) => TaskResult<User>.Success(user);
    }


}

namespace Csys.Common
{
    public static partial class ErrorDescriber
    {
        public static Error PwdNotCorrect => new Error
        {
            Code = nameof(PwdNotCorrect),
            Description = Resource.PwdNotCorrect
        };
        public static Error ValidateFailed => new Error
        {
            Code = nameof(ValidateFailed),
            Description = Resource.ValidateFailed
        };
    }

}
    

