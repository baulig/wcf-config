// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.17020
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace WCF.Config.MonoTouch.Test.MyService {
    
    
    [System.ServiceModel.ServiceContractAttribute(Namespace="WCF.Config.MonoTouch.Test.MyService")]
    public interface IMyService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://provcon-faust/TestWCF/IMyService/Hello", ReplyAction="http://provcon-faust/TestWCF/IMyService/HelloResponse")]
        string Hello();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://provcon-faust/TestWCF/IMyService/Hello", ReplyAction="http://provcon-faust/TestWCF/IMyService/HelloResponse", AsyncPattern=true)]
        System.IAsyncResult BeginHello(System.AsyncCallback asyncCallback, object userState);
        
        string EndHello(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://provcon-faust/TestWCF/IMyService/Test", ReplyAction="http://provcon-faust/TestWCF/IMyService/TestResponse")]
        int Test();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://provcon-faust/TestWCF/IMyService/Test", ReplyAction="http://provcon-faust/TestWCF/IMyService/TestResponse", AsyncPattern=true)]
        System.IAsyncResult BeginTest(System.AsyncCallback asyncCallback, object userState);
        
        int EndTest(System.IAsyncResult result);
    }
    
    public interface IMyServiceChannel {
    }
    
    public class MyServiceClient : System.ServiceModel.ClientBase<IMyService>, IMyService {
        
        public MyServiceClient() {
        }
        
        public MyServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MyServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MyServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MyServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpoint) : 
                base(binding, endpoint) {
        }
        
        public string Hello() {
            return base.Channel.Hello();
        }
        
        public System.IAsyncResult BeginHello(System.AsyncCallback asyncCallback, object userState) {
            return base.Channel.BeginHello(asyncCallback, userState);
        }
        
        public string EndHello(System.IAsyncResult result) {
            return base.Channel.EndHello(result);
        }
        
        public int Test() {
            return base.Channel.Test();
        }
        
        public System.IAsyncResult BeginTest(System.AsyncCallback asyncCallback, object userState) {
            return base.Channel.BeginTest(asyncCallback, userState);
        }
        
        public int EndTest(System.IAsyncResult result) {
            return base.Channel.EndTest(result);
        }
        
        protected override IMyService CreateChannel() {
            return ((IMyService)(new MyServiceChannel(this)));
        }
        
        private class MyServiceChannel : ChannelBase<IMyService>, IMyService {
            
            public MyServiceChannel(System.ServiceModel.ClientBase<IMyService> client) : 
                    base(client) {
            }
            
            public string Hello() {
                object[] args = new object[0];
                return ((string)(base.Invoke("Hello", args)));
            }
            
            public System.IAsyncResult BeginHello(System.AsyncCallback asyncCallback, object userState) {
                return base.BeginInvoke("Hello", new object[0], asyncCallback, userState);
            }
            
            public string EndHello(System.IAsyncResult result) {
                object[] args = new object[0];
                return ((string)(base.EndInvoke("Hello", args, result)));
            }
            
            public int Test() {
                object[] args = new object[0];
                return ((int)(base.Invoke("Test", args)));
            }
            
            public System.IAsyncResult BeginTest(System.AsyncCallback asyncCallback, object userState) {
                return base.BeginInvoke("Test", new object[0], asyncCallback, userState);
            }
            
            public int EndTest(System.IAsyncResult result) {
                object[] args = new object[0];
                return ((int)(base.EndInvoke("Test", args, result)));
            }
        }
    }
}
