using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentResults;

namespace WebAPI.Helpers
{
    public class ErrorStringHelper
    {
        public static string AppendErrors(IEnumerable<IError> errors)
        {
            var res = errors.Aggregate(
                    new StringBuilder(),
                    (current, next) => current.Append(current.Length == 0 ? "" : ";").Append(next.Message))
                .ToString();

            return res;
        }
    }
}