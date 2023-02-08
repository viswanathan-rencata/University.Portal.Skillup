using Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using University.Portal.Web.Models;

namespace University.Portal.Web.Extensions
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{			
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{				
				await HandleExceptionAsync(httpContext, ex);
			}
		}
		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
            var response = context.Response;
			int statusCode = (int)HttpStatusCode.InternalServerError;
			string message = exception?.Message;
            response.Redirect($"/Home/Error/?statusCode={statusCode}&message={message}");
		}
	}	
}
