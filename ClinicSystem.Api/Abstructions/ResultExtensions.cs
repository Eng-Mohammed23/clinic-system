using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Abstractions
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot success result to problem");

            var problem = Results.Problem(statusCode: result.Error.StatusCode);
            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "error",new[]
                    {
                        result.Error.Code,
                        result.Error.Description,
                    }
                }
            };

            //var  problemDetails = new ProblemDetails
            //{
            //    Type = "",
            //    Status=statusCode,
            //        Title= title,
            //        Extensions= new Dictionary<string, object?>
            //        {
            //            {
            //                "error",new[] {result.Error}
            //            }
            //        }
            //};

            return new ObjectResult(problemDetails);
        }
    }
}
