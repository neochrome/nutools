using System;
using System.Net;

namespace NuTools.WGet
{
	class WebClientWithInfo : WebClient
	{
		public class ResponseEventArgs : EventArgs
		{
			public WebResponse Response;
		}
		public event EventHandler<ResponseEventArgs> GotResponse;
		
		protected override WebResponse GetWebResponse (WebRequest request)
		{
			var response = base.GetWebResponse(request);
			OnGotResponse(new ResponseEventArgs { Response = response });
			return response;
		}
		
		protected virtual void OnGotResponse(ResponseEventArgs args)
		{
			if(GotResponse != null)
				GotResponse(this, args);
		}
	}
}