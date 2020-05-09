using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DarkAgeBot.Reliability
{
    public static class Retrier
    {
        public static T Attempt<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempts = 0; attempts < maxAttemptCount; attempts++)
            {
                try
                {          
                    var result = action.Invoke();

                    if (result.Equals(typeof(bool)))
                    {
                        if (Convert.ToBoolean(result))
                        {
                            return action();
                        }
                    }

                    Task.Delay(retryInterval);

                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
