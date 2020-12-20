using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SmokeMe.Tests.Helpers
{
    public static class ActionResultExtensions
    {
        public static T ExtractValue<T>(this IActionResult actionResult)
        {
            if (actionResult is OkObjectResult result)
            {
                return (T)result.Value;
            }

            return (T)((ObjectResult)actionResult).Value;
        }

        public static void CheckIsOk200(this IActionResult actionResult)
        {
            if (actionResult is OkResult)
            {
                return;
            }

            var result = (ObjectResult)actionResult;
            var isOk = result.StatusCode.HasValue && result.StatusCode.Value == (int)HttpStatusCode.OK;

            if (!isOk)
            {
                throw new Exception($"The response does not have the expected 200 HttpStatusCode. HttpStatusCode was: '{result.StatusCode.Value}'.");
            }
        }

        public static T CheckIsError<T>(this IActionResult actionResult, HttpStatusCode expectedHttpStatusCode)
        {
            var result = (ObjectResult)actionResult;

            var isErrorWithCode = result.StatusCode.HasValue && result.StatusCode.Value == (int)expectedHttpStatusCode;

            if (!isErrorWithCode)
            {
                throw new Exception($"The response does not have the expected HttpStatusCode. Expected: {expectedHttpStatusCode} but was: '{result.StatusCode.Value}'.");
            }

            return (T)result.Value;
        }
    }
}