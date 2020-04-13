using Microsoft.Coyote;
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAspNetCoyote
{
    public class RequestEvent<TRequest, TResult> : Event
    {
        public TRequest Request;
        public Microsoft.Coyote.Tasks.TaskCompletionSource<TResult> Completed = TaskCompletionSource.Create<TResult>();

        public RequestEvent(TRequest request)
        {
            this.Request = request;
        }
    }

    public class RequestResponseActor<TRequest, TResult> : Actor
    {
        private RequestEvent<TRequest, TResult> Request;

        protected override System.Threading.Tasks.Task OnInitializeAsync(Event initialEvent)
        {
            if (initialEvent is RequestEvent<TRequest, TResult> req)
            {
                this.Request = req;
                ProcessRequest(req.Request);
            }
            return base.OnInitializeAsync(initialEvent);
        }

        protected virtual void ProcessRequest(TRequest request)
        {
            throw new NotImplementedException();
        }


        protected void FinishRequest(TResult response)
        {
            this.Request.Completed.SetResult(response);
        }
    }
}
