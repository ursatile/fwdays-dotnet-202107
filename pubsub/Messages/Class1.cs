using System;

namespace Messages {
	public class ExampleMessage {
        public int MessageNumber { get;set;}
        public string MessageBody {get;set;}
        public DateTimeOffset MessageSent { get; set; }
        public string MachineName {get;set;}
    }
}
