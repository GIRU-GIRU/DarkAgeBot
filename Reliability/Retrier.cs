using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DarkAgeBot.Reliability
{
    public static class Retrier
    {
        public static T Attempt<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3, bool successIfTrue = false)
        {
            var exceptions = new List<Exception>();

            for (int attempts = 0; attempts < maxAttemptCount; attempts++)
            {
                try
                {
                    T result = action();

                    if (successIfTrue)
                    {
                        if (typeof(T) == typeof(bool))
                        {
                            if (Convert.ToBoolean(result))
                            {
                                return result;
                            }
                        }
                    }



                    else
                    {
                        return result;
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
