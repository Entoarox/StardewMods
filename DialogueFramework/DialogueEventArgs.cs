using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace DialogueFramework
{
    public class DialogueEventArgs : EventArgs
    {
        private readonly List<Response> Responses;
        internal DialogueEventArgs(List<Response> responses)
        {
            this.Responses = responses;
        }

        public void AddResponse(Response response)
        {
            this.Responses.Add(response);
        }

        public void ReplaceResponse(string responseKey, Response response)
        {
            this.Responses[this.Responses.FindIndex(a => a.responseKey.Equals(responseKey))] = response;
        }
    }
}
