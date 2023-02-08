namespace University.Portal.Web.Models
{
	public class ErrorViewModel
	{
		public string RequestId { get; set; }
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
		public Exception Exception { get; set; }
		public int StatusCode { get; set; }
		public string Message { get; set; }
	}
}